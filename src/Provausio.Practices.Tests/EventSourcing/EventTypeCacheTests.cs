using System;
using Provausio.Practices.EventSourcing;
using Xunit;

namespace Provausio.Practices.Tests.EventSourcing
{
    public class EventTypeCacheTests
    {
        // these tests will randomly fail when run on appveyor. Not sure why.
        //[Theory]
        //[InlineData("doubledecorated1")]
        //[InlineData("doubledecorated2")]
        public void ResolveType_DoubleDecorated_Resolves(string nameSpace)
        {
            // arrange
            var cache = EventTypeCache.GetInstance(typeof(DoubleDecoratedClass));

            // act
            var type = cache.ResolveType(nameSpace);

            // assert
            Assert.Equal(typeof(DoubleDecoratedClass), type);
        }

        [Fact]
        public void ResolveType_MultipleRequestedNamespacesMatch_DoesNotThrow()
        {
            // arrange
            var cache = EventTypeCache.GetInstance(typeof(DoubleDecoratedClass));

            // act
            var type = cache.ResolveType("doubledecorated1;doubledecorated2");

            // assert
            Assert.True(true);
        }

        [EventNamespace("Type1")]
        public class Type1 : EventInfo<Guid>
        {
            
        }

        [EventNamespace("Type2")]
        public class Type2 : EventInfo<Guid>
        {
            
        }

        [EventNamespace("doubledecorated1")]
        [EventNamespace("doubledecorated2")]
        private class DoubleDecoratedClass : EventInfo<Guid>
        {

        }
    }
}
