using System;
using System.Collections.Generic;
using Provausio.Practices.EventSourcing;
using Xunit;

namespace Provausio.Practices.Tests.EventSourcing
{
    public class AggregateTests
    {
        [Fact]
        public void Ctor_VersionIsNew()
        {
            // arrange

            // act
            var agg = new FakeAggregate();

            // assert
            Assert.Equal(-1L, agg.Version);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(8)]
        [InlineData(13)]
        public void LoadFromHistory_AdvancesVersion(int count)
        {
            // arrange
            var agg = new FakeAggregate();
            var events = new List<EventInfo<Guid>>();
            for(var incomingVersion = 0; incomingVersion < count; incomingVersion++)
            {
                var newEvent = new FakeEvent {Version = incomingVersion};
                events.Add(newEvent);
            }

            // act
            agg.LoadFromHistory(events);

            // assert
            Assert.Equal(count - 1, agg.Version);
        }

        [Fact]
        public void LoadFromHistory_InvalidVersion_SameVersion_Throws()
        {
            // arrange
            var agg = new FakeAggregate();
            var incomingEvent = new FakeEvent {Version = 5};
            var incomingEvents = new List<EventInfo<Guid>> {incomingEvent, incomingEvent};

            // act
            
            // assert
            Assert.Throws<InvalidOperationException>(() => agg.LoadFromHistory(incomingEvents));
        }

        [Fact]
        public void LoadFromHistory_InvalidVersion_LowerVersion_Throws()
        {
            // arrange
            var agg = new FakeAggregate();
            var firstEvent = new FakeEvent { Version = 5 };
            var newEvent = new FakeEvent {Version = 4};
            var incomingEvents = new List<EventInfo<Guid>> {firstEvent, newEvent};

            // act

            // assert
            Assert.Throws<InvalidOperationException>(() => agg.LoadFromHistory(incomingEvents));
        }

        private class FakeAggregate : AggregateRoot<Guid>
        {

            protected override EventInfo<Guid> BuildSnapshot()
            {
                return null;
            }
        }

        private class FakeEvent : EventInfo<Guid> { }
    }
}
