using ClassIsland.Shared;
using Microsoft.Extensions.Logging;
using OmniTTS.Plugin.Services;
using OmniTTS.Shared;
using ReactiveUI;

namespace OmniTTS.Plugin.ViewModels;

internal class ProviderEditorViewModel : ReactiveObject
{
    public ProviderEditorViewModel()
    {
        this.WhenAnyValue(
            x => x.BaseUrl,
            x => x.ApiKey,
            x => x.Model,
            x => x.VoiceId
        ).Subscribe(_ =>
        {
            IsConfigValid = IsValid();
            if (!IsConfigValid) IsEnabled = false;
        });
    }
    internal Provider Provider { get; set; } = Provider.None;

    private bool _isEnabled = false;
    public bool IsEnabled
    {
        get => _isEnabled;
        set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
    }

    private string _baseUrl = string.Empty;
    public string BaseUrl
    {
        get => _baseUrl;
        set => this.RaiseAndSetIfChanged(ref _baseUrl, value);
    }

    private string _apiKey = string.Empty;
    public string ApiKey
    {
        get => _apiKey;
        set => this.RaiseAndSetIfChanged(ref _apiKey, value);
    }

    private string _model = string.Empty;
    public string Model
    {
        get => _model;
        set => this.RaiseAndSetIfChanged(ref _model, value);
    }

    private string _voiceId = string.Empty;
    public string VoiceId
    {
        get => _voiceId;
        set => this.RaiseAndSetIfChanged(ref _voiceId, value);
    }
    private bool _isConfigValid = true;
    public bool IsConfigValid
    {
        get => _isConfigValid;
        set => this.RaiseAndSetIfChanged(ref _isConfigValid, value);
    }

    private float _volume = 1.0f;
    public float Volume
    {
        get => _volume;
        set => this.RaiseAndSetIfChanged(ref _volume, value);
    }
    private float _speed = 1.0f;
    public float Speed
    {
        get => _speed;
        set => this.RaiseAndSetIfChanged(ref _speed, value);
    }
    private bool _isSpeedAvailable = false;
    public bool IsSpeedAvailable
    {
        get => _isSpeedAvailable;
        set => this.RaiseAndSetIfChanged(ref _isSpeedAvailable, value);
    }
    private float _minSpeed = 0.5f;
    public float MinSpeed
    {
        get => _minSpeed;
        set => this.RaiseAndSetIfChanged(ref _minSpeed, value);
    }
    private float _maxSpeed = 2.0f;
    public float MaxSpeed
    {
        get => _maxSpeed; 
        set => this.RaiseAndSetIfChanged(ref _maxSpeed, value);
    }

