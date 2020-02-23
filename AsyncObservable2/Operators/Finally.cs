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
            try
            {
                await _source.SubscribeAsync(observer, token).ConfigureAwait(true);
            }
            finally
            {
                _action();
            }
        }
    }
}
