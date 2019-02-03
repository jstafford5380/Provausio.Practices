using Provausio.Practices.EventSourcing.Aggregate;
using Xunit;

namespace Provausio.Practices.Tests.EventSourcing
{
    public class AggregateEventTests
    {
        [Fact]
        public void Create_GeneratesAnId()
        {
            // arrange

            // act
            var ev = AggregateEvent.Create<TestEvent1>();

            Assert.NotEmpty(ev.Id.ToString());
            Assert.NotNull(ev.Id);
        }
    }

    public class TestEvent1 : AggregateEvent
    {

    }
}
