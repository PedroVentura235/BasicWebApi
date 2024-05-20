using BasicWebApi.Domain.Base;
using Domain.Base;

namespace BasicWebApi.Domain.Entities;

public class Product : AuditableEntity, IAggregateRoot
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }

    public Product(string name, string description, decimal price)
    {
        Name = name;
        Description = description;
        Price = price;
    }

    public Product Update(string? name, string? description, decimal? price)
    {
        if (name is not null && !Name.Equals(name))
            Name = name;
        if (description is not null && !Description.Equals(description))
            Description = description;
        if (price is not null && price.HasValue && !Price.Equals(price))
            Price = price.Value;

        return this;
    }
}