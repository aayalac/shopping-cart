using Polly;
using Polly.Registry;
using ShoppingCart.Api.Shared.Domain.Models;

namespace ShoppingCart.Api.Shared.Networking.CatalogoApi;

public sealed class CatalogoApiClient(
    ILoggerFactory loggerFactory,
    CatalogoApiService catalogoApiService,
    ResiliencePipelineProvider<string> pipelineProvider
) 
: ICatalogoApiClient
{
    private readonly ILoggerFactory _loggerFactory = loggerFactory;
    private readonly CatalogoApiService _catalogoApiService = catalogoApiService;
    private readonly ResiliencePipelineProvider<string> _pipelineProvider = pipelineProvider;
    public async Task<Catalogo?> GetProductByCodeAsync(string code, CancellationToken cancellationToken)
    {
        var _logger = _loggerFactory.CreateLogger("RetryLog");

        var policy = Policy.Handle<ApplicationException>()
                    .WaitAndRetryAsync(3, retryAttemp => {
                        _logger.LogInformation($"Intento : {retryAttemp}");
                        var timeToRetry = TimeSpan.FromSeconds(Math.Pow(2, retryAttemp));
                        return timeToRetry;
                    });

        var product = await policy.ExecuteAsync(() 
                    => _catalogoApiService.GetCatalogoByCodeAsync(code, cancellationToken));
    
        return product;
    }

    public async Task<IEnumerable<Catalogo>> GetProductsAsync(
        CancellationToken cancellationToken
    )
    {
        var _logger = _loggerFactory.CreateLogger("RetryLog");
        var pipeline = _pipelineProvider
        .GetPipeline<IEnumerable<Catalogo>>("catalogo-products");

        var products = await pipeline.ExecuteAsync(
            async token 
                => 
                await _catalogoApiService.GetProductsAsync(token), 
                cancellationToken
            );


        return products;
    }
}