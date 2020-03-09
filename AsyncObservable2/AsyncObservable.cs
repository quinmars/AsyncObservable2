using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    public static class AsyncObservable
    {
        public static IAsyncObservable<T> Defer<T>(Func<IAsyncObservable<T>> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            return new Defer<T>.Sync(factory);
        }

        public static IAsyncObservable<T> Defer<T>(Func<ValueTask<IAsyncObservable<T>>> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            return new Defer<T>.Async(factory);
        }

        public static IAsyncObservable<T> Distinct<T>(this IAsyncObservable<T> sources)
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));

            return new DistinctBy<T, T>(sources, v => v);
        }

        public static IAsyncObservable<T> DistinctBy<T, TSelect>(this IAsyncObservable<T> sources, Func<T, TSelect> selector)
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new DistinctBy<T, TSelect>(sources, selector);
        }

        public static IAsyncObservable<T> DistinctUntilChanged<T>(this IAsyncObservable<T> sources)
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));

            return new DistinctUntilChanged<T, T>(sources, v => v, EqualityComparer<T>.Default);
        }

        public static IAsyncObservable<T> DistinctUntilChanged<T, TSelect>(this IAsyncObservable<T> sources, Func<T, TSelect> selector)
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new DistinctUntilChanged<T, TSelect>(sources, selector, EqualityComparer<TSelect>.Default);
        }

        public static IAsyncObservable<T> Do<T>(this IAsyncObservable<T> source, Action<T> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return new Do<T>(source, action);
        }

        public static IAsyncObservable<T> DropOnBackpressure<T>(this IAsyncObservable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new DropOnBackpressure<T>(source);
        }

        public static IAsyncObservable<T> Empty<T>()
        {
            return new Empty<T>();
        }

        public static IAsyncObservable<T> Finally<T>(this IAsyncObservable<T> source, Action action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return new Finally<T>(source, action);
        }
        
        public static IAsyncObservable<T> LazyConcat<T>(this IAsyncObservable<IAsyncObservable<T>> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new LazyConcat<T>(source);
        }

        public static IAsyncObservable<T> Never<T>()
        {
            return new Never<T>();
        }

        public static IAsyncObservable<T> Prefetch<T>(this IAsyncObservable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new Prefetch<T>(source);
        }

        public static IAsyncObservable<int> Range(int start, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return new Range(start, count);
        }

        public static IAsyncObservable<T> Return<T>(T value)
        {
            return new Return<T>(value);
        }

        public static IAsyncObservable<TResult> Select<TSource, TResult>(this IAsyncObservable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new Select<TSource, TResult>.Sync(source, selector);
        }

        public static IAsyncObservable<TResult> Select<TSource, TResult>(this IAsyncObservable<TSource> source, Func<TSource, ValueTask<TResult>> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new Select<TSource, TResult>.Async(source, selector);
        }

        public static IAsyncObservable<TResult> Select<TSource, TResult>(this IAsyncObservable<TSource> source, Func<TSource, CancellationToken, ValueTask<TResult>> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new Select<TSource, TResult>.AsyncWithCancellation(source, selector);
        }

        public static IAsyncObservable<T> Skip<T>(this IAsyncObservable<T> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return new Skip<T>(source, count);
        }

        public static IAsyncObservable<T> Take<T>(this IAsyncObservable<T> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return new Take<T>(source, count);
        }

        public static IAsyncObservable<T> Throw<T>(Exception ex)
        {
            return new Throw<T>(ex);
        }

        public static IAsyncObservable<T> Where<T>(this IAsyncObservable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return new Where<T>.Sync(source, predicate);
        }

        public static IAsyncObservable<T> Where<T>(this IAsyncObservable<T> source, Func<T, ValueTask<bool>> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return new Where<T>.Async(source, predicate);
        }

        public static IAsyncObservable<T> ToAsyncObservable<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new FromEnumerable<T>(source);
        }

        public static async ValueTask SubscribeAsync<T>(this IAsyncObservable<T> source, Action<T> onNext = null, Action<Exception> onError = null, Action onCompleted = null, CancellationToken token = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var observer = new AnonymousAsyncObserver<T>.Sync(onNext);

            try
            {
                await source.SubscribeAsync(observer, token);
                onCompleted?.Invoke();
            }
            catch (Exception ex) when (onError != null)
            {
                onError(ex);
            }
        }

        public static async ValueTask SubscribeAsync<T>(this IAsyncObservable<T> source, Func<T, ValueTask> onNext, Func<Exception, ValueTask> onError = null, Func<ValueTask> onCompleted = null, CancellationToken token = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var observer = new AnonymousAsyncObserver<T>.Async(onNext);

            try
            {
                await source.SubscribeAsync(observer, token);
                
                if (onCompleted != null)
                {
                    await onCompleted();
                }
            }
            catch (Exception ex) when (onError != null)
            {
                await onError(ex);
            }
        }
    }
}
