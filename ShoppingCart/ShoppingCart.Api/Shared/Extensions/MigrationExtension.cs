using Microsoft.EntityFrameworkCore;
using ShoppingCart.Api.Shared.Persistence;

namespace ShoppingCart.Api.Shared.Extensions;

public static class MigrationExtension
{

    public static async void ApplyMigration(this IApplicationBuilder app)
    {
        using(var scope = app.ApplicationServices.CreateScope())
        {
            var service = scope.ServiceProvider;
            var loggerFactory = service.GetRequiredService<ILoggerFactory>();

            try
            {
                var context = service.GetRequiredService<ShoppingDbContext>();
                await context.Database.MigrateAsync();
            }
            catch(Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "Error en la migracion");

            }


        }
    }

}