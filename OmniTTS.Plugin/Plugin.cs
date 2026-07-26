using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls;
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
            services.AddSingleton<IOmniTTS, OmniTTService>();
            AppBase.Current.AppStarted += async (_, _) =>
            {
                await CommonTaskDialogs.ShowDialog("Hello world!", "Hello from OmniTTS.Plugin!");
                var settingsService = IAppHost.GetService<SettingsService>();
                await settingsService.InitializeAsync(PluginConfigFolder);
            };
        }
    }
}