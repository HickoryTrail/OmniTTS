using Microsoft.Extensions.Logging;
using OmniTTS.Plugin.Models;
using OmniTTS.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OmniTTS.Plugin.Services
{
    internal class GenerationService
    {
        private ILogger<OmniTTService>? Logger { get; set; }
        private SettingsService SettingsService { get; set; }
        private Infrastructures.OpenAI.OpenAIClient? OpenAIClient { get; set; }
        private Infrastructures.Gemini.GeminiClient? GeminiClient { get; set; }
        private Infrastructures.FishAudio.FishAudioClient? FishAudioClient { get; set; }
        private Infrastructures.ElevenLabs.ElevenLabsClient? ElevenLabsClient { get; set; }
        private Infrastructures.MiniMax.MiniMaxClient? MiniMaxClient { get; set; }

        internal GenerationService(ILogger<OmniTTService> logger, SettingsService settingsService)
        {
            Logger = logger ?? null;
            SettingsService = settingsService;
            if (SettingsService == null)
            {
                Logger?.LogCritical("SettingsService is null. OmniTTService cannot be initialized.");
                throw new InvalidOperationException("SettingsService is null. OmniTTService cannot be initialized.");
            }
            if (settingsService.Setting.ProviderSetting.OpenAISetting.IsEnabled) OpenAIClient = new Infrastructures.OpenAI.OpenAIClient();
            if (settingsService.Setting.ProviderSetting.GeminiSetting.IsEnabled) GeminiClient = new Infrastructures.Gemini.GeminiClient();
            if (settingsService.Setting.ProviderSetting.FishAudioSetting.IsEnabled) FishAudioClient = new Infrastructures.FishAudio.FishAudioClient();
            if (settingsService.Setting.ProviderSetting.ElevenLabsSetting.IsEnabled) ElevenLabsClient = new Infrastructures.ElevenLabs.ElevenLabsClient();
            if (settingsService.Setting.ProviderSetting.MiniMaxSetting.IsEnabled) MiniMaxClient = new Infrastructures.MiniMax.MiniMaxClient();
        }

        internal void StartWorker(int num, Channel<RequestOption> channel)
        {
            Logger?.LogInformation($"Starting {num} worker threads for generation service.");
            for (int i = 0; i < num; i++)
            {
                _ = Worker(channel);
            }
        }

        private async Task Worker(Channel<RequestOption> channel)
        {
            while (true)
            {
                var requestOption = await channel.Reader.ReadAsync();
                await GenerateAsync(requestOption);
            }
        }

        private async Task GenerateAsync(RequestOption requestOption)
        {
            try
            {
                switch (requestOption.Provider)
                {
                    case Provider.OpenAI:
                        if (OpenAIClient == null) throw new InvalidOperationException("OpenAIClient is not initialized.");
                        var openai_option = new Infrastructures.OpenAI.OpenAIOption
                        {
                            Model = requestOption.Model,
                            Voice = requestOption.Voice,
                            Text = requestOption.Text,
                            Speed = requestOption.Speed,
                            FilePath = requestOption.FilePath,
                        };
                        await OpenAIClient.GenerateAudioAsync(openai_option,requestOption.Cts);
                        break;
                    case Provider.Gemini:
                        if (GeminiClient == null) throw new InvalidOperationException("GeminiClient is not initialized.");
                        var gemini_option = new Infrastructures.Gemini.GeminiOption
                        {
                            Model = requestOption.Model,
                            Voice = requestOption.Voice,
                            Text = requestOption.Text,
                            Speed = requestOption.Speed,
                            FilePath = requestOption.FilePath,
                        };
                        await GeminiClient.GenerateAudioAsync(gemini_option,requestOption.Cts);
                        break;
                    case Provider.FishAudio:
                        if (FishAudioClient == null) throw new InvalidOperationException("FishAudioClient is not initialized.");
                        var fishaudio_option = new Infrastructures.FishAudio.FishAudioOption
                        {
                            Model = requestOption.Model,
                            Voice = requestOption.Voice,
                            Text = requestOption.Text,
                            Speed = requestOption.Speed,
                            FilePath = requestOption.FilePath,
                        };
                        await FishAudioClient.GenerateAudioAsync(fishaudio_option, requestOption.Cts);
                        break;
                    case Provider.Elevenlabs:
                        if (ElevenLabsClient == null) throw new InvalidOperationException("ElevenLabsClient is not initialized.");
                        var elevenlabs_option = new Infrastructures.ElevenLabs.ElevenLabsOption
                        {
                            Model = requestOption.Model,
                            Voice = requestOption.Voice,
                            Text = requestOption.Text,
                            Speed = requestOption.Speed,
                            FilePath = requestOption.FilePath,
                        };
                        await ElevenLabsClient.GenerateAudioAsync(elevenlabs_option, requestOption.Cts);
                        break;
                    case Provider.MiniMax:
                        if (MiniMaxClient == null) throw new InvalidOperationException("MiniMaxClient is not initialized.");
                        var minimax_option = new Infrastructures.MiniMax.MiniMaxOption
                        {
                            Model = requestOption.Model,
                            Voice = requestOption.Voice,
                            Text = requestOption.Text,
                            Speed = requestOption.Speed,
                            FilePath = requestOption.FilePath,
                        };
                        await MiniMaxClient.GenerateAudioAsync(minimax_option, requestOption.Cts);
                        break;
                    default:
                        throw new NotSupportedException($"Provider {requestOption.Provider} is not supported.");

                }
                requestOption.Completion.SetResult(true);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, $"Error processing request for provider {requestOption.Provider}: {ex.Message}");
                requestOption.Completion.SetException(ex);
            }
        }
    }
}
