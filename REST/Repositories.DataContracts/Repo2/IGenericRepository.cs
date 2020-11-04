using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.DataContracts.Repo2
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
