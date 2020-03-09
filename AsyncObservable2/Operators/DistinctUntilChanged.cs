using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    class DistinctUntilChanged<T, TSelect> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<T> _source;
        readonly Func<T, TSelect> _selector;
        readonly IEqualityComparer<TSelect> _comparer;

        public DistinctUntilChanged(IAsyncObservable<T> source, Func<T, TSelect> selector, IEqualityComparer<TSelect> comparer)
        {
            _source = source;
            _selector = selector;
            _comparer = comparer;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
        {
            var o = new Observer(observer, _selector, _comparer);
            return _source.SubscribeAsync(o, token);
        }

        class Observer : IAsyncObserver<T>
        {
            readonly IAsyncObserver<T> _observer;
            readonly Func<T, TSelect> _selector;
            readonly IEqualityComparer<TSelect> _comparer;
            TSelect lastValue;
            bool hasLastValue;

            public Observer(IAsyncObserver<T> observer, Func<T, TSelect> selector, IEqualityComparer<TSelect> comparer)
            {
                _observer = observer;
                _selector = selector;
                _comparer = comparer;
            }

            public ValueTask<bool> OnNextAsync(T value)
            {
                var sval = _selector(value);
                if (hasLastValue && _comparer.Equals(sval, lastValue))
                    return new ValueTask<bool>(true);

                hasLastValue = true;
                lastValue = sval;

                return _observer.OnNextAsync(value);
            }

            public ValueTask DisposeAsync() => _observer.DisposeAsync();
        }
    }
}
