using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    class Do<T> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<T> _source;
        readonly Action<T> _action;

        public Do(IAsyncObservable<T> source, Action<T> action)
        {
            _source = source;
            _action = action;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
        {
            var o = new Observer(observer, _action);
            return _source.SubscribeAsync(o, token);
        }

        class Observer : IAsyncObserver<T>
        {
            readonly Action<T> _action;
            readonly IAsyncObserver<T> _observer;

            public Observer(IAsyncObserver<T> observer, Action<T> action)
            {
                _observer = observer;
                _action = action;
            }

            public async ValueTask<bool> OnNextAsync(T value)
            {
                _action(value);
                return await _observer.OnNextAsync(value).ConfigureAwait(false);
            }
            
            public async ValueTask DisposeAsync()
            {
                await _observer.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
