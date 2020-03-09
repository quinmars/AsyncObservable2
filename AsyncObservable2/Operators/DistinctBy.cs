using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    class DistinctBy<T, TSelect> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<T> _source;
        readonly Func<T, TSelect> _selector;

        public DistinctBy(IAsyncObservable<T> source, Func<T, TSelect> selector)
        {
            _source = source;
            _selector = selector;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
        {
            var o = new Observer(observer, _selector);
            return _source.SubscribeAsync(o, token);
        }

        class Observer : IAsyncObserver<T>
        {
            readonly IAsyncObserver<T> _observer;
            readonly Func<T, TSelect> _selector;
            readonly HashSet<TSelect> _set;

            public Observer(IAsyncObserver<T> observer, Func<T, TSelect> selector)
            {
                _observer = observer;
                _selector = selector;
                _set = new HashSet<TSelect>();
            }

            public ValueTask<bool> OnNextAsync(T value)
            {
                var sval = _selector(value);

                if (!_set.Add(sval))
                    return new ValueTask<bool>(true);

                return _observer.OnNextAsync(value);
            }

            public ValueTask DisposeAsync() => _observer.DisposeAsync();
        }
    }
}
