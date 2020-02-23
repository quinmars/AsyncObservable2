using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    class Take<T> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<T> _source;
        readonly int _count;

        public Take(IAsyncObservable<T> source, int count)
        {
            _source = source;
            _count = count;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
        {
            var o = new Observer(observer, _count);
            return _source.SubscribeAsync(o, token);
        }

        class Observer : IAsyncObserver<T>
        {
            readonly IAsyncObserver<T> _observer;
            int _remaining;

            public Observer(IAsyncObserver<T> observer, int count)
            {
                _observer = observer;
                _remaining = count;
            }

            public async ValueTask<bool> OnNextAsync(T value)
            {
                if (_remaining > 0)
                {
                    _remaining--;
                    return await _observer.OnNextAsync(value) && _remaining > 0;
                }

                return false;
            }

            public async ValueTask DisposeAsync()
            {
                await _observer.DisposeAsync();
            }
        }
    }
}
