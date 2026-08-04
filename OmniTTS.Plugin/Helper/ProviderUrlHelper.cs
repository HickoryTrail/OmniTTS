namespace OmniTTS.Plugin.Helper;

internal static class ProviderUrlHelper
{
    internal static string NormalizeFishAudioBaseUrl(string url) => Normalize(url, "v1/tts", "v1");

    internal static string NormalizeOpenAIBaseUrl(string url) => Normalize(url, "v1/audio/speech", "v1/audio", "v1");

    internal static string NormalizeMiniMaxBaseUrl(string url) => Normalize(url, "v1/t2a_v2", "v1");

    internal static string NormalizeMiMoBaseUrl(string url) => Normalize(url, "v1/chat/completions", "v1/chat", "v1");

    internal static string NormalizeGeminiBaseUrl(string url) => Normalize(url, "v1beta/interactions", "v1beta");

    internal static string NormalizeElevenLabsBaseUrl(string url) => Normalize(url, "v1/text-to-speech", "v1");

    private static string Normalize(string url, params string[] providerPaths)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("A valid absolute HTTP or HTTPS URL is required.", nameof(url));
        }

        var path = uri.AbsolutePath.Trim('/');
        var providerPathToRemove = providerPaths
            .OrderByDescending(providerPath => providerPath.Length)
            .FirstOrDefault(providerPath =>
                path.Equals(providerPath, StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith($"/{providerPath}", StringComparison.OrdinalIgnoreCase));

        if (providerPathToRemove is not null)
        {
            path = path.Length == providerPathToRemove.Length
                ? string.Empty
                : path[..^(providerPathToRemove.Length + 1)];
        }

        var builder = new UriBuilder(uri)
        {
            Path = path,
            Query = string.Empty,
            Fragment = string.Empty
        };

        return builder.Uri.AbsoluteUri.TrimEnd('/');
    }
}
