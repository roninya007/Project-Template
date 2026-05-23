using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Exposes product catalogue endpoints.
/// </summary>
[ApiController]
[Route("api/v1/products")]
[Produces("application/json")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    /// <summary>
    /// Initialises a new <see cref="ProductsController"/>.
    /// </summary>
    /// <param name="productService">Service providing product data.</param>
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Returns all products in the catalogue.
    /// </summary>
    /// <returns>
    /// <list type="bullet">
    ///   <item><description>200 OK – body contains an array of <see cref="Product"/>.</description></item>
    /// </list>
    /// </returns>
    /// <response code="200">Product list returned successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Product>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var products = _productService.GetAll();
        return Ok(products);
    }
}
