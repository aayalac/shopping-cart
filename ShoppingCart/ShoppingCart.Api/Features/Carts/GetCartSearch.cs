using System.Text;
using Dapper;
using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ShoppingCart.Api.Shared.Data;
using ShoppingCart.Api.Shared.Extensions;
using ShoppingCart.Api.Shared.Slices;

namespace ShoppingCart.Api.Features.Carts;

public sealed class GetCartSearch : ISlice
{

    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "api/carts/{code}/items",
            async (
                string code,
                IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                return await mediator.Send(new GetCartSearchQuery(code), cancellationToken);
            }
        );
    }

    public sealed class GetCartSearchQueryHandler(ISqlConectionFactory sqlConectionFactory)
    : IRequestHandler<GetCartSearchQuery, IResult>
    {
        private readonly ISqlConectionFactory _sqlConnectionFactory = sqlConectionFactory;
        public async Task<IResult> Handle(
            GetCartSearchQuery request,
            CancellationToken cancellationToken
        )
        {

            var search = "%" + request.Code.EncodeForLike() + "%";
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

            using var connection = _sqlConnectionFactory.CreateConnection();
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
                new { Code = request.Code, Search = search },
                splitOn: "ItemId"
            );

            var resultados = cartDictionary.Values.ToList();

            // if(!resultados.Any())
            // {
            //     return Results.NotFound();
            // }

            return Results.Ok(resultados);
        }
    }


    public sealed class GetCartSearchQuery(string code) : IRequest<IResult>
    {

        public string Code { get; } = code;

    }


    public sealed class GetCartSearchResponse(IEnumerable<CartDTO> carts)
    {
        public IEnumerable<CartDTO> carts { get; } = carts;
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