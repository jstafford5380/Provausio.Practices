using System.Threading.Tasks;

namespace Provausio.Practices.EventSourcing
{
    public interface IRepository<TEntityType, TIdType> : IRepository<TEntityType, TIdType, EventInfo<TIdType>>
        where TEntityType : IAggregateRoot<TIdType, EventInfo<TIdType>>
    {
    }

    public interface IRepository<TEntityType, in TIdType, TSnapshotType>
        where TSnapshotType : EventInfo<TIdType>
        where TEntityType : IAggregateRoot<TIdType, TSnapshotType>
    {
        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        Task Save(TEntityType entity);

        /// <summary>
        /// Saves the specified entity and then gets the updated version.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntityType> SaveAndUpdate(TEntityType entity);

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="useSnapshot">if set to <c>true</c> [use snapshot].</param>
        /// <returns></returns>
        Task<TEntityType> GetById(TIdType id, bool useSnapshot);
    }
}
