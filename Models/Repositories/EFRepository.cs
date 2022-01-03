using api.Models.Entities.Shared;
using api.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace api.Models.Repositories
{
    public class EFRepository<T> : IAsyncRepository<T> where T : BaseEntity
    {
        protected AppDbContext Context;
        private DbSet<T> entities;

        public EFRepository(AppDbContext context)
        {
            Context = context;
            entities = context.Set<T>();
        }

        public async Task<T> GetById(int id) => await entities
            .SingleOrDefaultAsync(e => e.Id == id);

        public async Task Add(T entity)
        {
            entity.CreatedDate = DateTime.Now;
            await entities.AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        public async Task AddRange(List<T> entity)
        {
            await entities.AddRangeAsync(entity);
            await Context.SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            entities.Update(entity);
            await Context.SaveChangesAsync();
        }

        public async Task Remove(T entity)
        {
            entities.Remove(entity);
            await Context.SaveChangesAsync();
        }

        public IQueryable<T> Entities =>
           entities;
    }
}
