using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    class LazyConcat<T> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<IAsyncObservable<T>> _source;

        public LazyConcat(IAsyncObservable<IAsyncObservable<T>> source)
        {
            _source = source;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
        {
            var o = new OuterObserver(observer, token);
            return _source.SubscribeAsync(o, token);
        }

        class OuterObserver : IAsyncObserver<IAsyncObservable<T>>
        {
            readonly IAsyncObserver<T> _observer;
            readonly CancellationToken _token;

            public OuterObserver(IAsyncObserver<T> observer, CancellationToken token)
            {
                _observer = observer;
                _token = token;
            }

            public async ValueTask<bool> OnNextAsync(IAsyncObservable<T> value)
            {
                var inner = new InnerObserver(_observer);
                await value.SubscribeAsync(inner, _token).ConfigureAwait(false);
                return inner.Keep;
            }

            public async ValueTask DisposeAsync()
            {
                await _observer.DisposeAsync().ConfigureAwait(false);
            }
        }

        class InnerObserver : IAsyncObserver<T>
        {
            readonly IAsyncObserver<T> _observer;

            public bool Keep { get; set; } = true;

            public InnerObserver(IAsyncObserver<T> observer)
            {
                _observer = observer;
            }

            public async ValueTask<bool> OnNextAsync(T value)
            {
                Keep = await _observer.OnNextAsync(value).ConfigureAwait(false);
                return Keep;
            }

            public ValueTask DisposeAsync()
            {
                return default;
            }
        }
    }
}
