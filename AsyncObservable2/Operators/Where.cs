using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    class Where<T>
    {
        public class Sync : IAsyncObservable<T>
        {
            readonly IAsyncObservable<T> _source;
            readonly Func<T, bool> _predicate;

            public Sync(IAsyncObservable<T> source, Func<T, bool> predicate)
            {
                _source = source;
                _predicate = predicate;
            }

            public ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
            {
                var o = new Observer(observer, _predicate);
                return _source.SubscribeAsync(o, token);
            }

            class Observer : IAsyncObserver<T>
            {
                readonly IAsyncObserver<T> _observer;
                readonly Func<T, bool> _predicate;

                public Observer(IAsyncObserver<T> observer, Func<T, bool> predicate)
                {
                    _observer = observer;
                    _predicate = predicate;
                }

                public ValueTask<bool> OnNextAsync(T value)
                {
                    if (!_predicate(value))
                        return new ValueTask<bool>(true);

                    return _observer.OnNextAsync(value);
                }

                public async ValueTask DisposeAsync()
                {
                    await _observer.DisposeAsync();
                }
            }
        }

        public class Async : IAsyncObservable<T>
        {
            readonly IAsyncObservable<T> _source;
            readonly Func<T, ValueTask<bool>> _predicate;

            public Async(IAsyncObservable<T> source, Func<T, ValueTask<bool>> predicate)
            {
                _source = source;
                _predicate = predicate;
            }

            public ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
            {
                var o = new Observer(observer, _predicate);
                return _source.SubscribeAsync(o, token);
            }

            class Observer : IAsyncObserver<T>
            {
                readonly IAsyncObserver<T> _observer;
                readonly Func<T, ValueTask<bool>> _predicate;

                public Observer(IAsyncObserver<T> observer, Func<T, ValueTask<bool>> predicate)
                {
                    _observer = observer;
                    _predicate = predicate;
                }

                public async ValueTask<bool> OnNextAsync(T value)
                {
                    if (!await _predicate(value).ConfigureAwait(false))
                        return true;

                    return await _observer.OnNextAsync(value).ConfigureAwait(false);
                }

                public async ValueTask DisposeAsync()
                {
                    await _observer.DisposeAsync();
                }
            }
        }
    }
}
