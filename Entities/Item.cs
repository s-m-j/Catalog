namespace Catalog.Entities
{
    public record Item
    {
        // Init-only пропертис, новинка .net 5.0
        // после инициализации в конструкторе поменять нельзя
        public Guid Id { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
        public DateTimeOffset CreatedDate { get; init; }

    }
}