using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    class DropOnBackpressure<T> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<T> _source;

        public DropOnBackpressure(IAsyncObservable<T> source)
        {
            _source = source;
        }

        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
        {
            var o = new Observer(observer);
            await _source.SubscribeAsync(o, token);
            await o.Current;
        }

        class Observer : IAsyncObserver<T>
        {
            readonly IAsyncObserver<T> _observer;

            public Task<bool> Current { get; set; } = Task.FromResult(true);

            public Observer(IAsyncObserver<T> observer)
            {
                _observer = observer;
            }

            public async ValueTask<bool> OnNextAsync(T value)
            {
                if (Current.IsCompleted)
                {
                    if (!await Current)
                        return false;

                    Current = OnNextCore(value);
                }

                return true;
            }

            public async ValueTask DisposeAsync()
            {
                try
                {
                    await Current.ConfigureAwait(false);
                }
                finally
                {
                    await _observer.DisposeAsync().ConfigureAwait(false);
                }
            }

            private async Task<bool> OnNextCore(T value)
            {
                return await _observer.OnNextAsync(value).ConfigureAwait(false);
            }
        }
    }
}
