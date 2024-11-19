using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IndianRecipeAPI.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(string id);  // Nullable return type
        Task CreateAsync(T entity);
        Task UpdateAsync(string id, T entity);
        Task DeleteAsync(string id);
        Task<List<T>> GetByFilterAsync(FilterDefinition<T> filter);  // Method for filtering
    }
}
