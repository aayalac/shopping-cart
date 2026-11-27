using System.Drawing;
using Catalogo.Application.Products.AllProducts;
using Catalogo.Application.Products.CreateProduct;
using Catalogo.Application.Products.SearchProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalogo.Api.Controllers.Products;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    
    private readonly ISender _sender;

    public ProductController(ISender sender)
    {
        _sender = sender;
    }

    //  http://localhost:5000/api/products/code/12324323
    [HttpGet("code/{value}")]
    public async Task<IActionResult> GetByCode(string value)
    {
        HttpContext context = HttpContext; 
        var query = new SearchProductQuery { Code = value, Context = context };
        var product = await _sender.Send(query);
        return Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        HttpContext context = HttpContext;
        var query = new AllProductQuery {Context = context};
        var productDTOs = await _sender.Send(query);
        return Ok(productDTOs);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ProductRequest request)
    {
        var command = new CreateProductCommand(
            request.Nombre, 
            request.Descripcion, 
            request.Precio,
            request.CategoryId
        );

        var resultado =  await _sender.Send(command);

        return Ok(resultado);
    }

}