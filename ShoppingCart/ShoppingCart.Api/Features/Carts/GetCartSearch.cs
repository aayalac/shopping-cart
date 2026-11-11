using System.Text;
using Dapper;
using ShoppingCart.Api.Shared.Data;
using ShoppingCart.Api.Shared.Extensions;

namespace ShoppingCart.Api.Features.Carts;

public static class GetCartSearch
{

    public static void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "api/carts/{code}/items",
            async (
                string code,
                ILoggerFactory logger,
                ISqlConectionFactory sqlConnectionFactory,
                CancellationToken cancellationToken
            ) =>
            {
                logger.CreateLogger("EndpointCartSearch")
                .LogInformation("Consulta de busqueda de carrito por code");

                var search = "%" + code.EncodeForLike() + "%";
                var whereStatement = $" WHERE a.code LIKE @Search";

                var sql = new StringBuilder("""
                    SELECT 
                        a.id as Id,
                        a.code as Code,
                        b.id as ItemId,
                        b.name as Name
                    FROM shopping_carts a 
                        LEFT JOIN items b
                            ON a.Id=b.cart_id
                """
                );

                sql.AppendLine(whereStatement);

                var cartDictionary = new Dictionary<string, CartDTO>();

                using var connection = sqlConnectionFactory.CreateConnection();
                await connection.QueryAsync<CartDTO, ItemDTO, CartDTO>(
                    sql.ToString(),
                    (cart, item) =>
                    {
                        if (cartDictionary.TryGetValue(cart.Code, out var existingCart))
                        {
                            cart = existingCart;
                        }
                        else
                        {
                            cartDictionary.Add(cart.Code, cart);
                        }

                        cart.Items.Add(item);
                        return cart;
                    },
                    new { Code = code, Search = search },
                    splitOn: "ItemId"
                );

                var resultados = cartDictionary.Values.ToList();

                return Results.Ok(resultados);
            }
        );
    }


    public sealed class CartDTO
    {
        public required Guid Id { get; set; }
        public required string Code { get; set; }
        public List<ItemDTO> Items { get; set; } = [];
    }

    public sealed class ItemDTO
    {
        public required Guid ItemId { get; set; }
        public required string Name { get; set; }
    }

}