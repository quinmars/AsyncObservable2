using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    internal class ToObservable<T> : IObservable<T>
    {
        readonly IAsyncObservable<T> _obs;
        public ToObservable(IAsyncObservable<T> obs)
        {
            _obs = obs;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var d = new CancellationDisposable(new CancellationTokenSource());
            
            Subscribe(observer, d.Token);
            
            return d;
        }

        private async void Subscribe(IObserver<T> observer, CancellationToken token)
        {
            var o = new Observer(observer);

            try
            {
                await _obs.SubscribeAsync(o, token).ConfigureAwait(false);
                observer.OnCompleted();
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
            }
        }

        private class Observer : IAsyncObserver<T>
        {
            private readonly IObserver<T> _observer;
            public Observer(IObserver<T> observer)
            {
                _observer = observer;
            }

            public ValueTask DisposeAsync()
            {
                return default;
            }

            public ValueTask<bool> OnNextAsync(T value)
            {
                _observer.OnNext(value);
                return default;
            }
        }
    }
}
