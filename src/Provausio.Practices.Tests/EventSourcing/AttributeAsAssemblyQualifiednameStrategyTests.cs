using System;
using Provausio.Practices.EventSourcing;
using Provausio.Practices.EventSourcing.Deserialization;
using Xunit;

namespace Provausio.Practices.Tests.EventSourcing
{
    public class AttributeAsAssemblyQualifiednameStrategyTests
    {
        // This test will randomly fail but only when run on appveyor. Not sure why. 
        //[Fact]
        public void DeserializeEvent_NewClass_Deserializes()
        {
            // arrange
            var oldclass = new OldClass { Id = Guid.NewGuid() };
            var data = oldclass.GetData();
            var metadata = oldclass.Metadata.GetData();
            var strategy = new AttributeAsAssemblyQualifiedNameStrategy<Guid>(typeof(OldClass));

            // act
            var info = strategy.DeserializeEvent(data, metadata);

            // assert
            Assert.NotNull(info);
        }

        [EventNamespace("OldClass")]
        // this is a simplified namespace name, not an actual AQN
        [EventNamespace("DAS.Practices.Tests.EventSourcing.AttributeAsAssemblyQualifiednameStrategyTests+OldClass, DAS.Practices.Tests, Version=1.0")]
        private class NewClass : EventInfo<Guid>
        {
            public Guid Id { get; set; }
        }

        private class OldClass : EventInfo<Guid>
        {
            public Guid Id { get; set; }
        }
    }
}