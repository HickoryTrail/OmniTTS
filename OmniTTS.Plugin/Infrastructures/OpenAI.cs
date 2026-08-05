using OpenAI;
using OpenAI.Audio;
using OmniTTS.Plugin.Helper;
using System.ClientModel;
using OmniTTS.Plugin.Services;
using ClassIsland.Shared;
using ReactiveUI;

namespace OmniTTS.Plugin.Infrastructures
{
    internal class OpenAI
    {
        /// <summary>
        /// Represents an audio option for OpenAI
        /// </summary>
        internal class OpenAIOption
        {
            internal required string Text { get; set; }
            internal required string Model { get; set; }
            internal required string Voice { get; set; }
            internal required float Speed { get; set; }
            internal required string FilePath { get; set; }
        }

        internal class OpenAIClient
        {
            private string _apiKey;
            private string _baseUrl;
            private SettingsService SettingsService { get; set; }
            internal OpenAIClient(string base_url, string apiKey)
            {
                SettingsService = IAppHost.GetService<SettingsService>();
                _baseUrl = ProviderUrlHelper.NormalizeOpenAIBaseUrl(base_url);
                _apiKey = apiKey;
            }
            internal OpenAIClient()
            {
                SettingsService = IAppHost.GetService<SettingsService>();
                _baseUrl = ProviderUrlHelper.NormalizeOpenAIBaseUrl(SettingsService.Setting.ProviderSetting.OpenAISetting.BaseUrl);
                _apiKey = SettingsService.Setting.ProviderSetting.OpenAISetting.ApiKey;
                SettingsService.Setting.ProviderSetting.OpenAISetting.WhenAnyValue(
                    x => x.BaseUrl,
                    x => x.ApiKey).Subscribe(_ =>
                    {
                        _baseUrl = ProviderUrlHelper.NormalizeOpenAIBaseUrl(SettingsService.Setting.ProviderSetting.OpenAISetting.BaseUrl);
                        _apiKey = SettingsService.Setting.ProviderSetting.OpenAISetting.ApiKey;
                    });
            }
            internal async Task GenerateAudioAsync(OpenAIOption option, CancellationToken? cts)
            {
                var clientOptions = new OpenAIClientOptions();
                if (!string.IsNullOrWhiteSpace(_baseUrl))
                {
                    clientOptions.Endpoint = new Uri(_baseUrl, UriKind.Absolute);
                }

                var client = new AudioClient(
                    option.Model,
                    new ApiKeyCredential(_apiKey),
                    clientOptions);
                var cancellationToken = cts ?? CancellationToken.None;
                var speech = await client.GenerateSpeechAsync(
                    option.Text,
                    new GeneratedSpeechVoice(option.Voice),
                    new SpeechGenerationOptions
                    {
                        SpeedRatio = option.Speed,
                        ResponseFormat = GeneratedSpeechFormat.Mp3
                    },
                    cancellationToken);

                var directory = Path.GetDirectoryName(option.FilePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllBytesAsync(option.FilePath, speech.Value.ToArray(), cancellationToken);
            }
        }
    }
}
