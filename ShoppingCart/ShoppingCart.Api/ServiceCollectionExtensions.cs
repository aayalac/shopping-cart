using System.Data;
using System.Reflection;
using Dapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Api.Shared.Behaviors;
using ShoppingCart.Api.Shared.Data;
using ShoppingCart.Api.Shared.Exceptions;
using ShoppingCart.Api.Shared.Metrics;
using ShoppingCart.Api.Shared.Networking.CatalogoApi;
using ShoppingCart.Api.Shared.Persistence;
using ShoppingCart.Api.Shared.Slices;

namespace ShoppingCart.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplicationServices(
        this IServiceCollection services
    )
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        
        services.RegisterSlices();
        var currentAssembly = Assembly.GetExecutingAssembly();
        services.AddAutoMapper(currentAssembly);

        services.AddMediatR(m => {
            m.RegisterServicesFromAssemblies(currentAssembly)
            .AddOpenRequestPreProcessor(typeof(LoggingBehavior<>))
            .AddOpenBehavior(typeof(ModelValidationBehavior<,>))
            .AddOpenBehavior(typeof(HandlerPerformanceMetricBehavior<,>));
        });

        services.AddValidatorsFromAssembly(currentAssembly);

        services.AddSingleton<HandlerPerformanceMetric>();
        
        services.AddScoped<ICatalogoApiClient, CatalogoApiClient>();
        return services;
    }

    public static IServiceCollection RegisterPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration
        .GetConnectionString("SqliteShoppingCart") 
        ?? throw new ArgumentNullException(nameof(configuration));
        
        services.AddDbContext<ShoppingDbContext>(opt => 
        {
            opt.LogTo(Console.WriteLine, new[] {
                DbLoggerCategory.Database.Command.Name
            }, LogLevel.Information).EnableSensitiveDataLogging();

            opt.UseSqlite(connectionString).UseSnakeCaseNamingConvention();
        });

        services.AddSingleton<ISqlConectionFactory>(
            _ => new SqlConnectionFactory(connectionString)
        );

        SqlMapper.AddTypeHandler(new GuidOnlyTypeHandler());

        return services;
    }


}