using System;
using System.Collections.Generic;
using System.Text;

namespace Quinmars.AsyncObservable2
{
    interface IAsyncSubject<in TSource,out TResult> : IAsyncObservable<TResult>, IAsyncObserver<TSource>
    {
    }

    interface IAsyncSubject<T> : IAsyncSubject<T, T>
    {
    }
}
