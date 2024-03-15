using March2024BackendTask.Infrastructure;
using March2024BackendTask.WebApi.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers
           (options => { options.SuppressAsyncSuffixInActionNames = false; })
       .AddJsonOptions
           (options => { options.AllowInputFormatterExceptionMessages = builder.Environment.IsDevelopment(); });

builder.Services.AddInfrastructure();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();