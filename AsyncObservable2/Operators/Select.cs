using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    static class Select<TSource, TResult>
    {
        public class Sync : IAsyncObservable<TResult>
        {
            readonly IAsyncObservable<TSource> _source;
            readonly Func<TSource, TResult> _selector;

            public Sync(IAsyncObservable<TSource> source, Func<TSource, TResult> selector)
            {
                _source = source;
                _selector = selector;
            }

            public ValueTask SubscribeAsync(IAsyncObserver<TResult> observer, CancellationToken token)
            {
                var o = new Observer(observer, _selector);
                return _source.SubscribeAsync(o, token);
            }

            class Observer : IAsyncObserver<TSource>
            {
                readonly IAsyncObserver<TResult> _observer;
                readonly Func<TSource, TResult> _selector;

                public Observer(IAsyncObserver<TResult> observer, Func<TSource, TResult> selector)
                {
                    _observer = observer;
                    _selector = selector;
                }

                public ValueTask<bool> OnNextAsync(TSource value)
                {
                    return _observer.OnNextAsync(_selector(value));
                }

                public async ValueTask DisposeAsync()
                {
                    await _observer.DisposeAsync();
                }
            }
        }

        public class Async : IAsyncObservable<TResult>
        {
            readonly IAsyncObservable<TSource> _source;
            readonly Func<TSource, ValueTask<TResult>> _selector;

            public Async(IAsyncObservable<TSource> source, Func<TSource, ValueTask<TResult>> selector)
            {
                _source = source;
                _selector = selector;
            }

            public ValueTask SubscribeAsync(IAsyncObserver<TResult> observer, CancellationToken token)
            {
                var o = new Observer(observer, _selector);
                return _source.SubscribeAsync(o, token);
            }

            class Observer : IAsyncObserver<TSource>
            {
                readonly IAsyncObserver<TResult> _observer;
                readonly Func<TSource, ValueTask<TResult>> _selector;

                public Observer(IAsyncObserver<TResult> observer, Func<TSource, ValueTask<TResult>> selector)
                {
                    _observer = observer;
                    _selector = selector;
                }

                public async ValueTask<bool> OnNextAsync(TSource value)
                {
                    var v = await _selector(value).ConfigureAwait(false);
                    return await _observer.OnNextAsync(v).ConfigureAwait(false);
                }
                
                public async ValueTask DisposeAsync()
                {
                    await _observer.DisposeAsync();
                }
            }
        }

        public class AsyncWithCancellation : IAsyncObservable<TResult>
        {
            readonly IAsyncObservable<TSource> _source;
            readonly Func<TSource, CancellationToken, ValueTask<TResult>> _selector;

            public AsyncWithCancellation(IAsyncObservable<TSource> source, Func<TSource, CancellationToken, ValueTask<TResult>> selector)
            {
                _source = source;
                _selector = selector;
            }

            public ValueTask SubscribeAsync(IAsyncObserver<TResult> observer, CancellationToken token)
            {
                var o = new Observer(observer, _selector, token);
                return _source.SubscribeAsync(o, token);
            }

            class Observer : IAsyncObserver<TSource>
            {
                readonly IAsyncObserver<TResult> _observer;
                readonly Func<TSource, CancellationToken, ValueTask<TResult>> _selector;
                readonly CancellationToken _token;

                public Observer(IAsyncObserver<TResult> observer, Func<TSource, CancellationToken, ValueTask<TResult>> selector, CancellationToken token)
                {
                    _observer = observer;
                    _selector = selector;
                    _token = token;
                }

                public async ValueTask<bool> OnNextAsync(TSource value)
                {
                    var v = await _selector(value, _token).ConfigureAwait(false);
                    return await _observer.OnNextAsync(v).ConfigureAwait(false);
                }

                public async ValueTask DisposeAsync()
                {
                    await _observer.DisposeAsync();
                }
            }
        }
    }
}
