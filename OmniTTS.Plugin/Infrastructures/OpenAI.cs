using OpenAI;
using OpenAI.Audio;
using OmniTTS.Plugin.Helper;
using System.ClientModel;

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
            private readonly string _apiKey;
            private readonly string _baseUrl;
            internal OpenAIClient(string base_url, string apiKey)
            {
                _baseUrl = ProviderUrlHelper.NormalizeOpenAIBaseUrl(base_url);
                _apiKey = apiKey;
            }
            internal OpenAIClient()
            {
                _baseUrl = ProviderUrlHelper.NormalizeOpenAIBaseUrl("");
                _apiKey = "";
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
