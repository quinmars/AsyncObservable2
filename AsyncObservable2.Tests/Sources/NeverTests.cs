using System;
using Xunit;
using Quinmars.AsyncObservable2;
using System.Threading;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class NeverTests
    {
        [Fact]
        public void Never1()
        {
            string result = "";
            var cts = new CancellationTokenSource();

            _ = AsyncObservable.Never<int>()
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C", cts.Token);

            cts.Cancel();

            result
                .Should().Be("E");
        }
    }
}
