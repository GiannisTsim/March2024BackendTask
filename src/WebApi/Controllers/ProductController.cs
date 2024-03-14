using March2024BackendTask.Core.Product.Abstractions;
using March2024BackendTask.Core.Product.Models;
using March2024BackendTask.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace March2024BackendTask.WebApi.Controllers;

[ApiController]
[Route("/api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _repository;

    public ProductController(IProductRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAsync()
    {
        var products = await _repository.GetAsync();
        return Ok(products);
    }

    [HttpGet("{productNo:int}")]
    public async Task<ActionResult<Product?>> GetDetailsAsync([FromRoute] int productNo)
    {
        var product = await _repository.FindByProductNoAsync(productNo);
        return product != null ? Ok(product) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Product?>> AddAsync([FromBody] ProductAddRequest request)
    {
        var command = new ProductAddCommand(request.Name!, request.Description!, (decimal)request.Price!);
        var productNo = await _repository.AddAsync(command);
        var product = await _repository.FindByProductNoAsync(productNo);
        return CreatedAtAction(nameof(GetDetailsAsync), new { productNo }, product);
    }

    [HttpPut("{productNo:int}")]
    public async Task<ActionResult<Product?>> ModifyAsync(
        [FromRoute] int productNo,
        [FromBody] ProductModifyRequest request
    )
    {
        var command = new ProductModifyCommand
        (
            productNo,
            (DateTimeOffset)request.UpdatedDtm!,
            request.Name!,
            request.Description!,
            (decimal)request.Price!
        );
        await _repository.ModifyAsync(command);
        return NoContent();
    }

    [HttpDelete("{productNo:int}")]
    public async Task<ActionResult<Product?>> DropAsync(
        [FromRoute] int productNo,
        [FromBody] ProductDropRequest request
    )
    {
        var command = new ProductDropCommand(productNo, (DateTimeOffset)request.UpdatedDtm!);
        await _repository.DropAsync(command);
        return NoContent();
    }
}