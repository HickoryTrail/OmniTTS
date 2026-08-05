using System.Net;
using System.Text.Json;
using ClassIsland.Shared;
using OmniTTS.Plugin.Helper;
using OmniTTS.Plugin.Services;
using ReactiveUI;
using RestSharp;

namespace OmniTTS.Plugin.Infrastructures
{
    internal class MiMo
    {
        /// <summary>
        /// Represents an audio option for MiMo
        /// </summary>
        internal class MiMoOption
        {
            internal required string Text { get; set; }
            internal required string Model { get; set; }
            internal required string Voice { get; set; }
            internal required float Speed { get; set; }
            internal required string FilePath { get; set; }
        }

        internal class MiMoClient
        {
            private string _apiKey;
            private string _baseUrl;
            private SettingsService SettingsService { get; set; }

            internal MiMoClient(string baseUrl, string apiKey)
            {
                SettingsService = IAppHost.GetService<SettingsService>();
                _baseUrl = ProviderUrlHelper.NormalizeMiMoBaseUrl(baseUrl);
                _apiKey = apiKey;
            }
            internal MiMoClient()
            {
                SettingsService = IAppHost.GetService<SettingsService>();
                _baseUrl = ProviderUrlHelper.NormalizeMiMoBaseUrl(SettingsService.Setting.ProviderSetting.MiMoSetting.BaseUrl);
                _apiKey = SettingsService.Setting.ProviderSetting.MiMoSetting.ApiKey;
                SettingsService.Setting.ProviderSetting.MiMoSetting.WhenAnyValue(
                    x => x.BaseUrl,
                    x => x.ApiKey).Subscribe(_ =>
                    {
                        _baseUrl = ProviderUrlHelper.NormalizeMiMoBaseUrl(SettingsService.Setting.ProviderSetting.MiMoSetting.BaseUrl);
                        _apiKey = SettingsService.Setting.ProviderSetting.MiMoSetting.ApiKey;
                    });
            }

            internal async Task GenerateAudioAsync(MiMoOption option, CancellationToken? cts)
            {
                const int maxAttempts = 3;
                var cancellationToken = cts ?? CancellationToken.None;
                using var client = new RestClient(new RestClientOptions(_baseUrl));

                for (var attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    var request = new RestRequest("v1/chat/completions", Method.Post)
                        .AddHeader("api-key", _apiKey)
                        .AddHeader("Content-Type", "application/json")
                        .AddJsonBody(new
                        {
                            model = option.Model,
                            messages = new[]
                            {
                                new { role = "assistant", content = option.Text }
                            },
                            audio = new
                            {
                                format = "mp3",
                                voice = option.Voice
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
                    if (!root.TryGetProperty("choices", out var choices) ||
                        choices.ValueKind != JsonValueKind.Array ||
                        choices.GetArrayLength() == 0 ||
                        !choices[0].TryGetProperty("message", out var message) ||
                        !message.TryGetProperty("audio", out var audioElement) ||
                        !audioElement.TryGetProperty("data", out var dataElement))
                    {
                        return false;
                    }

                    var data = dataElement.GetString();
                    if (string.IsNullOrWhiteSpace(data))
                    {
                        return false;
                    }

                    audio = Convert.FromBase64String(data);
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
                var message = $"MiMo TTS request failed with status {(int)response.StatusCode} ({response.StatusDescription}). {response.Content}";
                return new HttpRequestException(message, response.ErrorException, response.StatusCode);
            }
        }
    }
}
