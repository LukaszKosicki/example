using api.Models.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Interfaces
{
    public interface IAsyncRepository<T> where T : BaseEntity
    {
        Task<T> GetById(int id);
        Task Add(T entity);
        Task Update(T entity);
        Task Remove(T entity);
        IQueryable<T> Entities { get; }
    }
}
