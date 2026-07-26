using System.Net;
using RestSharp;
using OmniTTS.Plugin.Helper;

namespace OmniTTS.Plugin.Infrastructures
{
    internal class FishAudio
    { 
        /// <summary>
        /// Represents an audio option for FishAudio
        /// </summary>
        internal class FishAudioOption
        {
            internal required string Text { get; set; }
            internal required string Model { get; set; }
            internal required string Voice { get; set; }
            internal required float Speed { get; set; }
            internal required string FilePath { get; set; }
        }

        internal class FishAudioClient
        {
            private readonly string _apiKey;
            private readonly string _baseUrl;
            internal FishAudioClient(string base_url,string apiKey)
            {
                _baseUrl = ProviderUrlHelper.NormalizeFishAudioBaseUrl(base_url);
                _apiKey = apiKey;
            }
            internal FishAudioClient()
            {
                _baseUrl = ProviderUrlHelper.NormalizeFishAudioBaseUrl("");
                _apiKey = "";
            }
            internal async Task GenerateAudioAsync(FishAudioOption option, CancellationToken? cts)
            {
                const int maxAttempts = 3;
                var cancellationToken = cts ?? CancellationToken.None;
                using var client = new RestClient(new RestClientOptions(_baseUrl));

                for (var attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    var request = new RestRequest("/v1/tts", Method.Post)
                        .AddHeader("Authorization", $"Bearer {_apiKey}")
                        .AddHeader("model", option.Model)
                        .AddJsonBody(new
                        {
                            text = option.Text,
                            reference_id = option.Voice,
                            prosody = new
                            {
                                speed = option.Speed,
                                volume = 1,
                                normalize_loudness = true
                            },
                            format = "mp3",
                            sample_rate = 44100,
                            mp3_bitrate = 128
                        });

                    var response = await client.ExecuteAsync(request, cancellationToken);
                    if (response.IsSuccessful && response.RawBytes is { Length: > 0 } audio)
                    {
                        var directory = Path.GetDirectoryName(option.FilePath);
                        if (!string.IsNullOrEmpty(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        await File.WriteAllBytesAsync(option.FilePath, audio, cancellationToken);
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
                var message = $"Fish Audio TTS request failed with status {(int)response.StatusCode} ({response.StatusDescription}). {response.Content}";
                return new HttpRequestException(message, response.ErrorException, response.StatusCode);
            }
        }
    }
}
