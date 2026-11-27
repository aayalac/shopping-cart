using Bogus;
using Catalogo.Domain.Categories;
using Catalogo.Domain.Products;
using Catalogo.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Api.Extensions;

public static class DataSeed
{
    public static async Task SeedCatalogoProduct(
        this IApplicationBuilder app
    )
    {
        using var scope = app.ApplicationServices.CreateScope();
        var service = scope.ServiceProvider;
        var loggerFactory = service.GetRequiredService<ILoggerFactory>();

        try
        {
            var context = service.GetRequiredService<CatalogoDbContext>();

            if (!context.Set<Category>().Any())
            {
                var categoryComputadora = Category.Create("COMPUTADORA");
                var categoryTelefono = Category.Create("TELEFONO");
                context.AddRange(new List<Category> { categoryTelefono, categoryComputadora });
                
                await context.SaveChangesAsync();
            }

            if (!context.Set<Product>().Any())
            {
                var computadora = await context.Set<Category>()
                                .Where(x => x.Name == "COMPUTADORA")
                                .FirstOrDefaultAsync();

                var telefono = await context.Set<Category>()
                                .Where(x => x.Name == "TELEFONO")
                                .FirstOrDefaultAsync();

                var faker = new Faker();
                List<Product> products = new();
                var defaultValue = 10000;
                for(var i = 1; i<=10 ; i++)
                {
                    products.Add(
                        Product.Create(
                            faker.Commerce.Product(),
                            100.00m,
                            faker.Commerce.ProductDescription(),
                            $"img_{i}.jpg",
                            faker.Hashids.Encode(defaultValue, i*1000),
                            i > 5 ? computadora!.Id : telefono!.Id
                        )
                    );
                }
                context.AddRange(products);
                await context.SaveChangesAsync();
            }

            
        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<CatalogoDbContext>();
            logger.LogError(ex, ex.Message);
        }


    }



}