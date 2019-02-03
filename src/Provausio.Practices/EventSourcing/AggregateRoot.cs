using System;
using System.Collections;
using System.Collections.Generic;

namespace Provausio.Practices.EventSourcing
{
    public abstract class AggregateRoot<TIdType> : AggregateRoot<TIdType, EventInfo<TIdType>>, IAggregateRoot<TIdType>
    {
    }

    public abstract class AggregateRoot<TIdType, TSnapshotType> : IAggregateRoot<TIdType, TSnapshotType>
        where TSnapshotType : EventInfo<TIdType>
    {
        private readonly object _lock = new object();
        protected List<EventInfo<TIdType>> Changes = new List<EventInfo<TIdType>>();

        public TIdType Id { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Gets the version. The version is the last event version that was applied to the aggregate's stream by the repository that fetched it. 
        /// So if new events were added after the fetch, this number will not be affected.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public long Version { get; protected set; } = -1; // new

        /// <inheritdoc />
        /// <summary>
        /// Gets the uncommitted events.
        /// </summary>
        /// <returns></returns>
        public virtual ICollection GetUncommittedEvents()
        {
            return Changes;
        }

        /// <inheritdoc />
        /// <summary>
        /// Clears the uncommitted events.
        /// </summary>
        public virtual void ClearUncommittedEvents()
        {
            Changes.Clear();
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the snapshot.
        /// </summary>
        /// <returns></returns>
        public EventInfo<TIdType> GetSnapshot()
        {
            var snapshot = BuildSnapshot();
            snapshot.EntityId = Id;
            return snapshot;
        }

        public TSnapshotType GetState()
        {
            return (TSnapshotType) GetSnapshot();
        }

        protected abstract TSnapshotType BuildSnapshot();

        /// <inheritdoc />
        /// <summary>
        /// Marks the changes as committed.
        /// </summary>
        public virtual void MarkChangesAsCommitted()
        {
            ClearUncommittedEvents();
        }

        /// <inheritdoc />
        /// <summary>
        /// Loads from history.
        /// </summary>
        /// <param name="history">The history.</param>
        public void LoadFromHistory(IEnumerable<EventInfo<TIdType>> history)
        {
            foreach (var @event in history)
                RaiseEvent((dynamic)@event, false);
        }

        /// <summary>
        /// Applies the event to the aggregate root.
        /// </summary>
        /// <param name="event">The event.</param>
        protected void RaiseEvent(EventInfo<TIdType> @event)
        {
            @event.EntityId = Id;
            @event.Version = Version + 1;
            RaiseEvent(@event, true);
        }

        /// <summary>
        /// Constructs and applies the event to the aggregate root.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modifier"></param>
        protected void RaiseEvent<T>(Action<T> modifier)
            where T : EventInfo<TIdType>, new()
        {
            var e = new T();
            modifier(e);
            RaiseEvent(e);
        }

        private void RaiseEvent(EventInfo<TIdType> @event, bool isNew)
        {
            lock (_lock)
            {
                if (@event.Version <= Version)
                    throw new InvalidOperationException($"Unexpected version. Current version is {Version} but incoming event is {@event.Version}");

                this.AsDynamic().Apply(@event);

                // only apply changes if the Apply() was successful
                if (isNew)
                {
                    Changes.Add(@event);
                }
                else
                {
                    // in other words, we only want to set the version when we're
                    // reading from the stream but not when events are being applied
                    // by the aggregate. That way, when it is a new aggregate, the 
                    // version is always -1 (new).
                    Version = @event.Version;
                }
            }
        }
    }
}
