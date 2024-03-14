using March2024BackendTask.Core.Customer.Abstractions;
using March2024BackendTask.Core.Customer.Models;
using March2024BackendTask.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace March2024BackendTask.WebApi.Controllers;

[ApiController]
[Route("/api/customers")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerRepository _repository;

    public CustomerController(ICustomerRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetAsync()
    {
        var customers = await _repository.GetAsync();
        return Ok(customers);
    }

    [HttpGet("{customerNo:int}")]
    public async Task<ActionResult<Customer?>> GetDetailsAsync([FromRoute] int customerNo)
    {
        var customer = await _repository.FindByCustomerNoAsync(customerNo);
        return customer != null ? Ok(customer) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Customer?>> AddAsync([FromBody] CustomerAddRequest request)
    {
        var command = new CustomerAddCommand(request.FirstName!, request.LastName!, (decimal)request.DiscountPct!);
        var customerNo = await _repository.AddAsync(command);
        var customer = await _repository.FindByCustomerNoAsync(customerNo);
        return CreatedAtAction(nameof(GetDetailsAsync), new { customerNo }, customer);
    }

    [HttpPut("{customerNo:int}")]
    public async Task<ActionResult<Customer?>> ModifyAsync(
        [FromRoute] int customerNo,
        [FromBody] CustomerModifyRequest request
    )
    {
        var command = new CustomerModifyCommand
        (
            customerNo,
            (DateTimeOffset)request.UpdatedDtm!,
            request.FirstName!,
            request.LastName!,
            (decimal)request.DiscountPct!
        );
        await _repository.ModifyAsync(command);
        return NoContent();
    }

    [HttpDelete("{customerNo:int}")]
    public async Task<ActionResult<Customer?>> DropAsync(
        [FromRoute] int customerNo,
        [FromBody] CustomerDropRequest request
    )
    {
        var command = new CustomerDropCommand(customerNo, (DateTimeOffset)request.UpdatedDtm!);
        await _repository.DropAsync(command);
        return NoContent();
    }
}