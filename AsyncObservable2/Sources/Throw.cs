using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    class Throw<T> : IAsyncObservable<T>
    {
        readonly Exception _exception;

        public Throw(Exception ex)
        {
            _exception = ex;
        }

        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
        {
            await observer.DisposeAsync().ConfigureAwait(false);
            throw _exception;
        }
    }
}
