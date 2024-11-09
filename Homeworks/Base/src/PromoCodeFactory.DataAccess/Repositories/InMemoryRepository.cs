using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected List<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = new();
            Data.AddRange(data);
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<T>>(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task CreateAsync(T item)
        {
            int index = GetIndexById(item.Id);
            if (index >= 0)
                throw new ArgumentException($"Сущность с идентификатором {item.Id} уже существует");
            if (item.Id == Guid.Empty)
                item.Id = Guid.NewGuid();
            Data.Add(item);
            return Task.FromResult(0);
        }

        public Task DeleteAsync(Guid id)
        {
            int index = GetIndexById(id);
            if (index >= 0)
                Data.RemoveAt(index);
            return Task.FromResult(0);
        }

        public Task UpdateAsync(T item)
        {
            int index = GetIndexById(item.Id);
            if (index >= 0)
                Data[index] = item;
            return Task.FromResult(0);
        }

        private int GetIndexById(Guid id)
        {
            for(int i=0;i<Data.Count;i++)
            {
                if (Data[i].Id == id)
                    return i;
            }
            return -1;
        }
    }
}