using ClassIsland.Core.Abstractions.Services;
using Microsoft.Extensions.Logging;
using OmniTTS.Plugin.Models;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OmniTTS.Plugin.Services
{
    internal class AudioService
    {
        private IAudioService Ci_AudioService { get; set; }
        private ILogger<AudioService> Logger { get; set; }
        internal AudioService(ILogger<AudioService> logger, IAudioService audioService)
        {
            Ci_AudioService = audioService;
            Logger = logger;
        }

        public void Start(Channel<PlayOption> clannel)
        {
            _ = Worker(clannel);
        }

        internal async Task Worker(Channel<PlayOption> channel)
        {
            while (true)
            {
                var playOption = await channel.Reader.ReadAsync();
                await PlayAudioAsync(playOption);
            }
        }

        internal async Task PlayAudioAsync(PlayOption option)
        {
            try
            {
                Logger.LogInformation("Playing audio file: {FilePath} at volume: {Volume}", option.FilePath, option.Volume);
                await Ci_AudioService.PlayAudioAsync(option.FilePath, option.Volume, option.Cts);
                Logger.LogInformation("Finished playing audio file: {FilePath}", option.FilePath);
                option.Completion.SetResult(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error playing audio file: {FilePath}", option.FilePath);
                option.Completion.SetException(ex);
            }
        }
    }
}
