using March2024BackendTask.Core.Common.Abstractions;
using March2024BackendTask.Core.Customer.Abstractions;
using March2024BackendTask.Core.Product.Abstractions;
using March2024BackendTask.Core.Purchase.Abstractions;
using March2024BackendTask.Core.Purchase.Models;
using March2024BackendTask.Infrastructure.Repositories;
using March2024BackendTask.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace March2024BackendTask.Infrastructure;

public static class ServiceCollectionExtensions
{
    private static IServiceCollection AddMessagingInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IMessageBus<PurchaseNotifyCommand>, InMemoryMessageBus<PurchaseNotifyCommand>>();
        return services;
    }

    private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<PurchaseNotificationService>();
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<ICustomerRepository, CustomerRepository>();
        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<IPurchaseRepository, PurchaseRepository>();
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddRepositories();
        services.AddMessagingInfrastructure();
        services.AddBackgroundServices();
        return services;
    }
}