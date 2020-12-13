
using System;

namespace Catalog.Entities
{
    public record Item
    {
        // 自动引入包使用快捷键：command + .
        public Guid Id { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}