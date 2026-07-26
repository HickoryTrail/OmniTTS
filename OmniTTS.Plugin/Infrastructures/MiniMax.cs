using System.Net;
using System.Text.Json;
using RestSharp;
using OmniTTS.Plugin.Helper;

namespace OmniTTS.Plugin.Infrastructures
{
    internal class MiniMax
    {
        /// <summary>
        /// Represents an audio option for MiniMax
        /// </summary>
        internal class MiniMaxOption
        {
            internal required string Text { get; set; }
            internal required string Model { get; set; }
            internal required string Voice { get; set; }
            internal required float Speed { get; set; }
            internal required string FilePath { get; set; }
        }

        internal class MiniMaxClient
        {
            private readonly string _apiKey;
            private readonly string _baseUrl;
            internal MiniMaxClient(string base_url, string apiKey)
            {
                _baseUrl = ProviderUrlHelper.NormalizeMiniMaxBaseUrl(base_url);
                _apiKey = apiKey;
            }
            internal MiniMaxClient()
            {
                _baseUrl = ProviderUrlHelper.NormalizeMiniMaxBaseUrl("");
                _apiKey = "";
            }
            internal async Task GenerateAudioAsync(MiniMaxOption option, CancellationToken? cts)
            {
                const int maxAttempts = 3;
                var cancellationToken = cts ?? CancellationToken.None;
                using var client = new RestClient(new RestClientOptions(_baseUrl));

                for (var attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    var request = new RestRequest("/v1/t2a_v2", Method.Post)
                        .AddHeader("Authorization", $"Bearer {_apiKey}")
                        .AddHeader("Content-Type", "application/json")
                        .AddJsonBody(new
                        {
                            model = option.Model,
                            text = option.Text,
                            stream = false,
                            language_boost = "auto",
                            output_format = "hex",
                            voice_setting = new
                            {
                                voice_id = option.Voice,
                                speed = option.Speed,
                                vol = 1,
                                pitch = 0
                            },
                            audio_setting = new
                            {
                                sample_rate = 32000,
                                bitrate = 128000,
                                format = "mp3",
                                channel = 1
                            }
                        });

                    var response = await client.ExecuteAsync(request, cancellationToken);
                    if (response.IsSuccessful && TryGetAudio(response.Content, out var audio))
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

            private static bool TryGetAudio(string? content, out byte[] audio)
            {
                audio = Array.Empty<byte>();
                if (string.IsNullOrWhiteSpace(content))
                {
                    return false;
                }

                try
                {
                    using var document = JsonDocument.Parse(content);
                    var root = document.RootElement;
                    if (!root.TryGetProperty("base_resp", out var baseResponse) ||
                        !baseResponse.TryGetProperty("status_code", out var statusCode) ||
                        statusCode.GetInt32() != 0 ||
                        !root.TryGetProperty("data", out var data) ||
                        !data.TryGetProperty("audio", out var audioElement))
                    {
                        return false;
                    }

                    var hexAudio = audioElement.GetString();
                    if (string.IsNullOrWhiteSpace(hexAudio))
                    {
                        return false;
                    }

                    audio = Convert.FromHexString(hexAudio);
                    return audio.Length > 0;
                }
                catch (JsonException)
                {
                    return false;
                }
                catch (FormatException)
                {
                    return false;
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
                var message = $"MiniMax TTS request failed with status {(int)response.StatusCode} ({response.StatusDescription}). {response.Content}";
                return new HttpRequestException(message, response.ErrorException, response.StatusCode);
            }
        }
    }
}
