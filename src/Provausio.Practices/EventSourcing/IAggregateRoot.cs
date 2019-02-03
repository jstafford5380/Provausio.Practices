using System.Collections;
using System.Collections.Generic;

namespace Provausio.Practices.EventSourcing
{
    /// <summary>
    /// Entities contain an identity. All instances of <see cref="T:DAS.Practices.EventSourcing.IEntity" /> that possess the same ID are considered to be equal.
    /// </summary>

    public interface IEntity<T>
    {
        T Id { get; set; }
    }

    public abstract class Entity<T> : IEntity<T>
    {
        public T Id { get; set; }

        protected bool Equals(Entity<T> other)
        {
            return Id.Equals(other.Id);
        }

        public bool Equals(IEntity<T> other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Entity<T>)obj);
        }

        public static bool operator ==(Entity<T> left, Entity<T> right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if ((object)left == null || (object)right == null)
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(Entity<T> left, Entity<T> right)
        {
            if (ReferenceEquals(left, right))
                return false;

            if ((object)left == null || (object)right == null)
            {
                return true;
            }

            return left.Equals(right);
        }

        public override int GetHashCode()
        {
            // allowing this to facilitate factory create methods.
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Id.GetHashCode();
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Extension of <see cref="T:DAS.Practices.EventSourcing.IAggregateRoot" />. Introduced as a non-breaking change to add a more precise GetState method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TIdType"></typeparam>
    public interface IAggregateRoot<TIdType> : IAggregateRoot<TIdType, EventInfo<TIdType>> 
    {
    }

    public interface IAggregateRoot<TIdType, out TSnapshotType> : IEntity<TIdType> 
        where TSnapshotType : EventInfo<TIdType>
    {
        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        long Version { get; }

        /// <summary>
        /// Gets the uncommitted events.
        /// </summary>
        /// <returns></returns>
        ICollection GetUncommittedEvents();

        /// <summary>
        /// Clears the uncommitted events.
        /// </summary>
        void ClearUncommittedEvents();

        /// <summary>
        /// Marks the changes as committed.
        /// </summary>
        void MarkChangesAsCommitted();

        /// <summary>
        /// Loads from history.
        /// </summary>
        /// <param name="history">The history.</param>
        void LoadFromHistory(IEnumerable<EventInfo<TIdType>> history);

        /// <summary>
        /// Gets the snapshot.
        /// </summary>
        /// <returns></returns>
        EventInfo<TIdType> GetSnapshot();

        /// <summary>
        /// Gets the snapshot.
        /// </summary>
        /// <typeparam name="TSnapshotType"></typeparam>
        /// <returns></returns>
        TSnapshotType GetState();
    }
}
