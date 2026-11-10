

using Dapper;
using ShoppingCart.Api.Shared.Data;

namespace ShoppingCart.Api.Features.Carts;

public static class GetCartByCode
{
    
    public static void AddEndpoint(IEndpointRouteBuilder app)
    {

        app.MapGet(
            "api/carts/{code}",
            async (
                string code,
                ILoggerFactory logger,
                ISqlConectionFactory sqlConnectionFactory,
                CancellationToken cancellationToken
            ) => {

                logger.CreateLogger("EndpointCartByCode")
                .LogInformation("Get Carrito de compra por el codigo");

                const string sql = """
                    SELECT 
                       a.id as Id,
                       a.code as Code,
                       a.user_id as UserId,
                       b.id as ItemId,
                       b.name as Name,
                       b.description as Description
                    FROM shopping_carts a 
                        LEFT JOIN items b
                        ON a.id=b.cart_id
                    WHERE a.code=@CartCode
                """;

                var cartDictionary = new Dictionary<string, CartDTO>();

                using var connection = sqlConnectionFactory.CreateConnection();

                await connection.QueryAsync<CartDTO, ItemDTO, CartDTO >(
                    sql, 
                    (cart, item) => {
                        if(cartDictionary.TryGetValue(cart.Code!, out var existingCart))
                        {
                            cart = existingCart;
                        }
                        else
                        {
                            cartDictionary.Add(cart.Code!, cart);
                        }
                        cart.Items.Add(item);

                        return cart;
                    },
                    new { CartCode = code },
                    splitOn: "ItemId"
                );
                
                var resultados = cartDictionary.Values.ToList();
                return Results.Ok(resultados);
            }
        );
    }


    public sealed class CartDTO
    {
        public required Guid Id {get;set;}
        public required string Code {get;set;}
        public required string UserId {get;set;}
        public List<ItemDTO> Items {get; init;} = [];
    }

    public sealed class ItemDTO 
    {
        public required Guid ItemId {get;set;}
        public required string Name {get;set;}
        public string? Description {get;set;}
    }


}