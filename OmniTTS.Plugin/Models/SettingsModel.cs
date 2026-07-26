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
            internal OpenAISettings? _openAISetting;
            internal OpenAISettings? OpenAISetting
            {
                get => _openAISetting;
                set => this.RaiseAndSetIfChanged(ref _openAISetting, value);
            }
            internal FishAudioSettings? _fishAudioSetting;
            internal FishAudioSettings? FishAudioSetting
            {
                get => _fishAudioSetting;
                set => this.RaiseAndSetIfChanged(ref _fishAudioSetting, value);
            }
            internal ElevenLabsSettings? _elevenLabsSetting;
            internal ElevenLabsSettings? ElevenLabsSetting
            {
                get => _elevenLabsSetting;
                set => this.RaiseAndSetIfChanged(ref _elevenLabsSetting, value);
            }
            internal GeminiSettings? _geminiSetting;
            internal GeminiSettings? GeminiSetting
            {
                get => _geminiSetting;
                set => this.RaiseAndSetIfChanged(ref _geminiSetting, value);
            }
            internal MiniMaxSettings? _miniMaxSetting;
            internal MiniMaxSettings? MiniMaxSetting
            {
                get => _miniMaxSetting;
                set => this.RaiseAndSetIfChanged(ref _miniMaxSetting, value);
            }

            // 类定义
            internal class OpenAISettings : ReactiveObject
            {
                internal required bool _isEnabled = false;
                internal required bool IsEnabled
                {
                    get => _isEnabled;
                    set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
                }
                internal required string _baseUrl = "https://api.openai.com";
                internal required string BaseUrl
                {
                    get => _baseUrl;
                    set => this.RaiseAndSetIfChanged(ref _baseUrl, value);
                }
                internal required string _apiKey = "";
                internal required string ApiKey
                {
                    get => _apiKey;
                    set => this.RaiseAndSetIfChanged(ref _apiKey, value);
                }
                internal required string _model = "gpt-4o-mini-tts";
                internal required string Model
                {
                    get => _model;
                    set => this.RaiseAndSetIfChanged(ref _model, value);
                }
                internal required string _voice = "alloy";
                internal required string Voice
                {
                    get => _voice;
                    set => this.RaiseAndSetIfChanged(ref _voice, value);
                }
                internal required float _speed = 1.0f;
                internal required float Speed
                {
                    get => _speed;
                    set => this.RaiseAndSetIfChanged(ref _speed, value);
                }
                internal required float _volume = 1.0f;
                internal required float Volume
                {
                    get => _volume;
                    set => this.RaiseAndSetIfChanged(ref _volume, value);
                }
            }
            internal class FishAudioSettings : ReactiveObject
            {
                internal required bool _isEnabled = false;
                internal required bool IsEnabled
                {
                    get => _isEnabled;
                    set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
                }
                internal required string _baseUrl = "https://api.fish.audio";
                internal required string BaseUrl
                {
                    get => _baseUrl;
                    set => this.RaiseAndSetIfChanged(ref _baseUrl, value);
                }
                internal required string _apiKey = "";
                internal required string ApiKey
                {
                    get => _apiKey;
                    set => this.RaiseAndSetIfChanged(ref _apiKey, value);
                }
                internal required string _model = "s2-pro";
                internal required string Model
                {
                    get => _model;
                    set => this.RaiseAndSetIfChanged(ref _model, value);
                }
                internal required string _voice = "8ef4a238714b45718ce04243307c57a7";
                internal required string Voice
                {
                    get => _voice;
                    set => this.RaiseAndSetIfChanged(ref _voice, value);
                }
                internal required float _speed = 1.0f;
                internal required float Speed
                {
                    get => _speed;
                    set => this.RaiseAndSetIfChanged(ref _speed, value);
                }
                internal required float _volume = 1.0f;
                internal required float Volume
                {
                    get => _volume;
                    set => this.RaiseAndSetIfChanged(ref _volume, value);
                }
            }
            internal class ElevenLabsSettings : ReactiveObject
            {
                internal required bool _isEnabled = false;
                internal required bool IsEnabled
                {
                    get => _isEnabled;
                    set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
                }
                internal required string _baseUrl = "https://api.elevenlabs.io";
                internal required string BaseUrl
                {
                    get => _baseUrl;
                    set => this.RaiseAndSetIfChanged(ref _baseUrl, value);
                }
                internal required string _apiKey = "";
                internal required string ApiKey
                {
                    get => _apiKey;
                    set => this.RaiseAndSetIfChanged(ref _apiKey, value);
                }
                internal required string _model = "eleven_multilingual_v2";
                internal required string Model
                {
                    get => _model;
                    set => this.RaiseAndSetIfChanged(ref _model, value);
                }
                internal required string _voice = "JBFqnCBsd6RMkjVDRZzb";
                internal required string Voice
                {
                    get => _voice;
                    set => this.RaiseAndSetIfChanged(ref _voice, value);
                }
                internal required float _speed = 1.0f;
                internal required float Speed
                {
                    get => _speed;
                    set => this.RaiseAndSetIfChanged(ref _speed, value);
                }
                internal required float _volume = 1.0f;
                internal required float Volume
                {
                    get => _volume;
                    set => this.RaiseAndSetIfChanged(ref _volume, value);
                }
            }
            internal class GeminiSettings : ReactiveObject
            {
                internal required bool _isEnabled = false;
                internal required bool IsEnabled
                {
                    get => _isEnabled;
                    set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
                }
                internal required string _baseUrl = "https://generativelanguage.googleapis.com";
                internal required string BaseUrl
                {
                    get => _baseUrl;
                    set => this.RaiseAndSetIfChanged(ref _baseUrl, value);
                }
                internal required string _apiKey = "";
                internal required string ApiKey
                {
                    get => _apiKey;
                    set => this.RaiseAndSetIfChanged(ref _apiKey, value);
                }
                internal required string _model = "gemini-3.1-flash-tts-preview";
                internal required string Model
                {
                    get => _model;
                    set => this.RaiseAndSetIfChanged(ref _model, value);
                }
                internal required string _voice = "Kore";
                internal required string Voice
                {
                    get => _voice;
                    set => this.RaiseAndSetIfChanged(ref _voice, value);
                }
                internal required float _speed = 1.0f;
                internal required float Speed
                {
                    get => _speed;
                    set => this.RaiseAndSetIfChanged(ref _speed, value);
                }
                internal required float _volume = 1.0f;
                internal required float Volume
                {
                    get => _volume;
                    set => this.RaiseAndSetIfChanged(ref _volume, value);
                }
            }
            internal class MiniMaxSettings : ReactiveObject
            {
                internal required bool _isEnabled = false;
                internal required bool IsEnabled
                {
                    get => _isEnabled;
                    set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
                }
                internal required string _baseUrl = "https://api.minimax.io";
                internal required string BaseUrl
                {
                    get => _baseUrl;
                    set => this.RaiseAndSetIfChanged(ref _baseUrl, value);
                }
                internal required string _apiKey = "";
                internal required string ApiKey
                {
                    get => _apiKey;
                    set => this.RaiseAndSetIfChanged(ref _apiKey, value);
                }
                internal required string _model = "speech-2.8-hd";
                internal required string Model
                {
                    get => _model;
                    set => this.RaiseAndSetIfChanged(ref _model, value);
                }
                internal required string _voice = "English_expressive_narrator";
                internal required string Voice
                {
                    get => _voice;
                    set => this.RaiseAndSetIfChanged(ref _voice, value);
                }
                internal required float _speed = 1.0f;
                internal required float Speed
                {
                    get => _speed;
                    set => this.RaiseAndSetIfChanged(ref _speed, value);
                }
                internal required float _volume = 1.0f;
                internal required float Volume
                {
                    get => _volume;
                    set => this.RaiseAndSetIfChanged(ref _volume, value);
                }
            }
        }
    }


}
