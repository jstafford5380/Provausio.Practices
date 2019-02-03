using System;
using System.Collections.Generic;
using System.Linq;
using Provausio.Practices.EventSourcing.Aggregate;
using Xunit;

namespace Provausio.Practices.Tests.EventSourcing
{
    public class AggregateRootTests
    {
        [Fact]
        public void Raise_QueuesEvent()
        {
            // arrange

            // act
            var test = TestRoot.Create("foo", 1);

            // assert
            Assert.Equal(1, test.UncommittedEvents.Count);
            Assert.IsType<CreationEvent>(test.UncommittedEvents.First());
        }

        [Fact]
        public void Raise_NewRoot_VersionIsNotIncremented()
        {
            // arrange
            var test = TestRoot.Create("foo", 1);

            // act
            test.Action1();

            // assert
            Assert.Equal(-1, test.Version);
        }

        [Fact]
        public void Raise_ExistingRoot_VersionIsIncremented()
        {
            // arrange
            var eventStream = new List<AggregateEvent>
            {
                new CreationEvent {Version = 0, RootId = "abc"},
                new Action1Occured {Version = 1, RootId = "abc"},
                new EntityCreated {Version = 2, RootId = "abc"}
            };

            // act
            var test = new TestRoot() {Id = "abc"};
            test.LoadFromStream(eventStream);

            // assert
            Assert.Equal(2, test.Version);
        }

        [Fact]
        public void LoadFromStream_BringsObjectToState()
        {
            // arrange
            var id = Guid.NewGuid();
            var test = new TestRoot { Id = id.ToString() };
            var create = new CreationEvent { Id = id, RootId = id.ToString(), Version = 0, Prop1 = "foo", Prop2 = 1 };
            var action1 = new Action1Occured { RootId = id.ToString(), Version = 1 };

            // act
            test.LoadFromStream(new AggregateEvent[] { create, action1 });

            // assert
            Assert.Equal(0, test.UncommittedEvents.Count);
            Assert.Equal(id.ToString(), test.Id);
            Assert.True(test.Action1Occured);
            Assert.Equal(1, test.Version);
        }

        [Fact]
        public void CreateEntity_CreatesEntity()
        {
            // arrange
            var test = TestRoot.Create("foo", 1);

            // act
            test.CreateEntity("entity-foo");

            // assert
            Assert.Equal("entity-foo", test.EntityProp);
        }

        [Fact]
        public void UpdateEntity_ProxiesEventsToEntity()
        {
            // arrange
            var test = TestRoot.Create("foo", 1);
            test.CreateEntity("entity-foo");

            // act
            test.UpdateEntity("entity-bar");

            // assert
            Assert.Equal("entity-bar", test.EntityProp);
        }
    }
}
