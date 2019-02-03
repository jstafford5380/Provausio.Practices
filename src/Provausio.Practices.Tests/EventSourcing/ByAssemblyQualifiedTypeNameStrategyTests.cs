using System;
using Provausio.Practices.EventSourcing;
using Provausio.Practices.EventSourcing.Deserialization;
using Xunit;

namespace Provausio.Practices.Tests.EventSourcing
{
    public class ByAssemblyQualifiedTypeNameStrategyTests
    {
        private const string TestString = "Hello World";
        private readonly byte[] _metaData;
        private readonly byte[] _eventData;

        public ByAssemblyQualifiedTypeNameStrategyTests()
        {
            _eventData = new TestClass1 {Property1 = TestString}.GetData();
            _metaData = new EventMetadata { AssemblyQualifiedType = typeof(TestClass1).AssemblyQualifiedName }.GetData();
        }

        [Fact]
        public void Deserialize_ValidData_Deserializes()
        {
            // arrange
            var strat = new ByAssemblyQualifiedTypeNameStrategy<Guid>();

            // act
            var result = (TestClass1) strat.DeserializeEvent(_eventData, _metaData);

            // assert
            Assert.Equal(TestString, result.Property1);
        }

        [Fact]
        public void DeserializeEvent_MissingAssemblyName_Throws()
        {
            // arrange
            var strat = new ByAssemblyQualifiedTypeNameStrategy<Guid>();
            var metaData = new EventMetadata().GetData();

            // act

            // assert
            Assert.Throws<InvalidOperationException>(
                () => strat.DeserializeEvent(_eventData, metaData));
        }

        private class TestClass1 : EventInfo<Guid>
        {
            public string Property1 { get; set; }
        }
    }
}
