using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Entities;

namespace Catalog.Repositoies
{
    public class InMemItemsRepository : IItemsRepository
    {
        private readonly List<Item> items = new()
        {
            new Item { Id = Guid.NewGuid(), Name = "初级血瓶", Price = 3.5m, CreatedDate = DateTimeOffset.UtcNow },
            new Item { Id = Guid.NewGuid(), Name = "中级血瓶", Price = 8.5m, CreatedDate = DateTimeOffset.UtcNow },
            new Item { Id = Guid.NewGuid(), Name = "高级血瓶", Price = 15.5m, CreatedDate = DateTimeOffset.UtcNow },
            new Item { Id = Guid.NewGuid(), Name = "初级铜剑", Price = 28.0m, CreatedDate = DateTimeOffset.UtcNow }
        };

        public async Task<IEnumerable<Item>> GetItemsAsync() => await Task.FromResult(items);

        public async Task<Item> GetItemAsync(Guid id) => await Task.FromResult(items.Where(item => item.Id == id).SingleOrDefault());

        public async Task CreateItemAsync(Item item)
        {
            items.Add(item);
            await Task.CompletedTask;
        }

        public async Task UpdateItemAsync(Item item)
        {
            items[this.items.FindIndex(existingItem => existingItem.Id == item.Id)] = item;
            await Task.CompletedTask;
        }

        public async Task DeleteItemAsync(Item item)
        {
            items.RemoveAt(this.items.FindIndex(existingItem => existingItem.Id == item.Id));
            await Task.CompletedTask;
        }
    }
}