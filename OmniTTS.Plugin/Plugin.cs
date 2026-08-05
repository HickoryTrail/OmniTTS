using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Abstractions.Services.SpeechService;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OmniTTS.Plugin.Services;
using OmniTTS.Shared;

namespace OmniTTS.Plugin
{
    [PluginEntrance]
    public class Plugin : PluginBase
    {
        public override void Initialize(HostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton<SettingsService>();
            services.AddSingleton<GenerationService>();
            services.AddSingleton<AudioService>();
            services.AddSingleton<IOmniTTS, OmniTTService>();
            services.AddSpeechProvider<OmniSpeechService>();
            services.AddSettingsPage<OmniTTSettingsPage>();
            AppBase.Current.AppStarted += async (_, _) =>
            {
                var settingsService = IAppHost.GetService<SettingsService>();
                await settingsService.InitializeAsync(PluginConfigFolder);
                var OmniTTService = IAppHost.GetService<IOmniTTS>() as OmniTTService;
                OmniTTService?.Initialize();
                var speechService = IAppHost.GetService<ISpeechService>() as OmniSpeechService;
                speechService?.Initialize();
            };
        }
    }
}
