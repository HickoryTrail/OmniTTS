using System.Globalization;
using System.Net;
using System.Text.Json;
using OmniTTS.Plugin.Helper;
using RestSharp;

namespace OmniTTS.Plugin.Infrastructures
{
    internal class Gemini
    {
        /// <summary>
        /// Represents an audio option for Gemini.
        /// </summary>
        internal class GeminiOption
        {
            internal required string Text { get; set; }
            internal required string Model { get; set; }
            internal required string Voice { get; set; }
            internal required float Speed { get; set; }
            internal required string FileName { get; set; }
        }

        internal class GeminiClient
        {
            private const int SampleRate = 24000;
            private const short Channels = 1;
            private const short BitsPerSample = 16;
            private readonly string _apiKey;
            private readonly string _baseUrl;

            internal GeminiClient(string base_url, string apiKey)
            {
                _baseUrl = ProviderUrlHelper.NormalizeGeminiBaseUrl(base_url);
                _apiKey = apiKey;
            }

            internal GeminiClient()
            {
                _baseUrl = ProviderUrlHelper.NormalizeGeminiBaseUrl("");
                _apiKey = "";
            }

            internal async Task GenerateAudioAsync(GeminiOption option, CancellationToken? cts)
            {
                const int maxAttempts = 3;
                var cancellationToken = cts ?? CancellationToken.None;
                using var client = new RestClient(new RestClientOptions(_baseUrl));

                for (var attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    var request = new RestRequest("/v1beta/interactions", Method.Post)
                        .AddHeader("x-goog-api-key", _apiKey)
                        .AddJsonBody(new
                        {
                            model = option.Model,
                            input = CreateInput(option),
                            response_format = new { type = "audio" },
                            generation_config = new
                            {
                                speech_config = new[]
                                {
                                    new { voice = option.Voice }
                                }
                            }
                        });

                    var response = await client.ExecuteAsync(request, cancellationToken);
                    if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
                    {
                        var pcm = GetPcmAudio(response.Content);
                        var directory = Path.GetDirectoryName(option.FileName);
                        if (!string.IsNullOrEmpty(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        await WriteWaveFileAsync(option.FileName, pcm, cancellationToken);
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

            private static string CreateInput(GeminiOption option)
            {
                if (!float.IsFinite(option.Speed) || option.Speed <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(option.Speed), "Gemini speech speed must be a positive finite value.");
                }

                if (Math.Abs(option.Speed - 1f) < 0.001f)
                {
                    return option.Text;
                }

                return string.Create(
                    CultureInfo.InvariantCulture,
                    $"Read the following text at approximately {option.Speed:0.##} times the normal speaking speed. Do not add or omit words:\n\n{option.Text}");
            }

            private static byte[] GetPcmAudio(string content)
            {
                using var document = JsonDocument.Parse(content);
                if (!document.RootElement.TryGetProperty("output_audio", out var outputAudio) ||
                    !outputAudio.TryGetProperty("data", out var data) ||
                    data.ValueKind != JsonValueKind.String ||
                    string.IsNullOrEmpty(data.GetString()))
                {
                    throw new InvalidDataException("Gemini TTS response did not contain output_audio.data.");
                }

                try
                {
                    return Convert.FromBase64String(data.GetString()!);
                }
                catch (FormatException exception)
                {
                    throw new InvalidDataException("Gemini TTS response contained invalid Base64 audio data.", exception);
                }
            }

            private static async Task WriteWaveFileAsync(string fileName, byte[] pcm, CancellationToken cancellationToken)
            {
                await using var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
                using var writer = new BinaryWriter(stream, System.Text.Encoding.ASCII, leaveOpen: true);
                var byteRate = SampleRate * Channels * BitsPerSample / 8;
                var blockAlign = (short)(Channels * BitsPerSample / 8);

                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + pcm.Length);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((short)1);
                writer.Write(Channels);
                writer.Write(SampleRate);
                writer.Write(byteRate);
                writer.Write(blockAlign);
                writer.Write(BitsPerSample);
                writer.Write("data".ToCharArray());
                writer.Write(pcm.Length);
                writer.Flush();

                await stream.WriteAsync(pcm, cancellationToken);
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
                var message = $"Gemini TTS request failed with status {(int)response.StatusCode} ({response.StatusDescription}). {response.Content}";
                return new HttpRequestException(message, response.ErrorException, response.StatusCode);
            }
        }
    }
}
