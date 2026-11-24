using System.Net;
using System.Text.Json;
using Newtonsoft.Json;
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
        var response = await _httpClient.GetAsync(
            $"api/products/code/{code}",
            cancellationToken
        );
        
        if(response.StatusCode == HttpStatusCode.NoContent)
        {
            return null;
        }

        var contentString = await response.Content.ReadAsStringAsync();
        var content =  JsonConvert.DeserializeObject<Catalogo>(contentString);
        
        // var content = await _httpClient.GetFromJsonAsync<Catalogo>(
        //     $"api/products/code/{code}", 
        //     cancellationToken
        // );
        return content;
    }

}