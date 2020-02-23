using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    class Range : IAsyncObservable<int>
    {
        readonly int _start;
        readonly int _end;

        public Range(int start, int count)
        {
            _start = start;
            _end = start + count;
        }

        public async ValueTask SubscribeAsync(IAsyncObserver<int> observer, CancellationToken token)
        {
            try
            {
                for (int i = _start; i < _end; i++)
                {
                    token.ThrowIfCancellationRequested();
                    
                    var keep = await observer.OnNextAsync(i).ConfigureAwait(false);
                    if (!keep)
                    {
                        break;
                    }
                }
            }
            finally
            {
                await observer.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
