using ClassIsland.Shared;
using Microsoft.Extensions.Logging;
using OmniTTS.Plugin.Models;
using OmniTTS.Shared;
using System.Text;
using System.Threading.Channels;

namespace OmniTTS.Plugin.Services
{
    internal class OmniTTService : IOmniTTS
    {
        private ILogger<OmniTTService> Logger { get; set; }
        private SettingsService SettingsService { get; set; }
        private GenerationService GenerationService { get; set; }
        private AudioService AudioService { get; set; }
        private Channel<RequestOption> GenerateChannel { get; set; } = Channel.CreateUnbounded<RequestOption>();
        private Channel<PlayOption> AudioPlayChannel { get; set; } = Channel.CreateUnbounded<PlayOption>();

        public OmniTTService(ILogger<OmniTTService> logger)
        {
            Logger = logger;
            Logger.LogInformation("OmniTTService created.");
        }

        internal void Initialize()
        {
            SettingsService = IAppHost.GetService<SettingsService>() ?? throw new InvalidOperationException("SettingsService is null. OmniTTService cannot be initialized.");
            GenerationService = IAppHost.GetService<GenerationService>() ?? throw new InvalidOperationException("GenerationService is null. OmniTTService cannot be initialized.");
            AudioService = IAppHost.GetService<AudioService>() ?? throw new InvalidOperationException("AudioService is null. OmniTTService cannot be initialized.");
            GenerationService.Initialize(SettingsService);
            GenerationService.StartWorker(SettingsService.Setting.MaxConcurrentRequests, GenerateChannel);
            AudioService.Start(AudioPlayChannel);
            Logger.LogInformation("OmniTTService initialized.");
        }



        // Play Audio
        public async Task PlayAudioAsync(TtsOption option, CancellationToken cts)
        {
            if (!CheckOption(option.Text, cts))
            {
                Logger.LogWarning($"Request canceled: {option.Text}");
                return;
            }
            Logger.LogInformation($"Start to process: {option.Text}");
            string filename = await GenerateCacheAsync(option, cts);

            // Play file
            var playOption = new PlayOption
            {
                FilePath = filename,
                Volume = option.Volume ?? 1.0f,
                Cts = cts
            };
            Logger.LogTrace($"Start to play: {option.Text}");
            await AudioPlayChannel.Writer.WriteAsync(playOption, cts);
            await playOption.Completion.Task.WaitAsync(cts);
            Logger.LogInformation($"Finished processing: {option.Text}");
        }
        public async Task PlayAudioAsync(string text, CancellationToken cts)
        {
            if (!CheckOption(text, cts)) return;

            var option = new TtsOption
            {
                Text = text
            };
            FormatTtsOption(option);
            await PlayAudioAsync(option, cts);

        }
        public void PlayAudio(TtsOption option, CancellationToken cts)
        {
            if (!CheckOption(option.Text, cts)) return;
            _ = SafeRunAsync(option, cts, PlayAudioAsync);
        }
        public void PlayAudio(string text, CancellationToken cts)
        {
            if (!CheckOption(text, cts)) return;
            _ = SafeRunAsync(text, cts, PlayAudioAsync);
        }
        public void CancelAllAudio()
        {
            while (AudioPlayChannel.Reader.TryRead(out var request))
            {
                request.Completion.SetCanceled();
            }
        }

