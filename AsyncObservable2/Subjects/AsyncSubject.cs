using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    public class AsyncSubject<T> : IAsyncSubject<T>
    {
        class Subscription : TaskCompletionSource<bool>
        {
            public Subscription(IAsyncObserver<T> observer, CancellationToken token)
            {
                Observer = observer;
                Token = token;
            }

            public IAsyncObserver<T> Observer { get; }
            public CancellationToken Token { get; }
        }

        private readonly object _locker = new object();

        private Subscription[] _subscriptions = new Subscription[] { };
        private ConcurrentQueue<Subscription> _cancelled = new ConcurrentQueue<Subscription>();
        private bool _isRunning = false;

        public async ValueTask<bool> OnNextAsync(T value)
        {
            Subscription[] list;

            lock (_locker)
            {
                _isRunning = true;
                list = _subscriptions;
            }

            foreach (var s in list)
            {
                bool keep = true;

                try
                {
                    keep = await s.Observer.OnNextAsync(value).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    s.TrySetException(ex);
                }

                if (!keep)
                {
                    s.TrySetResult(true);
                }
            }
            
            lock (_locker)
            {
                _isRunning = false;
            }

            while (_cancelled.TryDequeue(out var subscription))
            {
                subscription.TrySetCanceled(subscription.Token);
            }

            return true;
        }
        
        public ValueTask DisposeAsync()
        {
            Subscription[] list;

            lock (_locker)
            {
                _isRunning = true;
                list = _subscriptions;
            }

            foreach (var s in list)
            {
                s.TrySetResult(true);
            }
            
            lock (_locker)
            {
                _isRunning = false;
            }

            while (_cancelled.TryDequeue(out var subscription))
            {
                subscription.TrySetCanceled(subscription.Token);
            }

            return default;
        }


        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
        {
            var subscription = new Subscription(observer, token);

            // append the subscription 
            lock (_locker)
            {
                _subscriptions = _subscriptions.Append(subscription).ToArray();
            }

            if (token.CanBeCanceled)
            {
                token.Register(() =>
                {
                    bool enqueud = false;
                    lock (_locker)
                    {
                        if (_isRunning)
                        {
                            _cancelled.Enqueue(subscription);
                            enqueud = true;
                        }
                    }

                    if (!enqueud)
                    {
                        subscription.TrySetCanceled(token);
                    }
                });
            }

            try
            {
                await subscription.Task;
            }
            finally
            {
                // remove the subscription 
                lock (_locker)
                {
                    _subscriptions = _subscriptions.Where(s => s != subscription).ToArray();
                }

                await subscription.Observer.DisposeAsync();
            }
        }
    }
}
