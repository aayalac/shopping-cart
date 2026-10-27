using ShoppingCart.Api.Shared.Domain.Models;

namespace ShoppingCart.Api.Shared.Networking.CatalogoApi;

public sealed class CatalogoApiClient(CatalogoApiService catalogoApiService) 
: ICatalogoApiClient
{
    private readonly CatalogoApiService _catalogoApiService = catalogoApiService;
    public Task<Catalogo> GetProductByCodeAsync(string code, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Catalogo>> GetProductsAsync(
        CancellationToken cancellationToken
    )
    {
        return await _catalogoApiService.GetProductsAsync(cancellationToken);
    }
}