        // Generate Cache
        public async Task<string> GenerateCacheAsync(TtsOption option, CancellationToken cts)
        {
            if (!CheckOption(option.Text, cts)) return String.Empty;
            FormatTtsOption(option);
            var filename = GetFilePath(option);
            if (File.Exists(filename))
            {
                Logger.LogInformation($"GenerationService skipped: {option.Text} [{filename}]");
                return filename;
            }
            Logger.LogTrace($"Start to generate: {option.Text} [{filename}]");
            RequestOption options = new RequestOption
            {
                Provider = option.Provider ?? throw new ArgumentNullException(nameof(option)),
                Model = option.Model ?? throw new ArgumentNullException(nameof(option)),
                Voice = option.Voice ?? throw new ArgumentNullException(nameof(option)),
                Speed = option.Speed ?? throw new ArgumentNullException(nameof(option)),
                Text = option.Text,
                Cts = cts,
                FilePath = filename
            };
            await GenerateChannel.Writer.WriteAsync(options, cts);
            try
            {
                await options.Completion.Task.WaitAsync(cts);
            }
            catch (OperationCanceledException) when (cts.IsCancellationRequested)
            {
                // 外部取消了等待
            }
            catch
            {
                // Completion.Task 内部失败
                throw;
            }
            return filename;
        }
        public async Task<string> GenerateCacheAsync(string text, CancellationToken cts)
        {
            if (!CheckOption(text, cts)) return String.Empty;

            var option = new TtsOption
            {
                Text = text
            };
            FormatTtsOption(option);
            await GenerateCacheAsync(option, cts);
            return GetFilePath(option);
        }
        public void GenerateCache(TtsOption option, CancellationToken cts)
        {
            if (!CheckOption(option.Text, cts)) return;
            _ = SafeRunAsync(option, cts, GenerateCacheAsync);
        }
        public void GenerateCache(string text, CancellationToken cts)
        {
            if (!CheckOption(text, cts)) return;
            _ = SafeRunAsync(text, cts, GenerateCacheAsync);
        }

        // 批量缓存
        public async Task<Dictionary<string, string>> GenerateCacheAsync(List<TtsOption> options, CancellationToken cts)
        {
            Dictionary<string, string> result = new();
            List<RequestOption> requestOptions = new();
            foreach (var option in options)
            {
                if (!CheckOption(option.Text, cts)) continue;
                FormatTtsOption(option);
                result.Add(option.Text, GetFilePath(option));
                RequestOption r_options = new RequestOption
                {
                    Provider = option.Provider ?? throw new ArgumentNullException(nameof(option)),
                    Model = option.Model ?? throw new ArgumentNullException(nameof(option)),
                    Voice = option.Voice ?? throw new ArgumentNullException(nameof(option)),
                    Speed = option.Speed ?? throw new ArgumentNullException(nameof(option)),
                    Text = option.Text,
                    Cts = cts,
                    FilePath = GetFilePath(option)
                };
                await GenerateChannel.Writer.WriteAsync(r_options, cts);
                requestOptions.Add(r_options);
            }
            try
            {
                await Task.WhenAll(requestOptions.Select(r => r.Completion.Task));
            }
            catch (OperationCanceledException) when (cts.IsCancellationRequested)
            {
                // 外部取消了等待
            }
            catch
            {
                // Completion.Task 内部失败
                throw;
            }
            return result;
        }
        public async Task<Dictionary<string, string>> GenerateCacheAsync(List<string> texts, CancellationToken cts)
        {
            List<TtsOption> options = new();
            foreach (var text in texts)
            {
                if (!CheckOption(text, cts)) continue;
                var option = new TtsOption
                {
                    Text = text
                };
                FormatTtsOption(option);
                options.Add(option);
            }
            return await GenerateCacheAsync(options, cts);
        }
        public void GenerateCache(List<TtsOption> options, CancellationToken cts)
        {
            _ = SafeRunAsync(options, cts, GenerateCacheAsync);
        }
        public void GenerateCache(List<string> texts, CancellationToken cts)
        {
            _ = SafeRunAsync(texts, cts, GenerateCacheAsync);
        }

        // 工具方法
        public async Task ClearCache()
        {
            if (!Path.Exists(SettingsService.PluginConfigFolder)) return;
            var path = Path.Combine(SettingsService.PluginConfigFolder, "Cache");

            if (Directory.Exists(path))
            {
                foreach (var file in Directory.GetFiles(path))
                    File.Delete(file);

                foreach (var directory in Directory.GetDirectories(path))
                    Directory.Delete(directory, true);
            }
        }

