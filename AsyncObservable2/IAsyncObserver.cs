using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    public interface IAsyncObserver<in T> : IAsyncDisposable
    {
        ValueTask<bool> OnNextAsync(T value);
    }
}
