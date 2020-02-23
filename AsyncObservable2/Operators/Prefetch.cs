using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable2
{
    class Prefetch<T> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<T> _source;

        public Prefetch(IAsyncObservable<T> source)
        {
            _source = source;
        }

        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer, CancellationToken token)
        {
            var o = new Observer(observer, token);
            try
            {
                await _source.SubscribeAsync(o, o.Token).ConfigureAwait(false);
            }
            catch (TaskCanceledException) when (o.Exception != null)
            {
            }
            finally
            {
                if (o.Exception != null)
                    throw o.Exception;
            }
        }

        class Observer : IAsyncObserver<T>
        {
            readonly IAsyncObserver<T> _observer;
            readonly ChannelReader<T> _reader;
            readonly ChannelWriter<T> _writer;
            readonly CancellationTokenSource _cts;

            public CancellationToken Token => _cts.Token;
            public Task ReaderTask { get; }
            public Exception Exception { get; set; }

            public Observer(IAsyncObserver<T> observer, CancellationToken token)
            {
                _observer = observer;

                var channel = Channel.CreateUnbounded<T>(new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = true,
                    AllowSynchronousContinuations = true,
                });

                _cts = CancellationTokenSource.CreateLinkedTokenSource(token, default);

                _reader = channel.Reader;
                _writer = channel.Writer;

                ReaderTask = RunReader();
            }

            private async Task RunReader()
            {
                while (await _reader.WaitToReadAsync(Token).ConfigureAwait(false))
                {
                    while (_reader.TryRead(out var value))
                    {
                        try
                        {
                            var keep = await _observer.OnNextAsync(value).ConfigureAwait(false);
                            if (!keep)
                            {
                                _cts.Cancel();
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            Exception = ex;
                            _cts.Cancel();
                            return;
                        }
                    }
                }
            }

            public async ValueTask<bool> OnNextAsync(T value)
            {
                while (await _writer.WaitToWriteAsync(Token).ConfigureAwait(false))
                    if (_writer.TryWrite(value))
                        return true;

                return false;
            }

            public async ValueTask DisposeAsync()
            {
                try
                {
                    _writer.Complete();
                    await ReaderTask.ConfigureAwait(false);
                }
                finally
                {
                    await _observer.DisposeAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
