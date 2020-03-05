using FluentAssertions;
using Quinmars.AsyncObservable2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class AsyncSubjectTests
    {
        [Fact]
        public async Task OneObserverEmpty()
        {
            var subject = new AsyncSubject<int>();
            string result = "";

            var t = subject
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            await subject.DisposeAsync();

            await t;

            result
                .Should().Be("FC");
        }
        
        [Fact]
        public async Task OneObserverOneItem()
        {
            var subject = new AsyncSubject<int>();
            string result = "";

            var t = subject
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            await subject.OnNextAsync(1);
            await subject.DisposeAsync();

            await t;

            result
                .Should().Be("1FC");
        }
        
        [Fact]
        public async Task OneObserverTwoItems()
        {
            var subject = new AsyncSubject<int>();
            string result = "";

            var t = subject
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            await subject.OnNextAsync(1);
            await subject.OnNextAsync(2);
            await subject.DisposeAsync();

            await t;

            result
                .Should().Be("12FC");
        }
        
        [Fact]
        public async Task TwoObserverEmpty()
        {
            var subject = new AsyncSubject<int>();
            string result1 = "";
            string result2 = "";

            var t1 = subject
                .Finally(() => result1 += "F")
                .SubscribeAsync(i => result1 += i, ex => result1 += "E", () => result1 += "C");

            var t2 = subject
                .Finally(() => result2 += "F")
                .SubscribeAsync(i => result2 += i, ex => result2 += "E", () => result2 += "C");

            await subject.DisposeAsync();

            await t1;
            await t2;

            result1
                .Should().Be("FC");
            result2
                .Should().Be("FC");
        }
        
        [Fact]
        public async Task TwoObserverTwoItems()
        {
            var subject = new AsyncSubject<int>();
            string result1 = "";
            string result2 = "";

            var t1 = subject
                .Finally(() => result1 += "F")
                .SubscribeAsync(i => result1 += i, ex => result1 += "E", () => result1 += "C");

            var t2 = subject
                .Finally(() => result2 += "F")
                .SubscribeAsync(i => result2 += i, ex => result2 += "E", () => result2 += "C");

            await subject.OnNextAsync(1);
            await subject.OnNextAsync(2);
            await subject.DisposeAsync();

            await t1;
            await t2;

            result1
                .Should().Be("12FC");
            result2
                .Should().Be("12FC");
        }
        
        [Fact]
        public async Task TwoObserverOneCancelled()
        {
            var subject = new AsyncSubject<int>();
            var cts = new CancellationTokenSource();

            string result1 = "";
            string result2 = "";

            var t1 = subject
                .Finally(() => result1 += "F")
                .SubscribeAsync(i => result1 += i, ex => result1 += "E", () => result1 += "C", cts.Token);

            var t2 = subject
                .Finally(() => result2 += "F")
                .SubscribeAsync(i => result2 += i, ex => result2 += "E", () => result2 += "C");

            await subject.OnNextAsync(1);
            cts.Cancel();
            await subject.OnNextAsync(2);
            await subject.DisposeAsync();

            await t1;
            await t2;

            result1
                .Should().Be("1FE");
            result2
                .Should().Be("12FC");
        }
        
        [Fact]
        public async Task TwoObserverOneThrows()
        {
            var subject = new AsyncSubject<int>();

            string result1 = "";
            string result2 = "";

            var t1 = subject
                .Do(_ => throw new TestException())
                .SubscribeAsync(i => result1 += i, ex => result1 += "E", () => result1 += "C");

            var t2 = subject
                .SubscribeAsync(i => result2 += i, ex => result2 += "E", () => result2 += "C");

            await subject.OnNextAsync(1);
            await subject.OnNextAsync(2);
            await subject.DisposeAsync();

            await t1;
            await t2;

            result1
                .Should().Be("E");
            result2
                .Should().Be("12C");
        }
        
        [Fact]
        public async Task TwoObserverOneQuits()
        {
            var subject = new AsyncSubject<int>();

            string result1 = "";
            string result2 = "";

            var t1 = subject
                .Take(1)
                .SubscribeAsync(i => result1 += i, ex => result1 += "E", () => result1 += "C");

            var t2 = subject
                .SubscribeAsync(i => result2 += i, ex => result2 += "E", () => result2 += "C");

            await subject.OnNextAsync(1);
            await subject.OnNextAsync(2);
            await subject.DisposeAsync();

            await t1;
            await t2;

            result1
                .Should().Be("1C");
            result2
                .Should().Be("12C");
        }
    }
}
