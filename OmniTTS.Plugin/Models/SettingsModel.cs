using Avalonia.Xaml.Interactions.Custom;
using OmniTTS.Shared;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniTTS.Plugin.Models
{
    internal class SettingsModel : ReactiveObject
    {
        // 设置实例
        internal Provider _defaultProvider = Provider.None;
        internal Provider DefaultProvider
        {
            get => _defaultProvider;
            set => this.RaiseAndSetIfChanged(ref _defaultProvider, value);
        }
        internal int _maxConcurrentRequests = 5;
        internal int MaxConcurrentRequests
        {
            get => _maxConcurrentRequests;
            set => this.RaiseAndSetIfChanged(ref _maxConcurrentRequests, value);
        }
        internal ProviderSettings _providerSetting = new ProviderSettings();
        internal ProviderSettings ProviderSetting
        {
            get => _providerSetting;
            set => this.RaiseAndSetIfChanged(ref _providerSetting, value);
        }

        //类定义
        internal class ProviderSettings : ReactiveObject
        {
            // Setting 实例
            internal OpenAISettings? _openAISetting = new();
            internal OpenAISettings? OpenAISetting
            {
                get => _openAISetting;
                set => this.RaiseAndSetIfChanged(ref _openAISetting, value);
            }
            internal FishAudioSettings? _fishAudioSetting = new();
            internal FishAudioSettings? FishAudioSetting
            {
                get => _fishAudioSetting;
                set => this.RaiseAndSetIfChanged(ref _fishAudioSetting, value);
            }
            internal ElevenLabsSettings? _elevenLabsSetting = new();
            internal ElevenLabsSettings? ElevenLabsSetting
            {
                get => _elevenLabsSetting;
                set => this.RaiseAndSetIfChanged(ref _elevenLabsSetting, value);
            }
            internal GeminiSettings? _geminiSetting = new();
            internal GeminiSettings? GeminiSetting
            {
                get => _geminiSetting;
                set => this.RaiseAndSetIfChanged(ref _geminiSetting, value);
            }
            internal MiniMaxSettings? _miniMaxSetting = new();
            internal MiniMaxSettings? MiniMaxSetting
            {
                get => _miniMaxSetting;
                set => this.RaiseAndSetIfChanged(ref _miniMaxSetting, value);
            }
            internal MiMoSettings? _miMoSetting = new();
            internal MiMoSettings? MiMoSetting
            {
                get => _miMoSetting;
                set => this.RaiseAndSetIfChanged(ref _miMoSetting, value);
            }

            // 类定义
            internal class OpenAISettings : ReactiveObject
            {
                internal bool _isEnabled = false;
                internal bool IsEnabled
                {
                    get => _isEnabled;
                    set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
                }
                internal string _baseUrl = "https://api.openai.com";
                internal string BaseUrl
                {
                    get => _baseUrl;
                    set => this.RaiseAndSetIfChanged(ref _baseUrl, value);
                }
                internal string _apiKey = "";
                internal string ApiKey
                {
                    get => _apiKey;
                    set => this.RaiseAndSetIfChanged(ref _apiKey, value);
                }
                internal string _model = "gpt-4o-mini-tts";
                internal string Model
                {
                    get => _model;
                    set => this.RaiseAndSetIfChanged(ref _model, value);
                }
                internal string _voice = "alloy";
                internal string Voice
                {
                    get => _voice;
                    set => this.RaiseAndSetIfChanged(ref _voice, value);
                }
                internal float _speed = 1.0f;
                internal float Speed
                {
                    get => _speed;
                    set => this.RaiseAndSetIfChanged(ref _speed, value);
                }
                internal float _volume = 1.0f;
                internal float Volume
                {
                    get => _volume;
                    set => this.RaiseAndSetIfChanged(ref _volume, value);
                }
            }
            internal class FishAudioSettings : ReactiveObject
            {
                internal bool _isEnabled = false;
                internal bool IsEnabled
                {
                    get => _isEnabled;
                    set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
                }
                internal string _baseUrl = "https://api.fish.audio";
                internal string BaseUrl
                {
                    get => _baseUrl;
                    set => this.RaiseAndSetIfChanged(ref _baseUrl, value);
                }
                internal string _apiKey = "";
                internal string ApiKey
                {
                    get => _apiKey;
                    set => this.RaiseAndSetIfChanged(ref _apiKey, value);
                }
                internal string _model = "s2-pro";
                internal string Model
                {
                    get => _model;
                    set => this.RaiseAndSetIfChanged(ref _model, value);
                }
                internal string _voice = "8ef4a238714b45718ce04243307c57a7";
                internal string Voice
                {
                    get => _voice;
                    set => this.RaiseAndSetIfChanged(ref _voice, value);
                }
                internal float _speed = 1.0f;
                internal float Speed
                {
                    get => _speed;
                    set => this.RaiseAndSetIfChanged(ref _speed, value);
                }
                internal float _volume = 1.0f;
                internal float Volume
                {
                    get => _volume;
                    set => this.RaiseAndSetIfChanged(ref _volume, value);
                }
            }
            internal class ElevenLabsSettings : ReactiveObject
            {
                internal bool _isEnabled = false;
                internal bool IsEnabled
                {
                    get => _isEnabled;
                    set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
                }
                internal string _baseUrl = "https://api.elevenlabs.io";
                internal string BaseUrl
                {
                    get => _baseUrl;
                    set => this.RaiseAndSetIfChanged(ref _baseUrl, value);
                }
                internal string _apiKey = "";
                internal string ApiKey
                {
                    get => _apiKey;
                    set => this.RaiseAndSetIfChanged(ref _apiKey, value);
                }
                internal string _model = "eleven_multilingual_v2";
                internal string Model
                {
                    get => _model;
                    set => this.RaiseAndSetIfChanged(ref _model, value);
                }
                internal string _voice = "JBFqnCBsd6RMkjVDRZzb";
                internal string Voice
                {
                    get => _voice;
                    set => this.RaiseAndSetIfChanged(ref _voice, value);
                }
                internal float _speed = 1.0f;
                internal float Speed
                {
                    get => _speed;
                    set => this.RaiseAndSetIfChanged(ref _speed, value);
                }
                internal float _volume = 1.0f;
                internal float Volume
                {
                    get => _volume;
                    set => this.RaiseAndSetIfChanged(ref _volume, value);
                }
            }
            internal class GeminiSettings : ReactiveObject
            {
                internal bool _isEnabled = false;
                internal bool IsEnabled
                {
                    get => _isEnabled;
                    set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
                }
                internal string _baseUrl = "https://generativelanguage.googleapis.com";
                internal string BaseUrl
                {
                    get => _baseUrl;
                    set => this.RaiseAndSetIfChanged(ref _baseUrl, value);
                }
                internal string _apiKey = "";
                internal string ApiKey
                {
                    get => _apiKey;
                    set => this.RaiseAndSetIfChanged(ref _apiKey, value);
                }
                internal string _model = "gemini-3.1-flash-tts-preview";
                internal string Model
                {
                    get => _model;
                    set => this.RaiseAndSetIfChanged(ref _model, value);
                }
                internal string _voice = "Kore";
                internal string Voice
                {
                    get => _voice;
                    set => this.RaiseAndSetIfChanged(ref _voice, value);
                }
                internal float _speed = 1.0f;
                internal float Speed
                {
                    get => _speed;
                    set => this.RaiseAndSetIfChanged(ref _speed, value);
                }
                internal float _volume = 1.0f;
                internal float Volume
                {
                    get => _volume;
                    set => this.RaiseAndSetIfChanged(ref _volume, value);
                }
            }
            internal class MiniMaxSettings : ReactiveObject
            {
                internal bool _isEnabled = false;
                internal bool IsEnabled
                {
                    get => _isEnabled;
                    set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
                }
                internal string _baseUrl = "https://api.minimax.io";
                internal string BaseUrl
                {
                    get => _baseUrl;
                    set => this.RaiseAndSetIfChanged(ref _baseUrl, value);
                }
                internal string _apiKey = "";
                internal string ApiKey
                {
                    get => _apiKey;
                    set => this.RaiseAndSetIfChanged(ref _apiKey, value);
                }
                internal string _model = "speech-2.8-hd";
                internal string Model
                {
                    get => _model;
                    set => this.RaiseAndSetIfChanged(ref _model, value);
                }
                internal string _voice = "English_expressive_narrator";
                internal string Voice
                {
                    get => _voice;
                    set => this.RaiseAndSetIfChanged(ref _voice, value);
                }
                internal float _speed = 1.0f;
                internal float Speed
                {
                    get => _speed;
                    set => this.RaiseAndSetIfChanged(ref _speed, value);
                }
                internal float _volume = 1.0f;
                internal float Volume
                {
                    get => _volume;
                    set => this.RaiseAndSetIfChanged(ref _volume, value);
                }
            }
            internal class MiMoSettings : ReactiveObject
            {
                internal bool _isEnabled = false;
                internal bool IsEnabled
                {
                    get => _isEnabled;
                    set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
                }
                internal string _baseUrl = "https://api.xiaomimimo.com";
                internal string BaseUrl
                {
                    get => _baseUrl;
                    set => this.RaiseAndSetIfChanged(ref _baseUrl, value);
                }
                internal string _apiKey = "";
                internal string ApiKey
                {
                    get => _apiKey;
                    set => this.RaiseAndSetIfChanged(ref _apiKey, value);
                }
                internal string _model = "mimo-v2.5-tts";
                internal string Model
                {
                    get => _model;
                    set => this.RaiseAndSetIfChanged(ref _model, value);
                }
                internal string _voice = "mimo_default";
                internal string Voice
                {
                    get => _voice;
                    set => this.RaiseAndSetIfChanged(ref _voice, value);
                }
                internal float _speed = 1.0f;
                internal float Speed
                {
                    get => _speed;
                    set => this.RaiseAndSetIfChanged(ref _speed, value);
                }
                internal float _volume = 1.0f;
                internal float Volume
                {
                    get => _volume;
                    set => this.RaiseAndSetIfChanged(ref _volume, value);
                }
            }
        }
    }


}
