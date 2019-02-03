using System;
using System.Linq;
using Provausio.Common.Ext;

namespace Provausio.Practices.EventSourcing.Aggregate
{
    public class AggregateEvent : IAggregateEvent
    {
        public Guid Id { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public string RootId { get; set; }

        public long Version { get; set; }

        public string Reason { get; set; }

        public string Type => GetType().Name;

        public AggregateEventMetadata Metadata { get; set; } = new AggregateEventMetadata();

        public static T Create<T>()
            where T : IAggregateEvent, new()
        {
            var instance = Activator.CreateInstance<T>();
            instance.Id = Guid.NewGuid();
            return instance;
        }
    }

    public class AggregateEventMetadata
    {
        public string FullTypeName { get; set; }

        public string AssemblyQualifiedType { get; set; }

        public string EventNamespaces => string.Join(";", this.FindAttributes<EventNamespaceAttribute>().Select(ns => ns.Namespace));
    }
}