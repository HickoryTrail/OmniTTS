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

    internal void LoadContext()
    {
        var settings = IAppHost.GetService<SettingsService>();
        var Logger = IAppHost.GetService<ILogger<ProviderEditorViewModel>>();
        switch (Provider)
        {
            case Provider.OpenAI:
                IsEnabled = settings.Setting.ProviderSetting.OpenAISetting!.IsEnabled;
                BaseUrl = settings.Setting.ProviderSetting.OpenAISetting.BaseUrl;
                ApiKey = settings.Setting.ProviderSetting.OpenAISetting.ApiKey;
                Model = settings.Setting.ProviderSetting.OpenAISetting.Model;
                VoiceId = settings.Setting.ProviderSetting.OpenAISetting.Voice;
                break;
            case Provider.FishAudio:
                IsEnabled = settings.Setting.ProviderSetting.FishAudioSetting!.IsEnabled;
                BaseUrl = settings.Setting.ProviderSetting.FishAudioSetting.BaseUrl;
                ApiKey = settings.Setting.ProviderSetting.FishAudioSetting.ApiKey;
                Model = settings.Setting.ProviderSetting.FishAudioSetting.Model;
                VoiceId = settings.Setting.ProviderSetting.FishAudioSetting.Voice;
                break;
            case Provider.Elevenlabs:
                IsEnabled = settings.Setting.ProviderSetting.ElevenLabsSetting!.IsEnabled;
                BaseUrl = settings.Setting.ProviderSetting.ElevenLabsSetting.BaseUrl;
                ApiKey = settings.Setting.ProviderSetting.ElevenLabsSetting.ApiKey;
                Model = settings.Setting.ProviderSetting.ElevenLabsSetting.Model;
                VoiceId = settings.Setting.ProviderSetting.ElevenLabsSetting.Voice;
                break;
            case Provider.Gemini:
                IsEnabled = settings.Setting.ProviderSetting.GeminiSetting!.IsEnabled;
                BaseUrl = settings.Setting.ProviderSetting.GeminiSetting.BaseUrl;
                ApiKey = settings.Setting.ProviderSetting.GeminiSetting.ApiKey;
                Model = settings.Setting.ProviderSetting.GeminiSetting.Model;
                VoiceId = settings.Setting.ProviderSetting.GeminiSetting.Voice;
                break;
            case Provider.MiniMax:
                IsEnabled = settings.Setting.ProviderSetting.MiniMaxSetting!.IsEnabled;
                BaseUrl = settings.Setting.ProviderSetting.MiniMaxSetting.BaseUrl;
                ApiKey = settings.Setting.ProviderSetting.MiniMaxSetting.ApiKey;
                Model = settings.Setting.ProviderSetting.MiniMaxSetting.Model;
                VoiceId = settings.Setting.ProviderSetting.MiniMaxSetting.Voice;
                break;
            case Provider.MiMo:
                IsEnabled = settings.Setting.ProviderSetting.MiMoSetting!.IsEnabled;
                BaseUrl = settings.Setting.ProviderSetting.MiMoSetting.BaseUrl;
                ApiKey = settings.Setting.ProviderSetting.MiMoSetting.ApiKey;
                Model = settings.Setting.ProviderSetting.MiMoSetting.Model;
                VoiceId = settings.Setting.ProviderSetting.MiMoSetting.Voice;
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
                break;
            case Provider.FishAudio:
                settings.Setting.ProviderSetting.FishAudioSetting!.IsEnabled = IsEnabled;
                settings.Setting.ProviderSetting.FishAudioSetting.BaseUrl = BaseUrl;
                settings.Setting.ProviderSetting.FishAudioSetting.ApiKey = ApiKey;
                settings.Setting.ProviderSetting.FishAudioSetting.Model = Model;
                settings.Setting.ProviderSetting.FishAudioSetting.Voice = VoiceId;
                break;
            case Provider.Elevenlabs:
                settings.Setting.ProviderSetting.ElevenLabsSetting!.IsEnabled = IsEnabled;
                settings.Setting.ProviderSetting.ElevenLabsSetting.BaseUrl = BaseUrl;
                settings.Setting.ProviderSetting.ElevenLabsSetting.ApiKey = ApiKey;
                settings.Setting.ProviderSetting.ElevenLabsSetting.Model = Model;
                settings.Setting.ProviderSetting.ElevenLabsSetting.Voice = VoiceId;
                break;
            case Provider.Gemini:
                settings.Setting.ProviderSetting.GeminiSetting!.IsEnabled = IsEnabled;
                settings.Setting.ProviderSetting.GeminiSetting.BaseUrl = BaseUrl;
                settings.Setting.ProviderSetting.GeminiSetting.ApiKey = ApiKey;
                settings.Setting.ProviderSetting.GeminiSetting.Model = Model;
                settings.Setting.ProviderSetting.GeminiSetting.Voice = VoiceId;
                break;
            case Provider.MiniMax:
                settings.Setting.ProviderSetting.MiniMaxSetting!.IsEnabled = IsEnabled;
                settings.Setting.ProviderSetting.MiniMaxSetting.BaseUrl = BaseUrl;
                settings.Setting.ProviderSetting.MiniMaxSetting.ApiKey = ApiKey;
                settings.Setting.ProviderSetting.MiniMaxSetting.Model = Model;
                settings.Setting.ProviderSetting.MiniMaxSetting.Voice = VoiceId;
                break;
            case Provider.MiMo:
                settings.Setting.ProviderSetting.MiMoSetting!.IsEnabled = IsEnabled;
                settings.Setting.ProviderSetting.MiMoSetting.BaseUrl = BaseUrl;
                settings.Setting.ProviderSetting.MiMoSetting.ApiKey = ApiKey;
                settings.Setting.ProviderSetting.MiMoSetting.Model = Model;
                settings.Setting.ProviderSetting.MiMoSetting.Voice = VoiceId;
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
