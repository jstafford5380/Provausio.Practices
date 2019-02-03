using System;

namespace Provausio.Practices.EventSourcing.Aggregate
{
    public abstract class Entity
    {
        public string Id { get; set; }

        public abstract void RegisterHandlers(AggregateRoot root);

        public void SetRoot(AggregateRoot root)
        {
            Root = root;
        }

        protected AggregateRoot Root { get; private set; }

        protected void Raise<T>(Action<T> constructor)
            where T : IAggregateEvent, new()
        {
            Root.Raise(constructor);
        }
    }
}
