using ShoppingCart.Api.Shared.Domain.Models;

namespace ShoppingCart.Api.Shared.Networking.CatalogoApi;

public sealed class CatalogoApiService(HttpClient httpClient)
{

    private readonly HttpClient _httpClient = httpClient;
    private static readonly Random random = new();

    public async Task<IEnumerable<Catalogo>> GetProductsAsync(
        CancellationToken cancellationToken
    )
    {
        if(random.NextDouble() < 0.6)
        {
            throw new ApplicationException("La Api de catalogo no esta disponible");
        }
        
        var content = await _httpClient
        .GetFromJsonAsync<IEnumerable<Catalogo>>(
            "/api/products", 
            cancellationToken
        );

        return content!;
    }


    public async Task<Catalogo?> GetCatalogoByCodeAsync(
        string code, 
        CancellationToken cancellationToken
    )
    {
        if(random.NextDouble() < 0.6)
        {
            throw new ApplicationException(
                "La api de catalogo por code no esta disponible"
            );
        }

        var content = await _httpClient.GetFromJsonAsync<Catalogo>(
            $"api/products/code/{code}", 
            cancellationToken
        );
        return content;
    }

}