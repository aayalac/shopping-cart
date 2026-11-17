using ShoppingCart.Api.Shared.Networking.CatalogoApi;
using ShoppingCart.Api.Shared.Slices;

namespace ShoppingCart.Api.Features.Catalogo;

public sealed class SearchProduct : ISlice
{

    public void AddEndpoint(IEndpointRouteBuilder app)
    {

        app.MapGet("api/catalogo/{code}", async (
            string code,
            ILoggerFactory loggerFactory,
            ICatalogoApiClient catalogoClient,
            CancellationToken cancellationToken
        ) => {
            loggerFactory.CreateLogger("Endpoint-Catalogo-Code")
            .LogInformation("Buscar Producto por el codigo");

            var result = await catalogoClient
            .GetProductByCodeAsync(code, cancellationToken);

            return Results.Ok(result);

        });

    }


}