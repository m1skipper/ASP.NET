using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T>
        where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(Guid id);

        IQueryable<T> GetAll();

        void Add(T obj);

        void Delete(T obj);

        Task LoadCollectionAsync(T enity, string prop);

        Task SaveChangesAsync();
    }
}