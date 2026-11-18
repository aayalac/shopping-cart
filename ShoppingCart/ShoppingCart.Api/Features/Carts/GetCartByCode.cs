

using Dapper;
using MediatR;
using ShoppingCart.Api.Shared.Data;
using ShoppingCart.Api.Shared.Slices;

namespace ShoppingCart.Api.Features.Carts;

public sealed class GetCartByCode : ISlice
{
    
    public void AddEndpoint(IEndpointRouteBuilder app)
    {

        app.MapGet(
            "api/carts/{code}",
            async (
                string code,
                IMediator mediator,
                ILoggerFactory logger,
                CancellationToken cancellationToken
            ) => {

                logger.CreateLogger("EndpointCartByCode")
                .LogInformation("Get Carrito de compra por el codigo");
                
                return await mediator.Send(
                    new GetCartByCodeQuery(code), 
                    cancellationToken
                );
            }
        );
    }

    public sealed class GetCartByCodeQueryHandler(ISqlConectionFactory sqlConectionFactory)
    : IRequestHandler<GetCartByCodeQuery, IResult>
    {
        private readonly ISqlConectionFactory _sqlConnectionFactory = sqlConectionFactory;
        public async Task<IResult> Handle(
            GetCartByCodeQuery request, 
            CancellationToken cancellationToken
        )
        {
          
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

                using var connection = _sqlConnectionFactory.CreateConnection();

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
                    new { CartCode = request.Code },
                    splitOn: "ItemId"
                );
                
                if(cartDictionary.Count == 0)
                {
                    return Results.NotFound();
                }
                
                var resultados = cartDictionary.Values.ToList();
                return Results.Ok(new GetCartByCodeResponse(resultados));
        }
    }

    public sealed class GetCartByCodeQuery(string code) : IRequest<IResult>
    {
        public string Code {get;} = code;
    }


    public sealed class GetCartByCodeResponse(IEnumerable<CartDTO> carts)
    {
        public IEnumerable<CartDTO> Carts {get;} = carts;
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