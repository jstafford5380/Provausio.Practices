using System;

namespace Provausio.Practices.DDD
{
    public abstract class Entity<TStateType> : IEntity<TStateType>
        where TStateType : EntityState
    {
        /// <inheritdoc />
        /// <summary>
        /// Returns the Aggregate's AggregateRoot ID
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Implement this method by building and returning an instance of <see cref="EntityState"/> that represents the current state of the aggregate. The entity's ID will automatically be added to the state object.
        /// </summary>
        protected abstract TStateType BuildState();

        /// <inheritdoc />
        /// <summary>
        /// Returns the current state of the aggregate.
        /// </summary>
        public TStateType GetState()
        {
            var state = BuildState();
            state.EntityId = Id;
            return state;
        }

        /// <summary>
        /// Implement this method by applying object values or storing the state as an object directly. The entity's ID will automatically be set on the Aggregate instance.
        /// </summary>
        /// <param name="state"></param>
        protected abstract void SetState(TStateType state);

        /// <inheritdoc />
        /// <summary>
        /// Loads the current state of the entity.
        /// </summary>
        /// <param name="state">The object containing the aggregate's intended state.</param>
        public void LoadState(TStateType state)
        {
            Id = state.EntityId;
            SetState(state);
        }
    }
}
