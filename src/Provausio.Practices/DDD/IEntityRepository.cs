using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Provausio.Practices.DDD
{
    public class LinqResult<TEntityType, TStateType> : QueryResult<TEntityType>
        where TEntityType : Entity<TStateType> 
        where TStateType : EntityState
    {
        public Expression<Func<TStateType, bool>> Predicate { get; set; }
    }

    public class SqlResult<TEntityType, TStateType> : QueryResult<TEntityType>
        where TEntityType : Entity<TStateType>
        where TStateType : EntityState
    {
        public string SqlQuery { get; set; }
    }

    public class QueryResult<TAggregateType>
    {
        public IEnumerable<TAggregateType> Result { get; set; }

        public string Token { get; set; }

        public virtual bool HasMore => !string.IsNullOrEmpty(Token);

        public int PageSize { get; set; }
    }

    public interface IEntityRepository<TEntityType, TStateType>
        where TEntityType : Entity<TStateType> 
        where TStateType : EntityState
    {
        /// <summary>
        /// Attempts to retrive and aggregate by ID. Should return null when not found.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntityType> GetById(Guid id);

        /// <summary>
        /// When implemented, provides an interface through which the aggregate will be saved. The implementation should always use an "Upsert" type behavior.
        /// </summary>
        /// <param name="aggregate"></param>
        /// <returns></returns>
        Task Save(TEntityType aggregate);

        /// <summary>
        /// Lists all entities that meet the provided criteria, if any.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageSize"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<LinqResult<TEntityType, TStateType>> Find(
            Expression<Func<TStateType, bool>> predicate, 
            int pageSize = 100,
            string token = null);

        /// <summary>
        /// Executes the SQL query and returns matching elements.
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="pageSize"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<SqlResult<TEntityType, TStateType>> Find(
            string sqlQuery, 
            int pageSize = 100,
            string token = null);

        /// <summary>
        /// Executes a query using a token.
        /// </summary>
        /// <returns></returns>
        Task<LinqResult<TEntityType, TStateType>> Continue(
            LinqResult<TEntityType, TStateType> previousResult);

        /// <summary>
        /// Executes a query using a token.
        /// </summary>
        /// <returns></returns>
        Task<SqlResult<TEntityType, TStateType>> Find(
            SqlResult<TEntityType, TStateType> previousResult);

        /// <summary>
        /// Remove the aggregate from the data store.
        /// </summary>
        /// <param name="aggregate"></param>
        /// <returns></returns>
        Task Delete(TEntityType aggregate);
    }
}