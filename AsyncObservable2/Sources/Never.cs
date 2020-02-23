using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    class Never<T> : IAsyncObservable<T>
    {
        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
        {
            try
            {
                await Task.Delay(-1, token);
            }
            finally
            {
                await observer.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
