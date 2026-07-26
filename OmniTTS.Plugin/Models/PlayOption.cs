using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniTTS.Plugin.Models
{
    internal class PlayOption
    {
        internal required string FilePath { get; set; }
        internal required float Volume { get; set; }
        internal required CancellationToken Cts { get; set; }
        internal TaskCompletionSource<bool> Completion { get; set; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
