using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    class Empty<T> : IAsyncObservable<T>
    {
        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
        {
            await observer.DisposeAsync().ConfigureAwait(false);
        }
    }
}
