using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Repos
{
    public interface IGenericRepository <T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
    }
}
