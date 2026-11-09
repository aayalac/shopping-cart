using Polly;
using Polly.Fallback;
using Polly.Simmy;
using ShoppingCart.Api;
using ShoppingCart.Api.Features.Catalogo;
using ShoppingCart.Api.Shared.Domain.Models;
using ShoppingCart.Api.Shared.Extensions;
using ShoppingCart.Api.Shared.Networking.CatalogoApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


builder.Services.AddHttpClient<CatalogoApiService>((serviceProvider, httpClient)=>
{
    httpClient.BaseAddress = new Uri("http://localhost:5000");
}).ConfigurePrimaryHttpMessageHandler(() => {
    return new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(5)
    };
}).SetHandlerLifetime(Timeout.InfiniteTimeSpan);

builder.Services
.AddResiliencePipeline<string, IEnumerable<Catalogo>>(
            "catalogo-products",
            pipelineBuilder => {
                pipelineBuilder
                .AddFallback(new FallbackStrategyOptions<IEnumerable<Catalogo>>
                    {
                        FallbackAction = _ 
                        => Outcome
                        .FromResultAsValueTask<IEnumerable<Catalogo>>(
                            Enumerable.Empty<Catalogo>()
                        )
                    }
                );
            });



builder.Services.AddOpenApi();
builder.Services.RegisterPersistenceServices(builder.Configuration);
builder.Services.RegisterApplicationServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // app.UseSwaggerUi(options => {
    //     options.Path = "/openapi";
    //     options.DocumentPath = "/openapi/v1.json";
    // });

    // app.UseReDoc(options => {
    //     options.Path ="/openapi";
    //     options.DocumentPath = "/openapi/v1.json";
    // });

    app.UseSwaggerUI(options => 
    {
        options.SwaggerEndpoint("/openapi/v1.json", "V1 Documentacion de Shopping Cart");
    });

}


GetCatalogo.AddEndpoint(app);
SearchProduct.AddEndpoint(app);

app.ApplyMigration();
app.Run();
