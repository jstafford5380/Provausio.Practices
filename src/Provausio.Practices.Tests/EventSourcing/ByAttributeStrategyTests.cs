using System;
using System.IO;
using System.Reflection;
using Provausio.Practices.EventSourcing;
using Provausio.Practices.EventSourcing.Deserialization;
using Xunit;

namespace Provausio.Practices.Tests.EventSourcing
{
    public class ByAttributeStrategyTests
    {
        private readonly byte[] _eventData;
        private readonly byte[] _metaData;
        private readonly Guid _testId = Guid.NewGuid();

        public ByAttributeStrategyTests()
        {
            _eventData = new ValidEventClass { Id = _testId }.GetData();
            _metaData = new EventMetadata { EventNamespaces = "byattributetest" }.GetData();
        }

        //[Fact]
        public void DeserializeEvent_ByPath_ValidEvent_Deserializes()
        {
            // arrange
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(ValidEventClass)).Location);
            var strat = new ByAttributeStrategy<Guid>(path);

            // act
            var result = (ValidEventClass) strat.DeserializeEvent(_eventData, _metaData);

            // assert
            Assert.Equal(_testId, result.Id);
        }

        //[Fact]
        public void DeserializeEvent_ByTypeRef_ValidEvent_Deserializes()
        {
            // TODO: this test will randomly break with a null object ref... WHY?!

            // arrange
            var strat = new ByAttributeStrategy<Guid>(typeof(ValidEventClass));

            // act
            var result = (ValidEventClass)strat.DeserializeEvent(_eventData, _metaData);

            // assert
            Assert.Equal(_testId, result.Id);
        }
        
        [EventNamespace("byattributetest")]
        public class ValidEventClass : EventInfo<Guid>
        {
            public Guid Id { get; set; }
        }

        private class InvalidClass
        {
            public string Value { get; set; }
        }
    }
}
