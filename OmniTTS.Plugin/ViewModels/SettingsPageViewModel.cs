using ClassIsland.Shared;
using Microsoft.Extensions.Logging;
using OmniTTS.Plugin.Services;
using OmniTTS.Shared;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace OmniTTS.Plugin.ViewModels
{
    internal class SettingsPageViewModel : ReactiveObject
    {
        private SettingsService SettingsService { get; set; }
        private ILogger<SettingsPageViewModel> Logger { get; set; }
        public SettingsPageViewModel()
        {
            Logger = IAppHost.GetService<ILogger<SettingsPageViewModel>>();
            SettingsService = IAppHost.GetService<SettingsService>();
            Providers = new()
            {
                new(Provider.None, "不指定"),
                new(Provider.OpenAI, "OpenAI"),
                new(Provider.Gemini, "Gemini"),
                new(Provider.FishAudio, "FishAudio"),
                new(Provider.Elevenlabs, "ElevenLabs"),
                new(Provider.MiniMax, "MiniMax"),
                new(Provider.MiMo, "MiMo")
            };
            SelectedProviderPair = Providers.FirstOrDefault(x => x.Key == SettingsService.Setting.DefaultProvider);
            MaxConcurrentRequests = SettingsService.Setting.MaxConcurrentRequests;
            SettingsService.Setting.ProviderSetting.WhenAnyValue(
                x => x.OpenAISetting.IsEnabled,
                x => x.GeminiSetting.IsEnabled,
                x => x.FishAudioSetting.IsEnabled,
                x => x.ElevenLabsSetting.IsEnabled,
                x => x.MiMoSetting.IsEnabled,
                x => x.MiniMaxSetting.IsEnabled).Subscribe(_ =>
                {
                    CheckCurrentProvider();
                });
        }
        private ObservableCollection<KeyValuePair<Provider, string>> _providers = new();
        public ObservableCollection<KeyValuePair<Provider,string>> Providers
        {
            get => _providers;
            set => this.RaiseAndSetIfChanged(ref _providers, value);
        }
        private KeyValuePair<Provider, string> _selectedProviderPair = new();
        public KeyValuePair<Provider, string> SelectedProviderPair
        {
            get => _selectedProviderPair;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedProviderPair, value);
                SelectedProvider = value.Key;
            }
        }
        private Provider _selectedProvider = Provider.None;
        public Provider SelectedProvider
        {
            get => _selectedProvider;
            set 
            {
                this.RaiseAndSetIfChanged(ref _selectedProvider, value);
                SettingsService.Setting.DefaultProvider = value;
            }
        }
        internal int _maxConcurrentRequests = 5;
        internal int MaxConcurrentRequests
        {
            get => _maxConcurrentRequests;
            set
            {
                this.RaiseAndSetIfChanged(ref _maxConcurrentRequests, value);
                SettingsService.Setting.MaxConcurrentRequests = value;
            }
        }

        private void CheckCurrentProvider()
        {
            switch (SelectedProvider)
            {
                case Provider.OpenAI:
                    if (!SettingsService.Setting.ProviderSetting.OpenAISetting.IsEnabled)
                    {
                        SelectedProviderPair = Providers.FirstOrDefault(x => x.Key == Provider.None);
                    }
                    break;
                case Provider.Gemini:
                    if (!SettingsService.Setting.ProviderSetting.GeminiSetting.IsEnabled)
                    {
                        SelectedProviderPair = Providers.FirstOrDefault(x => x.Key == Provider.None);
                    }
                    break;
                case Provider.FishAudio:
                    if (!SettingsService.Setting.ProviderSetting.FishAudioSetting.IsEnabled)
                    {
                        SelectedProviderPair = Providers.FirstOrDefault(x => x.Key == Provider.None);
                    }
                    break;
                case Provider.Elevenlabs:
                    if (!SettingsService.Setting.ProviderSetting.ElevenLabsSetting.IsEnabled)
                    {
                        SelectedProviderPair = Providers.FirstOrDefault(x => x.Key == Provider.None);
                    }
                    break;
                case Provider.MiniMax:
                    if (!SettingsService.Setting.ProviderSetting.MiniMaxSetting.IsEnabled)
                    {
                        SelectedProviderPair = Providers.FirstOrDefault(x => x.Key == Provider.None);
                    }
                    break;
                case Provider.MiMo:
                    if (!SettingsService.Setting.ProviderSetting.MiMoSetting.IsEnabled)
                    {
                        SelectedProviderPair = Providers.FirstOrDefault(x => x.Key == Provider.None);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}