//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DAS.Practices.EventSourcing;
//using Moq;
//using Xunit;

//namespace DAS.Practices.Tests.EventSourcing
//{
//    public class RepositoryTests
//    {
//        private readonly Mock<IStoreEvents> _eventStoreMock;
        
//        public RepositoryTests()
//        {
//            _eventStoreMock = new Mock<IStoreEvents>();
//        }

//        [Fact]
//        public void Ctor_NullStreamName_Throws()
//        {
//            // arrange
//            var storeMock = new Mock<IStoreEvents>();

//            // act

//            // assert
//            Assert.Throws<ArgumentNullException>(() => new FakeRepo(null, storeMock.Object));
//        }

//        [Fact]
//        public void Ctor_NullEventStore_Throws()
//        {
//            // arrange
            
//            // act

//            // assert
//            Assert.Throws<ArgumentNullException>(() => new FakeRepo("foo", null));
//        }

//        #region -- Save --

//        [Fact]
//        public async Task Save_NoEvents_DoesntConnectToStore()
//        {
//            // arrange
//            _eventStoreMock.Setup(m => m.Connect());
//            var aggregate = new FakeAggregate();
//            var repo = new FakeRepo("foo", _eventStoreMock.Object);

//            // act
//            await repo.Save(aggregate);

//            // assert
//            _eventStoreMock.Verify(m => m.Connect(), Times.Never);
//        }

//        [Fact]
//        public async Task Save_NoEvents_DoesntCallStore()
//        {
//            // arrange
//            _eventStoreMock.Setup(m => m.AppendEventsAsync(It.IsAny<string>(), It.IsAny<EventInfo>()));
//            var aggregate = new FakeAggregate();
//            var repo = new FakeRepo("foo", _eventStoreMock.Object);

//            // act
//            await repo.Save(aggregate);

//            // assert
//            _eventStoreMock.Verify(m => m.AppendEventsAsync(
//                It.IsAny<string>(), 
//                It.IsAny<EventInfo>()), Times.Never);
//        }

//        [Fact]
//        public async Task Save_HasUnsavedEvents_ConnectsToStore()
//        {
//            // arrange
//            _eventStoreMock.Setup(m => m.Connect());
//            _eventStoreMock.Setup(m => m.Connect()).Returns(Task.CompletedTask);
            
//            var aggregate = new FakeAggregate();
//            aggregate.DoThing();

//            _eventStoreMock.Setup(m => m.AppendEventsAsync(It.IsAny<string>(), It.IsAny<EventInfo>()))
//                .Returns(Task.CompletedTask);


//            var repo = new FakeRepo("foo", _eventStoreMock.Object);

//            // act
//            await repo.Save(aggregate);

//            // assert
//            _eventStoreMock.Verify(m => m.Connect(), Times.Once);
//        }

//        [Fact]
//        public async Task Save_HasUnsavedEvents_AppendsToStore()
//        {
//            // arrange
//            _eventStoreMock.Setup(m => m.Connect()).Returns(Task.CompletedTask);

//            var aggregate = new FakeAggregate();
//            aggregate.DoThing();

//            _eventStoreMock.Setup(m => m.AppendEventsAsync(It.IsAny<string>(), It.IsAny<EventInfo>()))
//                .Returns(Task.CompletedTask);


//            var repo = new FakeRepo("foo", _eventStoreMock.Object);

//            // act
//            await repo.Save(aggregate);

//            // assert
//            _eventStoreMock.Verify(m => m.AppendEventsAsync(It.IsAny<string>(), It.IsAny<EventInfo>()), Times.Once);
//        }

//        [Fact]
//        public async Task Save_HasUnsavedEvents_Disconnects()
//        {
//            // arrange
//            _eventStoreMock.Setup(m => m.Disconnect());
//            _eventStoreMock.Setup(m => m.Connect())
//                .Returns(Task.CompletedTask);

//            var aggregate = new FakeAggregate();
//            aggregate.DoThing();

