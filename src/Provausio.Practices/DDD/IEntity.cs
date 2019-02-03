using System;

namespace Provausio.Practices.DDD
{
    /// <summary>
    /// When implemented, provides an interface for interacting with a basic aggregate that uses DTOs to store state.
    /// </summary>
    /// <typeparam name="TStateType"></typeparam>
    public interface IEntity<TStateType>
    {
        /// <summary>
        /// Returns the Aggregate's AggregateRoot ID
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Returns the current state of the aggregate.
        /// </summary>
        /// <returns></returns>
        TStateType GetState();

        /// <summary>
        /// Loads the current state of the entity.
        /// </summary>
        /// <param name="state">The object containing the aggregate' intended state.</param>
        void LoadState(TStateType state);
    }
}