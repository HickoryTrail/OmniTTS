using OmniTTS.Shared;

namespace OmniTTS.Plugin.Models
{
    internal class RequestOption
    {
        internal required Provider Provider { get; set; }
        internal required string Model { get; set; }
        internal required string Voice { get; set; }
        internal required string Text { get; set; }
        internal required float Speed { get; set; }
        internal required string FilePath { get; set; }
        internal required CancellationToken Cts {  get; set; }
        internal TaskCompletionSource<bool> Completion { get; set; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
