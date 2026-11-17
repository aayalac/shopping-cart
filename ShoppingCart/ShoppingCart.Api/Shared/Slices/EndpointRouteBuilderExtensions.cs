namespace ShoppingCart.Api.Shared.Slices;


public static class EndpointRouteBuilderExtensions
{

    public static IEndpointRouteBuilder MapSliceEndpoints(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        foreach(ISlice slc in endpointRouteBuilder.ServiceProvider.GetServices<ISlice>())
        {
            slc.AddEndpoint(endpointRouteBuilder);
        }

        return endpointRouteBuilder;
    }

}