//            _eventStoreMock.Setup(m => m.AppendEventsAsync(It.IsAny<string>(), It.IsAny<EventInfo>()))
//                .Returns(Task.CompletedTask);


//            var repo = new FakeRepo("foo", _eventStoreMock.Object);

//            // act
//            await repo.Save(aggregate);

//            // assert
//            _eventStoreMock.Verify(m => m.Disconnect(), Times.Once);
//        }

//        [Fact]
//        public async Task Save_HasUnsavedEvents_MarksChangesAsCommitted()
//        {
//            // arrange
//            _eventStoreMock.Setup(m => m.Connect()).Returns(Task.CompletedTask);

//            var aggregate = new FakeAggregate();
//            aggregate.DoThing();
            
//            _eventStoreMock.Setup(m => m.AppendEventsAsync(It.IsAny<string>(), It.IsAny<EventInfo>()))
//                .Returns(Task.CompletedTask);

//            var repo = new FakeRepo("foo", _eventStoreMock.Object);

//            // act
//            await repo.Save(aggregate);

//            // assert
//            Assert.Equal(1, aggregate.ChangesCommittedCalls);
//        }

//        #endregion

//        #region -- GetById --

//        [Fact]
//        public async Task GetById_ConnectsToStore()
//        {
//            // arrange
//            _eventStoreMock.Setup(m => m.Connect()).Returns(Task.CompletedTask);
//            _eventStoreMock
//                .Setup(m => m.GetEvents(It.IsAny<string>(), It.IsAny<int>()))
//                .Returns(Task.FromResult(new EventStreamResult { Events = new List<EventInfo>() }));
//            var repo = new FakeRepo("foo", _eventStoreMock.Object);

//            // act
//            await repo.GetById(It.IsAny<Guid>(), true);

//            // assert
//            _eventStoreMock.Verify(m => m.Connect(), Times.Once);
//        }
        
//        [Fact]
//        public async Task GetById_UseSnapshot_NullSnapshot_StartsAtPositionZero()
//        {
//            // arrange
//            _eventStoreMock
//                .Setup(m => m.GetEvents(It.IsAny<string>(), It.IsAny<int>()))
//                .Returns(Task.FromResult(new EventStreamResult { Events = new List<EventInfo>()}));

//            _eventStoreMock
//                .Setup(m => m.GetSnapshot(It.IsAny<string>()))
//                .Returns(Task.FromResult((EventInfo) null));

//            var repo = new FakeRepo("foo", _eventStoreMock.Object);

//            // act
//            var result = await repo.GetById(It.IsAny<Guid>(), true);

//            // assert
//            _eventStoreMock.Verify(m => m.GetEvents(It.IsAny<string>(), 0));
//        }

//        [Fact]
//        public async Task GetById_UseSnapshot_NotNullSnapshot_StartsAtNextPosition()
//        {
//            // arrange
//            const int snapshotPosition = 99;
//            var snapshot = new FakeSnapshot(snapshotPosition);

//            _eventStoreMock
//                .Setup(m => m.GetEvents(It.IsAny<string>(), It.IsAny<int>()))
//                .Returns(Task.FromResult(new EventStreamResult { Events = new List<EventInfo>() }));

//            _eventStoreMock
//                .Setup(m => m.GetSnapshot(It.IsAny<string>()))
//                .Returns(Task.FromResult((EventInfo) snapshot));

//            var repo = new FakeRepo("foo", _eventStoreMock.Object);

//            // act
//            var result = await repo.GetById(It.IsAny<Guid>(), true);

//            // assert
//            _eventStoreMock.Verify(m => m.GetEvents(It.IsAny<string>(), snapshotPosition + 1));
//        }


//        [Fact]
//        public async Task GetById_NoSnapshotNeeded_Disconnects()
//        {
//            // arrange
//            const int snapshotPosition = 99;
//            var snapshot = new FakeSnapshot(snapshotPosition);

