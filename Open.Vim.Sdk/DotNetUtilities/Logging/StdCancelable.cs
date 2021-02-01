using System.Threading;

namespace Vim.DotNetUtilities.Logging
{
    public class StdCancelable
    {
        readonly CancellationTokenSource _cancel
            = new CancellationTokenSource();

        public void Cancel()
            => _cancel.Cancel();

        public virtual bool IsCancelRequested()
            => _cancel.IsCancellationRequested;
    }
}
