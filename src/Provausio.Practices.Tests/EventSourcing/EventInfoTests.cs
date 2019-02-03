using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Newtonsoft.Json;
using Provausio.Practices.EventSourcing;
using Xunit;

namespace Provausio.Practices.Tests.EventSourcing
{
    [ExcludeFromCodeCoverage]
    public class EventInfoTests
    {
        private byte[] _v1data;
        private byte[] _v2data;
        private byte[] _v1metadata;
        private byte[] _v2metadata;

        public EventInfoTests()
        {
            _v1data = Encoding.UTF8.GetBytes("{\"Property1\": \"Hello world!\"}");
            _v2data = Encoding.UTF8.GetBytes("{\"Id\": \"b7a90398-e98d-4603-9e88-4be76009bf24\"}");
            _v1metadata =
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new EventMetadata {EventNamespaces = "test1"}));
            _v2metadata =
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new EventMetadata { EventNamespaces = "test2" }));
        }

        [Fact]
        public void Ctor_DefaultId_Throws()
        {
            // arrange

            // act

            // assert
            Assert.Throws<ArgumentException>(() => new FakeEventInfo(new Guid(), "foo"));
        }

        [Fact]
        public void Ctor_NullReason_Throws()
        {
            // arrange

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => new FakeEventInfo(Guid.NewGuid(), null));
        }

        [Fact]
        public void Ctor_EmptyReason_Throws()
        {
            // arrange

            // act

            // assert
            Assert.Throws<ArgumentException>(() => new FakeEventInfo(Guid.NewGuid(), string.Empty));
        }

        [Fact]
        public void Ctor_ValidParameters_EventGetsUniqueId()
        {
            // arrange

            // act
            var e1 = new FakeEventInfo(Guid.NewGuid(), "foo");
            var e2 = new FakeEventInfo(Guid.NewGuid(), "bar");

            // assert
            Assert.NotEqual(new Guid(), e1.EventId);
            Assert.NotEqual(new Guid(), e2.EventId);
            Assert.NotEqual(e1.EventId, e2.EventId);
        }

        [Fact]
        public void GetData_SerializesCorrectly()
        {
            // arrange
            var id = Guid.NewGuid();
            var original = new FakeEventInfo(id, "foo");
            
            // act
            var data = original.GetData();

            // assert
            var asJson = Encoding.UTF8.GetString(data);
            var asObject = JsonConvert.DeserializeObject<FakeEventInfo>(asJson);
            Assert.Equal(original, asObject);
        }

        [Fact]
        public void Ctor_DefaultCtor_IsPermittedForDeserialization()
        {
            // arrange

            // act
            var info = new FakeEventInfo();

            // assert
            Assert.NotNull(info);
        }

        [Fact]
        public void GetHashCode_DifferentObjects_DifferentHashCode()
        {
            // arrange
            var e1 = new FakeEventInfo {EventId = Guid.NewGuid()};
            var e2 = new FakeEventInfo {EventId = Guid.NewGuid()};

            // act
            var areEqual = e1.GetHashCode() == e2.GetHashCode();

            // assert
            Assert.False(areEqual);
        }

        private class FakeEventInfo : EventInfo<Guid>
        {
            public FakeEventInfo()
            {
            }

            public FakeEventInfo(Guid entityId, string reason) 
                : base(entityId, reason)
            {
            }
        }

        [EventNamespace("test1")]
        private class Version1 : EventInfo<Guid>
        {
            public string Property1 { get; set; }
        }

        [EventNamespace("test2")]
        private class Version2 : EventInfo<Guid>
        {
            public Guid Id { get; set; }
        }

        [EventNamespace("test3")]
        private class Version3
        {
            public string Property2 { get; set; }
        }
    }
}
