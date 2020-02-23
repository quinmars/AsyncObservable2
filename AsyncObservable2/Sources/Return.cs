using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    class Return<T> : IAsyncObservable<T>
    {
        readonly T _value;

        public Return(T value)
        {
            _value = value;
        }

        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
        {
            try
            {
                await observer.OnNextAsync(_value).ConfigureAwait(false);
            }
            finally
            {
                await observer.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
