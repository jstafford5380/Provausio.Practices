using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Provausio.Practices.EventSourcing.Aggregate
{
    public abstract class AggregateRoot
    {
        private readonly List<IAggregateEvent> _uncommittedEvents = new List<IAggregateEvent>();
        private readonly Dictionary<Type, Delegate> _eventHandlers = new Dictionary<Type, Delegate>();

        /// <summary>
        /// The Aggregate ID
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Version number of the aggregate.
        /// </summary>
        public long Version { get; internal set; } = -1;

        /// <summary>
        /// A list of uncommitted events.
        /// </summary>
        public IReadOnlyCollection<IAggregateEvent> UncommittedEvents =>
            new ReadOnlyCollection<IAggregateEvent>(_uncommittedEvents);

        /// <summary>
        /// Initializes a new instance of <see cref="AggregateRoot"/>
        /// </summary>
        protected AggregateRoot() => RegisterHandlers(); // should be safe despite virtual call

        /// <summary>
        /// Sets the ID of the aggregate root. Only use this when creating a new aggregate or
        /// when beginning a replay.
        /// </summary>
        /// <param name="id"></param>
        public void SetRootId(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Replays an event stream and brings the object up to state.
        /// </summary>
        /// <param name="events">A stream of events that will be replayed through the aggregate.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void LoadFromStream(IEnumerable<IAggregateEvent> events)
        {
            foreach (var e in events)
            {
                if (e.Version != 0 && e.RootId != Id) // version 0 is usually the one that sets the root id so let it pass
                    throw new InvalidOperationException("The event's rootId does not match the id of this aggregate root. Your data may be corrupted. Check stream.");

                if (e.Version <= Version && Version != -1)
                    throw new InvalidOperationException(
                        $"Incoming event version ({e.Version}) is <= root version ({Version}). This is probably due to corrupted data.");

                if(e.Version > Version + 1)
                    throw new InvalidOperationException("Incoming event version more than +1 of the current aggregate version. An event may have been missed somehow and the data may be corrupted.");

                Apply(e);
                Version = e.Version;
            }
        }

        /// <summary>
        /// Convenience method creates a properly constructed instance of a non-root entity.
        /// </summary>
        /// <typeparam name="T">The event type that will be registered.</typeparam>
        /// <returns></returns>
        protected T CreateEntity<T>(Func<T, string> idFactory = null)
            where T : Entity, new()
        {
            if (idFactory == null)
                idFactory = x => Guid.NewGuid().ToString();

            var instance = Activator.CreateInstance<T>();
            instance.SetRoot(this);
            instance.RegisterHandlers(this);
            instance.Id = idFactory(instance);
            return instance;
        }

        /// <summary>
        /// Registers handlers. To implement this, call <see cref="RegisterHandler{T}"/> for each event type (<see cref="IAggregateEvent"/>)
        /// </summary>
        protected abstract void RegisterHandlers();

        /// <summary>
        /// Raises an event and then invokes handlers.
        /// </summary>
        /// <param name="constructor">Constructs the event.</param>
        /// <typeparam name="T"></typeparam>
        public void Raise<T>(Action<T> constructor)
            where T : IAggregateEvent, new()
        {
            var instance = AggregateEvent.Create<T>();
            constructor(instance);
            Raise(instance);
        }

        private void Raise<T>(T e) where T : IAggregateEvent
        {
            e.RootId = Id;
            _uncommittedEvents.Add(e);

            Apply(e);
        }

        private void Apply<T>(T e)
            where T : IAggregateEvent
        {
            if (!TryInvokeHandler(e, out var ex)) throw ex;
        }

        private bool TryInvokeHandler<T>(T e, out Exception ex)
        {
            try
            {
                var eventType = e.GetType();
                if (!_eventHandlers.ContainsKey(eventType))
                {
                    ex = new HandlerNotRegisteredException(
                        eventType, $"No handler is registered for type '{typeof(T)}'");

                }
                else
                {
                    _eventHandlers[eventType].DynamicInvoke(e);
                    ex = null;
                }
            }
            catch (Exception exception)
            {
                ex = exception;
            }

            return ex == null;
        }

        /// <summary>
        /// Registers a handler.
        /// </summary>
        /// <param name="handler"></param>
        /// <typeparam name="T"></typeparam>
        public void RegisterHandler<T>(Action<T> handler)
            where T : IAggregateEvent => _eventHandlers.Add(typeof(T), handler);

        /// <summary>
        /// Clears uncommitted events.
        /// </summary>
        public void ClearEvents() => _uncommittedEvents.Clear();
    }
}