//            _eventStoreMock
//                .Setup(m => m.GetEvents(It.IsAny<string>(), It.IsAny<int>()))
//                .Returns(Task.FromResult(new EventStreamResult { Events = new List<EventInfo>() }));

//            _eventStoreMock
//                .Setup(m => m.GetSnapshot(It.IsAny<string>()))
//                .Returns(Task.FromResult((EventInfo)snapshot));

//            _eventStoreMock.Setup(m => m.Disconnect());

//            var repo = new FakeRepo("foo", _eventStoreMock.Object);

//            // act
//            var result = await repo.GetById(It.IsAny<Guid>(), true);

//            // assert
//            _eventStoreMock.Verify(m => m.Disconnect(), Times.Once);
//        }

//        [Fact]
//        public async Task GetById_SnapshotRequired_SavesSnapshot()
//        {
//            // arrange
//            const int snapshotPosition = 99;
//            var snapshot = new FakeSnapshot(snapshotPosition);

//            _eventStoreMock
//                .Setup(m => m.GetEvents(It.IsAny<string>(), It.IsAny<int>()))
//                .Returns(Task.FromResult(new EventStreamResult { Events = new List<EventInfo>(), NeedsSnapshot = true }));

//            _eventStoreMock
//                .Setup(m => m.GetSnapshot(It.IsAny<string>()))
//                .Returns(Task.FromResult((EventInfo)snapshot));

//            _eventStoreMock
//                .Setup(m => m.AppendEventsAsync(It.IsAny<string>(), It.IsAny<EventInfo>()))
//                .Returns(Task.CompletedTask);

//            var repo = new FakeRepo("foo", _eventStoreMock.Object);

//            // act
//            var result = await repo.GetById(It.IsAny<Guid>(), true);

//            // assert
//            _eventStoreMock.Verify(m => m.AppendEventsAsync(It.IsAny<string>(), It.IsAny<EventInfo>()), Times.Once);
//        }

//        #endregion

//        [Fact]
//        public void Dispose_DisconnectsEventStore()
//        {
//            // arrange
//            _eventStoreMock.Setup(m => m.Disconnect());
//            var repo = new FakeRepo("foo", _eventStoreMock.Object);

//            // act
//            repo.Dispose();

//            // assert
//            _eventStoreMock.Verify(m => m.Disconnect(), Times.Once);
//        }

//        [Fact]
//        public void Dispose_DisposesEventStore()
//        {
//            // arrange
//            _eventStoreMock.Setup(m => m.Dispose());
//            var repo = new FakeRepo("foo", _eventStoreMock.Object);

//            // act
//            repo.Dispose();

//            // assert
//            _eventStoreMock.Verify(m => m.Dispose(), Times.Once);
//        }

//        private class FakeAggregate : Aggregate
//        {
//            public int ChangesCommittedCalls { get; private set; }

//            public FakeAggregate()
//            {
//                Changes = new List<EventInfo>();
//            }

//            public FakeAggregate(List<EventInfo> changes)
//            {
//                Changes = changes;
//            }

//            public void DoThing()
//            {
//                var eventInfo = new FakeEvent();
//                ApplyEvent(eventInfo);
//            }

//            public void Apply(EventInfo e)
//            {
//                Debug.Write("Added event");
//            }

//            public override EventInfo GetSnapshot()
//            {
//                return new FakeSnapshot(Version);
//            }

//            public override ICollection GetUncommittedEvents()
//            {
//                return Changes;
//            }

//            public override void MarkChangesAsCommitted()
//            {
//                ChangesCommittedCalls++;
//            }
//        }

//        private class FakeRepo : Repository<FakeAggregate>
//        {
//            public FakeRepo(string streamName, IStoreEvents eventStore) 
//                : base(streamName, eventStore)
//            {
//            }
//        }

//        private class FakeEvent : EventInfo
//        {
//        }

//        private class FakeSnapshot : EventInfo
//        {
//            public FakeSnapshot(int version)
//            {
//                Version = version;
//            }
//        }
//    }
//}

