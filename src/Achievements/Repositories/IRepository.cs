using System.Collections.Generic;
using System.Threading.Tasks;

namespace Achievements.Repositories
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetEntities(string partitionKey = null);
        Task<T> GetEntity(string partitionKey, string rowKey);
        Task<T> InsertOrMergeEntity(T entity);
        Task InsertOrMergeEntities(IEnumerable<T> entities);
        Task DeleteEntity(T deleteEntity);
        Task DeleteEntities(IEnumerable<T> entities);
    }
}