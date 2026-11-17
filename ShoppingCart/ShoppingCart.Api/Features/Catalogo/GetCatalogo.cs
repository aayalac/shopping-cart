using ShoppingCart.Api.Shared.Networking.CatalogoApi;
using ShoppingCart.Api.Shared.Slices;

namespace ShoppingCart.Api.Features.Catalogo;

public sealed class GetCatalogo : ISlice
{

    public void AddEndpoint(IEndpointRouteBuilder app)
    {

        app.MapGet("api/catalogo", async(
            ILoggerFactory loggerFactory,
            ICatalogoApiClient catalogoClient,
            CancellationToken cancellationToken 
        ) => 
        {
            loggerFactory
            .CreateLogger("EndpointCatalogo-Get")
            .LogInformation("Catalogo de Productos");

            var result = await catalogoClient.GetProductsAsync(cancellationToken);
            return Results.Ok(result);
        });


    }

}
