using System.Collections.Generic;
using System.Threading.Tasks;

namespace Achievements.Repositories
{
    public interface IRepository<T, TKey> where T : class
    {
        Task<TKey> Add(T item);
        Task<IEnumerable<T>> GetAll();
        Task<T> GetByID(TKey id);
        Task Update(T item);
        Task Delete(TKey id);
    }
}