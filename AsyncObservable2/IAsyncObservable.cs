using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    public interface IAsyncObservable<out T>
    {
        ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token);
    }
}
