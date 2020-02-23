using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    static class AnonymousAsyncObserver<T>
    {
        public class Sync : IAsyncObserver<T>
        {
            static readonly Action<T> OnNextNop = v => { };

            readonly Action<T> _onNext;

            public Sync(Action<T> onNext)
            {
                _onNext = onNext ?? OnNextNop;
            }

            public ValueTask<bool> OnNextAsync(T value)
            {
                _onNext(value);
                return new ValueTask<bool>(true);
            }
            
            public ValueTask DisposeAsync()
            {
                return default;
            }
        }

        public class Async : IAsyncObserver<T>
        {
            static readonly Func<T, ValueTask> OnNextNop = v => default;

            readonly Func<T, ValueTask> _onNext;

            public Async(Func<T, ValueTask> onNext)
            {
                _onNext = onNext ?? OnNextNop;
            }

            public async ValueTask<bool> OnNextAsync(T value)
            {
                await _onNext(value);
                return true;
            }

            public ValueTask DisposeAsync()
            {
                return default;
            }
        }
    }
}
