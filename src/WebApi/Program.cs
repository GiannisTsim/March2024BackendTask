using March2024BackendTask.Infrastructure;
using March2024BackendTask.WebApi.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers
           (options => { options.SuppressAsyncSuffixInActionNames = false; })
       .AddJsonOptions
           (options => { options.AllowInputFormatterExceptionMessages = builder.Environment.IsDevelopment(); });

builder.Services.AddInfrastructure();

builder.Services.AddExceptionHandler<CustomerExceptionHandler>();
builder.Services.AddExceptionHandler<ProductExceptionHandler>();
builder.Services.AddExceptionHandler<PurchaseExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();