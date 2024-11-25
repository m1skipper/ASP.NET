using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EfRepository<T>
        : IRepository<T>
        where T : BaseEntity
    {
        protected DataContext Data { get; set; }

        public EfRepository(DataContext data)
        {
            Data = data;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var set = Data.Set<T>();
            return await set.ToListAsync();
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            var set = Data.Set<T>();
            return set.FirstOrDefaultAsync(i => id == i.Id);
        }

        public void Add(T obj)
        {
            var set = Data.Set<T>();
            set.Add(obj);
        }

        public void Delete(T obj)
        {
            var set = Data.Set<T>();
            set.Remove(obj);
        }

        public IQueryable<T> GetAll()
        {
            var set = Data.Set<T>();
            return set.AsQueryable();
        }

        public Task LoadCollectionAsync(T enity, string prop)
        {
            return Data.Entry(enity).Collection(prop).LoadAsync();
        }

        public Task SaveChangesAsync()
        {
            return Data.SaveChangesAsync();
        }
    }
}