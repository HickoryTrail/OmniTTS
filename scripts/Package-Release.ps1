[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+$')]
    [string]$Version,

    [string]$OutputDirectory
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repositoryRoot 'artifacts\release'
}

$releaseDirectory = Join-Path $OutputDirectory $Version
$nugetDirectory = Join-Path $releaseDirectory 'nuget'
$pluginDirectory = Join-Path $releaseDirectory 'plugin'
$cipxPath = Join-Path $releaseDirectory 'HickoryTrail.OmniTTS.cipx'
$releaseNoteSource = Join-Path $repositoryRoot "docs\CHANGELOG\$Version.md"
$releaseNotePath = Join-Path $releaseDirectory 'release-notes.md'
$nugetVersion = $Version.Substring(0, $Version.LastIndexOf('.'))

if (-not (Test-Path -LiteralPath $releaseNoteSource -PathType Leaf)) {
    throw "Release note not found: $releaseNoteSource"
}

if (Test-Path -LiteralPath $releaseDirectory) {
    throw "Output directory already exists: $releaseDirectory. Choose a different -OutputDirectory or remove it first."
}

New-Item -ItemType Directory -Path $nugetDirectory -Force | Out-Null
New-Item -ItemType Directory -Path $pluginDirectory -Force | Out-Null

Push-Location $repositoryRoot
try {
    dotnet restore 'OmniTTS.Shared\OmniTTS.Shared.csproj'
    if ($LASTEXITCODE -ne 0) { throw 'Failed to restore OmniTTS.Shared.' }

    dotnet restore 'OmniTTS.Plugin\OmniTTS.Plugin.csproj'
    if ($LASTEXITCODE -ne 0) { throw 'Failed to restore OmniTTS.Plugin.' }

    dotnet pack 'OmniTTS.Shared\OmniTTS.Shared.csproj' --configuration Release --no-restore `
        --output $nugetDirectory "-p:Version=$nugetVersion"
    if ($LASTEXITCODE -ne 0) { throw 'Failed to pack OmniTTS.Shared.' }

    dotnet publish 'OmniTTS.Plugin\OmniTTS.Plugin.csproj' --configuration Release --no-restore `
        --output $pluginDirectory
    if ($LASTEXITCODE -ne 0) { throw 'Failed to publish OmniTTS.Plugin.' }
}
finally {
    Pop-Location
}

$manifestPath = Join-Path $pluginDirectory 'manifest.yml'
if (-not (Test-Path -LiteralPath $manifestPath -PathType Leaf)) {
    throw "Published plugin manifest not found: $manifestPath"
}

$manifest = Get-Content -LiteralPath $manifestPath -Raw
if ($manifest -notmatch '(?m)^version: .*$') {
    throw "No version field was found in $manifestPath"
}
[System.IO.File]::WriteAllText(
    $manifestPath,
    [regex]::Replace($manifest, '(?m)^version: .*$', "version: $Version"),
    [System.Text.UTF8Encoding]::new($false))

Get-ChildItem -LiteralPath $pluginDirectory -Recurse -File -Filter '*.pdb' |
    Remove-Item -Force

$pluginContents = @(Get-ChildItem -LiteralPath $pluginDirectory -Force)
if ($pluginContents.Count -eq 0) {
    throw 'Plugin publish output is empty.'
}
Compress-Archive -Path $pluginContents.FullName -DestinationPath $cipxPath -CompressionLevel Optimal

$nugetPackages = @(
    Get-ChildItem -LiteralPath $nugetDirectory -File -Filter '*.nupkg' |
        Where-Object { $_.Name -notlike '*.snupkg' }
)
if ($nugetPackages.Count -ne 1) {
    throw "Expected exactly one NuGet package, found $($nugetPackages.Count)."
}

$cipxMd5 = (Get-FileHash -LiteralPath $cipxPath -Algorithm MD5).Hash.ToLowerInvariant()
$nugetMd5 = (Get-FileHash -LiteralPath $nugetPackages[0].FullName -Algorithm MD5).Hash.ToLowerInvariant()
$releaseNote = Get-Content -LiteralPath $releaseNoteSource -Raw
$releaseNote = $releaseNote.Replace('<CIPX MD5>', $cipxMd5)
$releaseNote = $releaseNote.Replace('<NUGET FILENAME>', $nugetPackages[0].Name)
$releaseNote = $releaseNote.Replace('<NUGET MD5>', $nugetMd5)
[System.IO.File]::WriteAllText($releaseNotePath, $releaseNote, [System.Text.UTF8Encoding]::new($false))

Write-Host "Local release package created: $releaseDirectory"
Write-Host "NuGet package: $($nugetPackages[0].FullName)"
Write-Host "NuGet MD5: $nugetMd5"
Write-Host "Plugin package: $cipxPath"
Write-Host "Plugin MD5: $cipxMd5"
Write-Host "Release note: $releaseNotePath"
