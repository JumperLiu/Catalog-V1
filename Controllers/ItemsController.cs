using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Dtos;
using Catalog.Entities;
using Catalog.Extens;
using Catalog.Repositoies;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository repository;

        public ItemsController(IItemsRepository iRepository) => repository = iRepository;
        // GET /items
        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync() => (await repository.GetItemsAsync()).Select(item => item.AsDto());
        // GET /items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
        {
            var item = await repository.GetItemAsync(id);
            if (item is null) return NotFound();
            return item.AsDto();
        }
        // POST /items
        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateOrUpdateItemDto createItemDto)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = createItemDto.Name,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await repository.CreateItemAsync(item);
            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDto());
        }
        // PUT /items/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ItemDto>> UpdateItem(Guid id, CreateOrUpdateItemDto updateItemDto)
        {
            var existingItem = await repository.GetItemAsync(id);
            if (existingItem is null) return NotFound();
            var updateItem = existingItem with { Name = updateItemDto.Name, Price = updateItemDto.Price };
            await repository.UpdateItemAsync(updateItem);
            return CreatedAtAction(nameof(GetItemAsync), new { id = id }, updateItem.AsDto());
        }
        // DELETE /items/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteItem(Guid id)
        {
            var existingItem = await repository.GetItemAsync(id);
            if (existingItem is null) return NotFound();
            await repository.DeleteItemAsync(existingItem);
            return "OK";
        }
    }
}