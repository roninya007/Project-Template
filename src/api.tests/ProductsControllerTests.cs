using Api.Controllers;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.Tests;

public sealed class ProductsControllerTests
{
    private readonly Mock<IProductService> _serviceMock = new();
    private readonly ProductsController _sut;

    public ProductsControllerTests()
    {
        _sut = new ProductsController(_serviceMock.Object);
    }

    [Fact]
    public void GetAll_Always_ReturnsHttp200()
    {
        _serviceMock.Setup(s => s.GetAll()).Returns(SampleProducts());

        var result = _sut.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public void GetAll_Always_ReturnsNonEmptyList()
    {
        _serviceMock.Setup(s => s.GetAll()).Returns(SampleProducts());

        var result = _sut.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsAssignableFrom<IReadOnlyList<Product>>(ok.Value);
        Assert.NotEmpty(products);
    }

    private static IReadOnlyList<Product> SampleProducts() =>
        new[]
        {
            new Product { Id = 1, Name = "Wireless Mouse",      Price = 29.99m },
            new Product { Id = 2, Name = "Mechanical Keyboard", Price = 89.99m },
            new Product { Id = 3, Name = "USB-C Hub",           Price = 49.99m },
        };
}
