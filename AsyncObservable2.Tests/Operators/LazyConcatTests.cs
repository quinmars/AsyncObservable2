using System;
using Xunit;
using Quinmars.AsyncObservable2;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;
using System.Collections.Generic;

namespace Tests
{
    public class LazyConcatTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obs1 = default(IAsyncObservable<IAsyncObservable<int>>);

            obs1.Invoking(o => o.LazyConcat())
               .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Single()
        {
            string result = "";

            var range = AsyncObservable.Range(0, 3);

            await AsyncObservable.Return(range.Finally(() => result += "f"))
                .LazyConcat()
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012fFC");
        }

        [Fact]
        public async Task Multiple()
        {
            string result = "";

            var seq = new[]
            {
                AsyncObservable.Range(0, 3).Finally(() => result += ","),
                AsyncObservable.Range(3, 2).Finally(() => result += ","),
                AsyncObservable.Range(5, 3).Finally(() => result += ",")
            }.ToAsyncObservable();

            await seq
                .LazyConcat()
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,34,567,FC");
        }

        [Fact]
        public async Task NullItem()
        {
            string result = "";

            var seq = new[]
            {
                AsyncObservable.Range(0, 3).Finally(() => result += ","),
                null,
                AsyncObservable.Range(5, 3).Finally(() => result += ",")
            }.ToAsyncObservable();

            await seq
                .LazyConcat()
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,FE");
        }

        [Fact]
        public async Task ThrowingItem()
        {
            string result = "";

            var seq = new[]
            {
                AsyncObservable.Range(0, 3).Finally(() => result += ","),
                AsyncObservable.Throw<int>(new Exception()),
                AsyncObservable.Range(5, 3).Finally(() => result += ",")
            }.ToAsyncObservable();

            await seq
                .LazyConcat()
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,FE");
        }

        [Fact]
        public async Task ThrowingItems()
        {
            string result = "";

            var seq = new[]
            {
                AsyncObservable.Range(0, 3).Finally(() => result += ","),
                AsyncObservable.Throw<int>(new Exception()),
                AsyncObservable.Throw<int>(new Exception())
            }.ToAsyncObservable();

            await seq
                .LazyConcat()
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,FE");
        }

        [Fact]
        public async Task Take()
        {
            string result = "";

            var seq = new[]
            {
                AsyncObservable.Range(0, 3).Do(_ => result += "_").Finally(() => result += ","),
                AsyncObservable.Range(3, 2).Do(_ => result += "_").Finally(() => result += ","),
                AsyncObservable.Range(5, 3).Do(_ => result += "_").Finally(() => result += ",")
            }.ToAsyncObservable();

            await seq
                .LazyConcat()
                .Take(4)
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("_0_1_2,_3,FC");
        }
    }
}
