using Api.Models;

namespace Api.Services;

/// <summary>
/// Contract for product catalogue operations.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Returns all products in the catalogue.
    /// </summary>
    /// <returns>Read-only list of <see cref="Product"/> records.</returns>
    IReadOnlyList<Product> GetAll();
}
