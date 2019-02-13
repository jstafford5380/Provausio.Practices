using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Provausio.Practices.DDD.InternalEvents;
using Xunit;

namespace Provausio.Practices.Tests.DDD.InternalEvents
{
    public class InternalEvenBusTests
    {
        [Fact]
        public void ScanHandlers_FindsAllHandlers()
        {
            // arrange
            var provider = new ServiceCollection().BuildServiceProvider();
            var bus = new InternalEventBus(provider);
            
            // act
            bus.ScanHandlers();

            // assert
            Assert.Equal(2, bus.Handlers.Count);
            Assert.True(bus.Handlers.ContainsKey(typeof(FooEvent)));
            Assert.Equal(2, bus.Handlers[typeof(FooEvent)].Count);
        }

        [Fact]
        public async Task DispatchEvent_InvokesCorrectHandler()
        {
            // arrange
            var testProvider = new Mock<IProvideNumbers>();
            testProvider.Setup(m => m.Get());

            var services = new ServiceCollection();
            services.AddSingleton(p => testProvider.Object);

            var bus = new InternalEventBus(services.BuildServiceProvider());

            bus.ScanHandlers();
            var fooEvent = new FooEvent();

            // act
            await bus.RaiseEvent(fooEvent);

            /* NOTE: this actually tests that the dependency is injected, and that the handler ran */

            // assert
            testProvider.Verify(m => m.Get(), Times.Exactly(2)); // runs once per handler
        }

        private class FooHandler1 : IHandle<FooEvent>
        {
            private readonly IProvideNumbers numberProvider;

            public FooHandler1(IProvideNumbers numberProvider)
            {
                this.numberProvider = numberProvider;
            }

            public Task Handle(FooEvent message)
            {
                var one = numberProvider.Get();
                return Task.CompletedTask;
            }
        }

        private class FooHandler2 : IHandle<FooEvent>
        {
            private readonly IProvideNumbers numberProvider;

            public FooHandler2(IProvideNumbers numberProvider)
            {
                this.numberProvider = numberProvider;
            }

            public Task Handle(FooEvent message)
            {
                var one = numberProvider.Get();
                return Task.CompletedTask;
            }
        }

        private class BarHandler : IHandle<BarEvent>
        {
            public Task Handle(BarEvent message)
            {
                var one = 1;
                return Task.CompletedTask;
            }
        }

        private class FooEvent : InternalEvent
        {

        }

        private class BarEvent : InternalEvent
        {

        }

        public interface IProvideNumbers
        {
            int Get();
        }

        private class NumberProvider : IProvideNumbers
        {
            private readonly int _numberToProvide;

            public NumberProvider(int numberToProvide)
            {
                _numberToProvide = numberToProvide;
            }

            public int Get()
            {
                return _numberToProvide;
            }
        }
    }
}
