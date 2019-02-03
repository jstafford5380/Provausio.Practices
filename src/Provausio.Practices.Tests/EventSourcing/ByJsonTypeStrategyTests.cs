using System;
using System.Text;
using Newtonsoft.Json;
using Provausio.Practices.EventSourcing;
using Provausio.Practices.EventSourcing.Deserialization;
using Xunit;

namespace Provausio.Practices.Tests.EventSourcing
{
    public class ByJsonTypeStrategyTests
    {
        private readonly byte[] _eventData;
        private readonly byte[] _metaData;
        private const string TestValue = "Hello World";

        public ByJsonTypeStrategyTests()
        {
            var typeClass = new JsonTypeClass { Value = TestValue };
            _eventData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(
                    typeClass,
                    new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All}));
            _metaData = new EventMetadata().GetData();
        }

        [Fact]
        public void DeserializeEvent_TypedObject_Deserializes()
        {
            // arrange
            var strat = new ByJsonTypeStrategy<Guid>();

            // act
            var result = (JsonTypeClass) strat.DeserializeEvent(_eventData, _metaData);

            //assert
            Assert.Equal(TestValue, result.Value);
        }

        [Fact]
        public void DeserializeEvent_UntypedObject_Throws()
        {
            // arrange
            var strat = new ByJsonTypeStrategy<Guid>();
            var untypedObject = JsonConvert.SerializeObject(new JsonTypeClass { Value = "irrelevant" });
            var asBytes = Encoding.UTF8.GetBytes(untypedObject);

            // act

            // assert
            Assert.Throws<InvalidCastException>(() => strat.DeserializeEvent(asBytes, _metaData));
        }

        private class JsonTypeClass : EventInfo<Guid>
        {
            public string Value { get; set; }
        }
    }
}
