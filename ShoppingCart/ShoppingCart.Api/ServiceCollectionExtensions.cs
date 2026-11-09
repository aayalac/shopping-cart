using Microsoft.EntityFrameworkCore;
using ShoppingCart.Api.Shared.Networking.CatalogoApi;
using ShoppingCart.Api.Shared.Persistence;

namespace ShoppingCart.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplicationServices(
        this IServiceCollection services
    )
    {
        services.AddScoped<ICatalogoApiClient, CatalogoApiClient>();
        return services;
    }

    public static IServiceCollection RegisterPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("SqliteShoppingCart");
        
        services.AddDbContext<ShoppingDbContext>(opt => 
        {
            opt.LogTo(Console.WriteLine, new[] {
                DbLoggerCategory.Database.Command.Name
            }, LogLevel.Information).EnableSensitiveDataLogging();

            opt.UseSqlite(connectionString).UseSnakeCaseNamingConvention();
        });

        return services;
    }


}