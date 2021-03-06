using System;

namespace Catalog.Api.Dtos
{
    public record ItemDto
    {
        // Init-only пропертис, новинка .net 5.0
        // после инициализации в конструкторе поменять нельзя
        public Guid Id { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
        public DateTimeOffset CreatedDate { get; init; }

    }
}