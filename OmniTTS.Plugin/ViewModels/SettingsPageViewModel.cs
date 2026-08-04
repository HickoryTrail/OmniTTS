using ClassIsland.Shared;
using Microsoft.Extensions.Logging;
using OmniTTS.Plugin.Services;
using OmniTTS.Shared;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

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
    }
}
