using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    static class Defer<T>
    {
        public class Sync : IAsyncObservable<T>
        {
            readonly Func<IAsyncObservable<T>> _factory;

            public Sync(Func<IAsyncObservable<T>> factory)
            {
                _factory = factory;
            }

            public async ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
            {
                var obs = default(IAsyncObservable<T>);

                try
                {
                    obs = _factory();
                }
                catch
                {
                    await observer.DisposeAsync().ConfigureAwait(false);
                    throw;
                }

                await obs.SubscribeAsync(observer, token);
            }
        }

        public class Async : IAsyncObservable<T>
        {
            readonly Func<ValueTask<IAsyncObservable<T>>> _factory;

            public Async(Func<ValueTask<IAsyncObservable<T>>> factory)
            {
                _factory = factory;
            }

            public async ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
            {
                var obs = default(IAsyncObservable<T>);

                try
                {
                    obs = await _factory().ConfigureAwait(false);
                }
                catch
                {
                    await observer.DisposeAsync().ConfigureAwait(false);
                    throw;
                }

                await obs.SubscribeAsync(observer, token).ConfigureAwait(false);
            }
        }
    }
}
