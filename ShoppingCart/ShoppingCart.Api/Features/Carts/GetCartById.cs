using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Api.Shared.Domain.Entities;
using ShoppingCart.Api.Shared.Persistence;
using ShoppingCart.Api.Shared.Slices;

namespace ShoppingCart.Api.Features.Carts;

public sealed class GetCartById : ISlice
{

    public void AddEndpoint(IEndpointRouteBuilder app)
    {

        app.MapGet("api/carts", async(
            Guid cartId,
            IMediator mediator,
            CancellationToken cancellationToken
        ) => {
            return await mediator.Send(new GetCartByIdQuery(cartId), cancellationToken);
           
        });

    }

    public sealed class GetCartByIdQueryHandler(
        ShoppingDbContext dbContext,
        IMapper mapperMap
    )
    : IRequestHandler<GetCartByIdQuery, IResult>
    {
        
        private readonly ShoppingDbContext _context = dbContext;
        private readonly IMapper _mapper = mapperMap;
        
        public async Task<IResult> Handle(
            GetCartByIdQuery request, 
            CancellationToken cancellationToken
        )
        {
            
             var cart = await _context.Carts
            .Include(c => c.Items)
            .Where(c => c.Id == request.Id).FirstOrDefaultAsync();

            if (cart is null)
            {
                return Results.NotFound();
            }

            var cartDTO = _mapper.Map<CartDTO>(cart);

           return Results.Ok(cartDTO);

        }
    }


    public sealed class GetCartByIdQuery(Guid id) : IRequest<IResult>
    {
      public Guid Id {get;} = id;
    }


    public sealed class GetCartByIdResponse(CartDTO cart)
    {
        public CartDTO Cart {get;} = cart;
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