    internal void LoadContext()
    {
        var settings = IAppHost.GetService<SettingsService>();
        var Logger = IAppHost.GetService<ILogger<ProviderEditorViewModel>>();
        switch (Provider)
        {
            case Provider.OpenAI:
                BaseUrl = settings.Setting.ProviderSetting.OpenAISetting.BaseUrl;
                ApiKey = settings.Setting.ProviderSetting.OpenAISetting.ApiKey;
                Model = settings.Setting.ProviderSetting.OpenAISetting.Model;
                VoiceId = settings.Setting.ProviderSetting.OpenAISetting.Voice;
                Speed = settings.Setting.ProviderSetting.OpenAISetting.Speed;
                Volume = settings.Setting.ProviderSetting.OpenAISetting.Volume;
                IsSpeedAvailable = true;
                MinSpeed = 0.25f;
                MaxSpeed = 4.0f;
                IsEnabled = settings.Setting.ProviderSetting.OpenAISetting!.IsEnabled;
                break;
            case Provider.FishAudio:
                BaseUrl = settings.Setting.ProviderSetting.FishAudioSetting.BaseUrl;
                ApiKey = settings.Setting.ProviderSetting.FishAudioSetting.ApiKey;
                Model = settings.Setting.ProviderSetting.FishAudioSetting.Model;
                VoiceId = settings.Setting.ProviderSetting.FishAudioSetting.Voice;
                Speed = settings.Setting.ProviderSetting.FishAudioSetting.Speed;
                Volume = settings.Setting.ProviderSetting.FishAudioSetting.Volume;
                IsSpeedAvailable = true;
                MinSpeed = 0.5f;
                MaxSpeed = 2.0f;
                IsEnabled = settings.Setting.ProviderSetting.FishAudioSetting!.IsEnabled;
                break;
            case Provider.Elevenlabs:
                BaseUrl = settings.Setting.ProviderSetting.ElevenLabsSetting.BaseUrl;
                ApiKey = settings.Setting.ProviderSetting.ElevenLabsSetting.ApiKey;
                Model = settings.Setting.ProviderSetting.ElevenLabsSetting.Model;
                VoiceId = settings.Setting.ProviderSetting.ElevenLabsSetting.Voice;
                Speed = settings.Setting.ProviderSetting.ElevenLabsSetting.Speed;
                Volume = settings.Setting.ProviderSetting.ElevenLabsSetting.Volume;
                IsSpeedAvailable = true;
                MinSpeed = 0.7f;
                MaxSpeed = 1.2f;
                IsEnabled = settings.Setting.ProviderSetting.ElevenLabsSetting!.IsEnabled;
                break;
            case Provider.Gemini:
                BaseUrl = settings.Setting.ProviderSetting.GeminiSetting.BaseUrl;
                ApiKey = settings.Setting.ProviderSetting.GeminiSetting.ApiKey;
                Model = settings.Setting.ProviderSetting.GeminiSetting.Model;
                VoiceId = settings.Setting.ProviderSetting.GeminiSetting.Voice;
                Speed = settings.Setting.ProviderSetting.GeminiSetting.Speed;
                Volume = settings.Setting.ProviderSetting.GeminiSetting.Volume;
                IsSpeedAvailable = true;
                MinSpeed = 0.25f;
                MaxSpeed = 4.0f;
                IsEnabled = settings.Setting.ProviderSetting.GeminiSetting!.IsEnabled;
                break;
            case Provider.MiniMax:
                BaseUrl = settings.Setting.ProviderSetting.MiniMaxSetting.BaseUrl;
                ApiKey = settings.Setting.ProviderSetting.MiniMaxSetting.ApiKey;
                Model = settings.Setting.ProviderSetting.MiniMaxSetting.Model;
                VoiceId = settings.Setting.ProviderSetting.MiniMaxSetting.Voice;
                Speed = settings.Setting.ProviderSetting.MiniMaxSetting.Speed;
                Volume = settings.Setting.ProviderSetting.MiniMaxSetting.Volume;
                IsSpeedAvailable = true;
                MinSpeed = 0.5f;
                MaxSpeed = 2.0f;
                IsEnabled = settings.Setting.ProviderSetting.MiniMaxSetting!.IsEnabled;
                break;
            case Provider.MiMo:
                BaseUrl = settings.Setting.ProviderSetting.MiMoSetting.BaseUrl;
                ApiKey = settings.Setting.ProviderSetting.MiMoSetting.ApiKey;
                Model = settings.Setting.ProviderSetting.MiMoSetting.Model;
                VoiceId = settings.Setting.ProviderSetting.MiMoSetting.Voice;
                Volume = settings.Setting.ProviderSetting.MiMoSetting.Volume;
                IsSpeedAvailable = false;
                IsEnabled = settings.Setting.ProviderSetting.MiMoSetting!.IsEnabled;
                break;
            default:
                Logger.LogError("Invalid provider: {Provider}", Provider);
                throw new ArgumentOutOfRangeException(nameof(Provider), Provider, "A TTS provider is required.");
        }
    }

