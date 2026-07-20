namespace OmniTTS.Shared;

public class TtsOption
{
    public Provider? Provider { get; set; }
    public string? Model { get; set; }
    public string? Voice { get; set; }
    public required string Text { get; set; }
    public float? Speed { get; set; } = 1.0f;
    public float? Volume { get; set; } = 1.0f;
}
