using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Api.Shared.Domain.Entities;
using ShoppingCart.Api.Shared.Persistence;

namespace ShoppingCart.Api.Features.Carts;

public static class GetCartById
{

    public static void AddEndpoint(IEndpointRouteBuilder app)
    {

        app.MapGet("api/carts", async(
            Guid cartId,
            ILoggerFactory logger,
            ShoppingDbContext context,
            IMapper mapper,
            CancellationToken cancellationToken
        ) => {
            
            logger.CreateLogger("EndpointCartItemsById")
            .LogInformation("Consulta de ShoppingCart por Id");

            var cart = await context.Carts
            .Include(c => c.Items)
            .Where(c => c.Id == cartId).FirstOrDefaultAsync();

            if (cart is null)
            {
                return Results.NotFound();
            }

            var cartDTO = mapper.Map<CartDTO>(cart);

           return Results.Ok(cartDTO);
        });

    }


    public sealed class CartMapProfile : Profile
    {
        public CartMapProfile()
        {
            CreateMap<Cart, CartDTO>();
            CreateMap<Item, ItemDTO>();
        }
    }

    public sealed class CartDTO
    {
        public required Guid Id {get;set;}
        public required string Code {get;set;}
        public string? UserId {get;set;}
        public List<ItemDTO> Items {get;set;} = [];
    }

    public sealed class ItemDTO
    {
        public required Guid Id {get;set;}
        public required string Code {get;set;}
        public string? ImageUrl {get;set;}
        public required decimal Price {get;set;}
        public required int Quantity {get;set;}
        public required string Name {get;set;}
        public string? Description {get;set;}
        public required Guid CartId {get;set;}
    }


}