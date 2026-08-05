using ClassIsland.Core.Abstractions.Services.SpeechService;
using ClassIsland.Core.Attributes;
using ClassIsland.Shared;
using Microsoft.Extensions.Logging;
using OmniTTS.Shared;

namespace OmniTTS.Plugin.Services
{
    [SpeechProviderInfo("classisland.speech.omniTts", "OmniTTS")]
    public class OmniSpeechService : ISpeechService
    {
        private ILogger<OmniSpeechService> Logger { get; set; }
        private IOmniTTS OmniTTService { get; set; }

        public OmniSpeechService(ILogger<OmniSpeechService> logger)
        {
            Logger = logger;
            Logger.LogInformation("OmniSpeechService created.");
        }
        public void Initialize()
        {
            OmniTTService = IAppHost.GetService<IOmniTTS>();
            Logger.LogInformation("OmniSpeechService initialized.");
        }
        public void EnqueueSpeechQueue(string text)
        {
            Logger.LogTrace("Received ISpeech request to enqueue speech: {Text}", text);
            OmniTTService.PlayAudio(text, CancellationToken.None);
        }
        public void ClearSpeechQueue()
        {
            try
            {
                OmniTTService.CancelAllAudio();
                Logger.LogInformation("Cleared ISpeech queue.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error clearing ISpeech queue");
            }
        }
    }
}
