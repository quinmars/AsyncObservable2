using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    class Finally<T> : IAsyncObservable<T>
    {
        readonly Action _action;
        readonly IAsyncObservable<T> _source;

        public Finally(IAsyncObservable<T> source, Action action)
        {
            _action = action;
            _source = source;
        }

        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
        {
            var obs = new Observer(observer, _action);
            await _source.SubscribeAsync(obs, token).ConfigureAwait(false);
        }

        private class Observer : IAsyncObserver<T>
        { 
            readonly IAsyncObserver<T> _observer;
            readonly Action _action;

            public Observer(IAsyncObserver<T> observer, Action action)
            {
                _observer = observer;
                _action = action;
            }

            public ValueTask<bool> OnNextAsync(T value) => _observer.OnNextAsync(value);
            
            public async ValueTask DisposeAsync()
            {
                try
                {
                    _action();
                }
                finally
                {
                    await _observer.DisposeAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
