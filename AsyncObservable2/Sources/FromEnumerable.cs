using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    class FromEnumerable<T> : IAsyncObservable<T>
    {
        readonly IEnumerable<T> _enumerable;

        public FromEnumerable(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
        }

        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
        {
            try
            {
                foreach (var item in _enumerable)
                {
                    token.ThrowIfCancellationRequested();

                    var keep = await observer.OnNextAsync(item).ConfigureAwait(false);
                    if (!keep)
                    {
                        break;
                    }
                }
            }
            finally
            {
                await observer.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
