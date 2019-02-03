using System;

namespace Provausio.Practices.EventSourcing.Aggregate
{
    public interface IAggregateEvent
    {
        Guid Id { get; set; }

        DateTimeOffset TimeStamp { get; set; }

        string RootId { get; set; }

        long Version { get; set; }

        string Type { get; }

        AggregateEventMetadata Metadata { get; set; }
    }
}