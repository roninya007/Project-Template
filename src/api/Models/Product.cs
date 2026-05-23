namespace Api.Models;

/// <summary>
/// Represents a product in the catalogue.
/// </summary>
public sealed class Product
{
    /// <summary>Unique identifier.</summary>
    public int Id { get; init; }

    /// <summary>Display name of the product.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Price in the default currency.</summary>
    public decimal Price { get; init; }
}
