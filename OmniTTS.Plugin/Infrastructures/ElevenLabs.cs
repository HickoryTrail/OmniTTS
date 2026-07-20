using System.Net;
using RestSharp;
using OmniTTS.Plugin.Helper;

namespace OmniTTS.Plugin.Infrastructures
{
    internal class ElevenLabs
    {
        /// <summary>
        /// Represents an audio option for ElevenLabs
        /// </summary>
        internal class ElevenLabsOption
        {
            internal required string Text { get; set; }
            internal required string Model { get; set; }
            internal required string Voice { get; set; }
            internal required float Speed { get; set; }
            internal required string FileName { get; set; }
        }

        internal class ElevenLabsClient
        {
            private readonly string _apiKey;
            private readonly string _baseUrl;
            internal ElevenLabsClient(string base_url, string apiKey)
            {
                _baseUrl = ProviderUrlHelper.NormalizeElevenLabsBaseUrl(base_url);
                _apiKey = apiKey;
            }
            internal ElevenLabsClient()
            {
                _baseUrl = ProviderUrlHelper.NormalizeElevenLabsBaseUrl("");
                _apiKey = "";
            }
            internal async Task GenerateAudioAsync(ElevenLabsOption option, CancellationToken? cts)
            {
                const int maxAttempts = 3;
                var cancellationToken = cts ?? CancellationToken.None;
                using var client = new RestClient(new RestClientOptions(_baseUrl));

                for (var attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    var request = new RestRequest("v1/text-to-speech/{voiceId}", Method.Post)
                        .AddUrlSegment("voiceId", option.Voice)
                        .AddQueryParameter("output_format", "mp3_44100_128")
                        .AddHeader("xi-api-key", _apiKey)
                        .AddJsonBody(new
                        {
                            text = option.Text,
                            model_id = option.Model,
                            voice_settings = new
                            {
                                speed = option.Speed
                            }
                        });

                    var response = await client.ExecuteAsync(request, cancellationToken);
                    if (response.IsSuccessful && response.RawBytes is { Length: > 0 } audio)
                    {
                        var directory = Path.GetDirectoryName(option.FileName);
                        if (!string.IsNullOrEmpty(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        await File.WriteAllBytesAsync(option.FileName, audio, cancellationToken);
                        return;
                    }

                    if (attempt < maxAttempts && IsRetryable(response))
                    {
                        await Task.Delay(GetRetryDelay(response, attempt), cancellationToken);
                        continue;
                    }

                    throw CreateRequestException(response);
                }
            }

            private static bool IsRetryable(RestResponse response)
            {
                if (response.ErrorException is not null)
                {
                    return true;
                }

                return response.StatusCode == HttpStatusCode.RequestTimeout ||
                       response.StatusCode == (HttpStatusCode)429 ||
                       (int)response.StatusCode >= 500;
            }

            private static TimeSpan GetRetryDelay(RestResponse response, int attempt)
            {
                var retryAfter = response.Headers?
                    .FirstOrDefault(header => string.Equals(header.Name, "Retry-After", StringComparison.OrdinalIgnoreCase))
                    ?.Value?.ToString();

                if (int.TryParse(retryAfter, out var seconds) && seconds >= 0)
                {
                    return TimeSpan.FromSeconds(seconds);
                }

                return TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
            }

            private static HttpRequestException CreateRequestException(RestResponse response)
            {
                var message = $"ElevenLabs TTS request failed with status {(int)response.StatusCode} ({response.StatusDescription}). {response.Content}";
                return new HttpRequestException(message, response.ErrorException, response.StatusCode);
            }
        }
    }
}