    internal void SaveContext()
    {
        var settings = IAppHost.GetService<SettingsService>();
        var Logger = IAppHost.GetService<ILogger<ProviderEditorViewModel>>();
        switch (Provider)
        {
            case Provider.OpenAI:
                settings.Setting.ProviderSetting.OpenAISetting!.IsEnabled = IsEnabled;
                settings.Setting.ProviderSetting.OpenAISetting.BaseUrl = BaseUrl;
                settings.Setting.ProviderSetting.OpenAISetting.ApiKey = ApiKey;
                settings.Setting.ProviderSetting.OpenAISetting.Model = Model;
                settings.Setting.ProviderSetting.OpenAISetting.Voice = VoiceId;
                settings.Setting.ProviderSetting.OpenAISetting.Speed = Speed;
                settings.Setting.ProviderSetting.OpenAISetting.Volume = Volume;
                break;
            case Provider.FishAudio:
                settings.Setting.ProviderSetting.FishAudioSetting!.IsEnabled = IsEnabled;
                settings.Setting.ProviderSetting.FishAudioSetting.BaseUrl = BaseUrl;
                settings.Setting.ProviderSetting.FishAudioSetting.ApiKey = ApiKey;
                settings.Setting.ProviderSetting.FishAudioSetting.Model = Model;
                settings.Setting.ProviderSetting.FishAudioSetting.Voice = VoiceId;
                settings.Setting.ProviderSetting.FishAudioSetting.Speed = Speed;
                settings.Setting.ProviderSetting.FishAudioSetting.Volume = Volume;
                break;
            case Provider.Elevenlabs:
                settings.Setting.ProviderSetting.ElevenLabsSetting!.IsEnabled = IsEnabled;
                settings.Setting.ProviderSetting.ElevenLabsSetting.BaseUrl = BaseUrl;
                settings.Setting.ProviderSetting.ElevenLabsSetting.ApiKey = ApiKey;
                settings.Setting.ProviderSetting.ElevenLabsSetting.Model = Model;
                settings.Setting.ProviderSetting.ElevenLabsSetting.Voice = VoiceId;
                settings.Setting.ProviderSetting.ElevenLabsSetting.Speed = Speed;
                settings.Setting.ProviderSetting.ElevenLabsSetting.Volume = Volume;
                break;
            case Provider.Gemini:
                settings.Setting.ProviderSetting.GeminiSetting!.IsEnabled = IsEnabled;
                settings.Setting.ProviderSetting.GeminiSetting.BaseUrl = BaseUrl;
                settings.Setting.ProviderSetting.GeminiSetting.ApiKey = ApiKey;
                settings.Setting.ProviderSetting.GeminiSetting.Model = Model;
                settings.Setting.ProviderSetting.GeminiSetting.Voice = VoiceId;
                settings.Setting.ProviderSetting.GeminiSetting.Speed = Speed;
                settings.Setting.ProviderSetting.GeminiSetting.Volume = Volume;
                break;
            case Provider.MiniMax:
                settings.Setting.ProviderSetting.MiniMaxSetting!.IsEnabled = IsEnabled;
                settings.Setting.ProviderSetting.MiniMaxSetting.BaseUrl = BaseUrl;
                settings.Setting.ProviderSetting.MiniMaxSetting.ApiKey = ApiKey;
                settings.Setting.ProviderSetting.MiniMaxSetting.Model = Model;
                settings.Setting.ProviderSetting.MiniMaxSetting.Voice = VoiceId;
                settings.Setting.ProviderSetting.MiniMaxSetting.Speed = Speed;
                settings.Setting.ProviderSetting.MiniMaxSetting.Volume = Volume;
                break;
            case Provider.MiMo:
                settings.Setting.ProviderSetting.MiMoSetting!.IsEnabled = IsEnabled;
                settings.Setting.ProviderSetting.MiMoSetting.BaseUrl = BaseUrl;
                settings.Setting.ProviderSetting.MiMoSetting.ApiKey = ApiKey;
                settings.Setting.ProviderSetting.MiMoSetting.Model = Model;
                settings.Setting.ProviderSetting.MiMoSetting.Voice = VoiceId;
                settings.Setting.ProviderSetting.MiMoSetting.Volume = Volume;
                break;
            default:
                Logger.LogError("Invalid provider: {Provider}", Provider);
                throw new ArgumentOutOfRangeException(nameof(Provider), Provider, "A TTS provider is required.");
        }
    }

    internal bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
            return false;
        if (string.IsNullOrWhiteSpace(ApiKey))
            return false;
        if (string.IsNullOrWhiteSpace(Model))
            return false;
        if (string.IsNullOrWhiteSpace(VoiceId))
            return false;
        return true;
    }

}
