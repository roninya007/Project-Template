using Api.Models;

namespace Api.Services;

/// <summary>
/// Default implementation of <see cref="IProductService"/> backed by an in-memory list.
/// </summary>
public sealed class ProductService : IProductService
{
    private static readonly IReadOnlyList<Product> Catalogue = new[]
    {
        new Product { Id = 1, Name = "Wireless Mouse",    Price = 29.99m  },
        new Product { Id = 2, Name = "Mechanical Keyboard", Price = 89.99m },
        new Product { Id = 3, Name = "USB-C Hub",         Price = 49.99m  }
    };

    /// <inheritdoc />
    public IReadOnlyList<Product> GetAll() => Catalogue;
}
