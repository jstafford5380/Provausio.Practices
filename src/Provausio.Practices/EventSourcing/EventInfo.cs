using System;
using System.Linq;
using DAS.Infrastructure.Ext;
using Provausio.Practices.Validation.Assertion;

namespace Provausio.Practices.EventSourcing
{
    public abstract class EventInfo<T> : JsonEventData
    {
        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        /// <value>
        /// The type of the event.
        /// </value>
        public string EventType => GetType().Name;

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public EventMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        /// <value>
        /// The entity identifier.
        /// </value>
        public T EntityId { get; set; }

        /// <summary>
        /// Gets or sets the event identifier. Used for idempotent operations.
        /// </summary>
        /// <value>
        /// The event identifier.
        /// </value>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the version (the event sequence number).
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public long Version { get; set; }

        /// <summary>
        /// Gets or sets the reason for the event.
        /// </summary>
        /// <value>
        /// The reason.
        /// </value>
        public string Reason { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventInfo"/> class.
        /// </summary>
        protected EventInfo()
        {
            EventId = Guid.NewGuid();

            var nameSpaces = this.FindAttributes<EventNamespaceAttribute>();
            var namespaceNames = string.Join(";", nameSpaces.Select(ns => ns.Namespace));

            Metadata = new EventMetadata
            {
                FullTypeName = GetType().FullName,
                AssemblyQualifiedType = GetType().AssemblyQualifiedName,
                EventNamespaces = namespaceNames
            };
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:DAS.Practices.EventSourcing.EventInfo" /> class.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="reason">The reason.</param>
        protected EventInfo(T entityId, string reason)
            : this()
        {
            EntityId = Ensure.IsNotDefault(entityId, nameof(entityId));
            Reason = Ensure.IsNotNullOrEmpty(reason, nameof(reason));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EventInfo<T>);
        }

        protected bool Equals(EventInfo<T> other)
        {
            return other != null && EventId.Equals(other.EventId);
        }

        public override int GetHashCode()
        {
            // needed to allow setter for deserialization :/
            return EventId.GetHashCode();
        }
    }
}
