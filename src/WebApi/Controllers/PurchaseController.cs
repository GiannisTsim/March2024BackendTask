using March2024BackendTask.Core.Common.Abstractions;
using March2024BackendTask.Core.Product.Exceptions;
using March2024BackendTask.Core.Purchase.Abstractions;
using March2024BackendTask.Core.Purchase.Exceptions;
using March2024BackendTask.Core.Purchase.Models;
using March2024BackendTask.WebApi.Filters;
using March2024BackendTask.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace March2024BackendTask.WebApi.Controllers;

[ApiController]
public class PurchaseController : ControllerBase
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IMessageBus<PurchaseNotifyCommand> _messageBus;

    public PurchaseController(IPurchaseRepository repository, IMessageBus<PurchaseNotifyCommand> messageBus)
    {
        _purchaseRepository = repository;
        _messageBus = messageBus;
    }

    // TODO: Add filtering & paging
    [HttpGet("/api/purchases")]
    public async Task<ActionResult<IEnumerable<PurchaseSimple>>> GetAsync()
    {
        var purchases = await _purchaseRepository.GetAsync();
        return Ok(purchases);
    }

    // TODO: Add filtering & paging
    [HttpGet("/api/customers/{customerNo:int}/purchases")]
    public async Task<ActionResult<IEnumerable<PurchaseSimple>>> GetByCustomerAsync([FromRoute] int customerNo)
    {
        var purchases = await _purchaseRepository.GetAsync(customerNo);
        return Ok(purchases);
    }

    [HttpGet("/api/customers/{customerNo:int}/purchases/{purchaseDtm}")]
    public async Task<ActionResult<PurchaseDetails>> GetDetailsAsync(
        [FromRoute] int customerNo,
        [FromRoute] DateTimeOffset purchaseDtm
    )
    {
        var purchase = await _purchaseRepository.GetDetailsAsync(customerNo, purchaseDtm);
        return purchase != null ? Ok(purchase) : NotFound();
    }

    [HttpPost("/api/customers/{customerNo:int}/purchases")]
    [SuppressModelStateInvalidFilter]
    public async Task<ActionResult<DataWithErrorsResponse<PurchaseDetails>>> AddAsync(
        [FromRoute] int customerNo,
        [FromBody] PurchaseAddRequest request
    )
    {
        // Respond with BadRequest if Items list is invalid, i.e. null or empty
        if (ModelState.ContainsKey(nameof(request.Items))) return ValidationProblem(ModelState);

        // Else attempt to create purchase with valid items
        var purchaseDtm = request.PurchaseDtm ?? DateTimeOffset.Now;
        for (var i = 0; i < request.Items!.Length; i++)
        {
            // Skip item if it contains validation errors
            if (!ModelState.FindKeysWithPrefix($"{nameof(request.Items)}[{i}]").IsNullOrEmpty()) continue;

            // Else attempt insert
            var item = request.Items[i];
            var itemCommand = new PurchaseItemAddCommand
            (
                customerNo,
                purchaseDtm,
                (int)item.ProductNo!,
                (int)item.Quantity!
            );
            try
            {
                // TODO: Consider parallel execution
                await _purchaseRepository.AddItemAsync(itemCommand);
            }
            catch (ProductNotFoundException ex)
            {
                ModelState.AddModelError($"{nameof(request.Items)}[{i}]", ex.Message);
            }
            catch (PurchaseDuplicateItemException ex)
            {
                ModelState.AddModelError($"{nameof(request.Items)}[{i}]", ex.Message);
            }
        }

        var purchase = await _purchaseRepository.GetDetailsAsync(customerNo, purchaseDtm);

        // Return BadRequest if all items where invalid and no purchase was created
        if (purchase == null) return ValidationProblem(ModelState);

        // Else return the crated purchase with the list of validation errors
        var errors = ModelState
                     .Where(kv => !kv.Value?.Errors.IsNullOrEmpty() ?? false)
                     .ToDictionary
                     (
                         kv => kv.Key,
                         kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                     );

        var response = new DataWithErrorsResponse<PurchaseDetails> { Data = purchase, Errors = errors };
        return CreatedAtAction(nameof(GetDetailsAsync), new { customerNo, purchaseDtm }, response);
    }

    [HttpPost("/api/customers/{customerNo:int}/purchases/{purchaseDtm}/submit")]
    public async Task<ActionResult<PurchaseSimple?>> SubmitAsync(
        [FromRoute] int customerNo,
        [FromRoute] DateTimeOffset purchaseDtm,
        CancellationToken cancellationToken
    )
    {
        var submitCommand = new PurchaseSubmitCommand(customerNo, purchaseDtm);
        await _purchaseRepository.AddSubmitAsync(submitCommand);

        var notifyCommand = new PurchaseNotifyCommand(customerNo, purchaseDtm);
        await _messageBus.WriteAsync(notifyCommand, cancellationToken);

        return Accepted(nameof(GetDetailsAsync), new { customerNo, purchaseDtm });
    }

    // TODO: POST 	/api/customers/{customerNo}/purchases/{purchaseDtm}  					    ?? (add content to existing purchase) ??
    // TODO: POST 	/api/customers/{customerNo}/purchases/{purchaseDtm}/products/               ?? (add specific product to existing purchase) ??
    // TODO: PUT 	/api/customers/{customerNo}/purchases/{purchaseDtm}  					    (update entire content of existing purchase)
    // TODO: PUT 	/api/customers/{customerNo}/purchases/{purchaseDtm}/products/{productNo}    (update purchase item quantity)
    // TODO: DELETE /api/customers/{customerNo}/purchases/{purchaseDtm}/products/{productNo}	(remove purchase item)
}