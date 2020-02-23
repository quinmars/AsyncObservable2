using System;
using Xunit;
using Quinmars.AsyncObservable2;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;
using FluentAssertions.Extensions;

namespace Tests
{
    public class PrefetchTests
    {
        [Fact]
        public async Task SlowConsumer()
        {
            string result = "";

            var scheduler = new TestAsyncScheduler();

            await scheduler.RunAsync(async () =>
            {
                return await AsyncObservable.Range(0, 5)
                   .Do(i => result += i)
                   .Prefetch()
                   .Do(i => result += i)
                   .ConsumeSlowly(scheduler,
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds());
            });

            result
                .Should().Be("0012341234");
        }

        [Fact]
        public async Task SlowProduce()
        {
            string result = "";

            var scheduler = new TestAsyncScheduler();

            await scheduler.RunAsync(async () =>
            {
                return await scheduler.ProduceSlowly(
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds())
                   .Do(i => result += i)
                   .Prefetch()
                   .Do(i => result += i)
                   .ConsumeFast(scheduler);
            });

            result
                .Should().Be("0011223344");
        }
        
        [Fact]
        public async Task ExceptionBefore()
        {
            string result = "";

            var scheduler = new TestAsyncScheduler();

            var catched = false;
            try
            {

                await AsyncObservable.Range(0, 5)
                   .Do(i => result += i)
                   .Do(i =>
                   {
                       if (i == 2)
                           throw new TestException();
                   })
                   .Prefetch()
                   .Do(i => result += i)
                   .SubscribeAsync()
                   .ConfigureAwait(false);
            }
            catch (TestException)
            {
                catched = true;
            }

            catched
                .Should().BeTrue();
            result
                .Should().Be("00112");
        }

        [Fact]
        public async Task ExceptionAfter()
        {
            string result = "";

            var scheduler = new TestAsyncScheduler();

            var catched = false;
            try
            {

                await AsyncObservable.Range(0, 5)
                   .Do(i => result += i)
                   .Prefetch()
                   .Do(i => result += i)
                   .Do(i =>
                   {
                       if (i == 2)
                           throw new TestException();
                   })
                   .SubscribeAsync()
                   .ConfigureAwait(false);
            }
            catch (TestException)
            {
                catched = true;
            }

            catched
                .Should().BeTrue();
            result
                .Should().Be("001122");
        }
    }
}
