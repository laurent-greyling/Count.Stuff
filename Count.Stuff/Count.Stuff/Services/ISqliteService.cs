using System.Collections.Generic;

namespace Count.Stuff.Services
{
    public interface ISqliteService<T>
    {
        /// <summary>
        /// Add Entity to the specified table
        /// </summary>
        /// <param name="entity"></param>
        void Add(T entity);

        /// <summary>
        /// Add range of entities to the specified table
        /// </summary>
        /// <param name="entities"></param>
        void AddRange(List<T> entities);

        /// <summary>
        /// Get list of entities
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> Get();
    }
}