        private bool CheckOption(string text, CancellationToken cts)
        {
            if (cts.IsCancellationRequested) return false;
            if (string.IsNullOrWhiteSpace(text))
            {
                Logger?.LogWarning("Text is null or whitespace. Cannot generate audio.");
                throw new ArgumentException("Text is null or whitespace. Cannot generate audio.");
            }
            if (SettingsService.Setting.DefaultProvider == Provider.None)
            {
                Logger?.LogWarning("Default provider is not set. Cannot generate audio.");
                throw new InvalidOperationException("Default provider is not set. Cannot generate audio.");
            }
            return true;
        }
        private string GetFilePath(TtsOption option)
        {
            var hash_option = new TtsOption()
            {
                Provider = option.Provider,
                Model = option.Model,
                Voice = option.Voice,
                Speed = option.Speed,
                Text = option.Text,
                Volume = 1.0f
            };
            string optionJson = System.Text.Json.JsonSerializer.Serialize(hash_option);
            string optionHash = Convert.ToHexString(System.Security.Cryptography.MD5.HashData(Encoding.UTF8.GetBytes(optionJson)));
            return Path.Combine(SettingsService?.PluginConfigFolder ?? "", "Cache", $"{optionHash}.mp3");
        }
        private void FormatTtsOption(TtsOption option)
        {
            string model = "";
            string voice = "";
            float speed = 1.0f;
            float volume = 1.0f;
            switch (SettingsService.Setting.DefaultProvider)
            {
                case Provider.OpenAI:
                    model = SettingsService.Setting.ProviderSetting.OpenAISetting.Model;
                    voice = SettingsService.Setting.ProviderSetting.OpenAISetting.Voice;
                    speed = SettingsService.Setting.ProviderSetting.OpenAISetting.Speed;
                    volume = SettingsService.Setting.ProviderSetting.OpenAISetting.Volume;
                    break;
                case Provider.Gemini:
                    model = SettingsService.Setting.ProviderSetting.GeminiSetting.Model;
                    voice = SettingsService.Setting.ProviderSetting.GeminiSetting.Voice;
                    speed = SettingsService.Setting.ProviderSetting.GeminiSetting.Speed;
                    volume = SettingsService.Setting.ProviderSetting.GeminiSetting.Volume;
                    break;
                case Provider.FishAudio:
                    model = SettingsService.Setting.ProviderSetting.FishAudioSetting.Model;
                    voice = SettingsService.Setting.ProviderSetting.FishAudioSetting.Voice;
                    speed = SettingsService.Setting.ProviderSetting.FishAudioSetting.Speed;
                    volume = SettingsService.Setting.ProviderSetting.FishAudioSetting.Volume;
                    break;
                case Provider.Elevenlabs:
                    model = SettingsService.Setting.ProviderSetting.ElevenLabsSetting.Model;
                    voice = SettingsService.Setting.ProviderSetting.ElevenLabsSetting.Voice;
                    speed = SettingsService.Setting.ProviderSetting.ElevenLabsSetting.Speed;
                    volume = SettingsService.Setting.ProviderSetting.ElevenLabsSetting.Volume;
                    break;
                case Provider.MiniMax:
                    model = SettingsService.Setting.ProviderSetting.MiniMaxSetting.Model;
                    voice = SettingsService.Setting.ProviderSetting.MiniMaxSetting.Voice;
                    speed = SettingsService.Setting.ProviderSetting.MiniMaxSetting.Speed;
                    volume = SettingsService.Setting.ProviderSetting.MiniMaxSetting.Volume;
                    break;
                case Provider.MiMo:
                    model = SettingsService.Setting.ProviderSetting.MiMoSetting.Model;
                    voice = SettingsService.Setting.ProviderSetting.MiMoSetting.Voice;
                    volume = SettingsService.Setting.ProviderSetting.MiMoSetting.Volume;
                    break;
            }
            option.Provider ??= SettingsService.Setting.DefaultProvider;
            option.Model ??= model;
            option.Voice ??= voice;
            option.Volume ??= volume;
            option.Speed ??= speed;
            return;
        }

        private async Task SafeRunAsync<T>(T value, CancellationToken cts, Func<T, CancellationToken, Task<T>> func)
        {
            try
            {
                await func(value, cts);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while processing the request.");
            }
        }
        private async Task SafeRunAsync<T>(T value, CancellationToken cts, Func<T, CancellationToken, Task> func)
        {
            try
            {
                await func(value, cts);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while processing the request.");
            }
        }
    }
}
