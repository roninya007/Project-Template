using Api.Services;

namespace Api.Tests;

public sealed class ProductServiceTests
{
    private readonly ProductService _sut = new();

    [Fact]
    public void GetAll_Always_ReturnsExactlyThreeProducts()
    {
        var result = _sut.GetAll();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void GetAll_Always_ReturnsProductsWithNonEmptyNames()
    {
        var result = _sut.GetAll();

        Assert.All(result, p => Assert.False(string.IsNullOrWhiteSpace(p.Name)));
    }

    [Fact]
    public void GetAll_Always_ReturnsProductsWithPriceGreaterThanZero()
    {
        var result = _sut.GetAll();

        Assert.All(result, p => Assert.True(p.Price > 0m));
    }
}
