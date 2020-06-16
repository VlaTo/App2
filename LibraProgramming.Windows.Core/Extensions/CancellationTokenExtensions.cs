using System.Threading;
using System.Threading.Tasks;

namespace LibraProgramming.Windows.Core.Extensions
{
    internal static class CancellationTokenExtensions
    {
        public static async Task AsTask(this CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();

            using (cancellationToken.Register(() => tcs.SetResult(true)))
            {
                await tcs.Task;
            }
        }
    }
}