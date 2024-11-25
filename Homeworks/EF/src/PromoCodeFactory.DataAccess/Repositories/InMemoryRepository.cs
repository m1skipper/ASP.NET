using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>
        : IRepository<T>
        where T : BaseEntity
    {
        protected IEnumerable<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public void Add(T obj)
        {
            throw new NotSupportedException();
        }

        public void Delete(T obj)
        {
            throw new NotSupportedException();
        }

        public IQueryable<T> GetAll()
        {
            throw new NotSupportedException();
        }

        public Task LoadCollectionAsync(T enity, string prop)
        {
            return Task.FromResult(0);
        }

        public Task SaveChangesAsync()
        {
            throw new NotSupportedException();
        }
    }
}