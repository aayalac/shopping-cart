using AutoMapper;
using FluentValidation;
using MediatR;
using ShoppingCart.Api.Shared.Domain.Entities;
using ShoppingCart.Api.Shared.Networking.CatalogoApi;
using ShoppingCart.Api.Shared.Persistence;
using ShoppingCart.Api.Shared.Slices;

namespace ShoppingCart.Api.Features.Items;

public sealed class AddItem : ISlice
{
  public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        
        endpointRouteBuilder.MapPost(
            "api/carts/{cartId}/items",
            async (
                Guid cartId,
                AddItemRequest addItemRequest,
                IMediator mediator,
                CancellationToken cancellationToken
            ) => {
                var addItemCommand = new AddItemCommand(
                    cartId, 
                    addItemRequest.Code, 
                    addItemRequest.Quantity
                );

                return  await mediator.Send(addItemCommand, cancellationToken);
            }
        );

    }

    public sealed class AddItemRequest(string code, int quantity)
    {
        public string Code {get;} = code;
        public int Quantity {get;} = quantity;
    }

    public sealed class AddItemCommand(
        Guid cartId,
        string productCode,
        int qty
    ) : IRequest<IResult>
    {
        public Guid CartId {get;set;} = cartId;
        public string ProductCode {get;set;} = productCode;
        public int Quantity {get;set;}  = qty;
    }

    public sealed class AddItemCommandValidator : AbstractValidator<AddItemCommand>
    {
        public AddItemCommandValidator()
        {
            RuleFor(x => x.ProductCode)
            .MaximumLength(200)
            .NotEmpty()
            .WithMessage("Error de validacion en codigo de producto");

            RuleFor(x => x.Quantity)
            .NotEmpty()
            .WithMessage("La cantidad no puede ser cero");
        }
    }


    public sealed class AddItemCommandHandler(
        ICatalogoApiClient catalogoApiClient,
        ShoppingDbContext shoppingDbContext,
        IMapper mapper
    )
    : IRequestHandler<AddItemCommand, IResult>
    {
        private readonly ICatalogoApiClient _catalogoApiClient = catalogoApiClient;
        private readonly ShoppingDbContext _shoppingDbContext = shoppingDbContext;
        private readonly IMapper _mapper = mapper;
        public async Task<IResult> Handle(
            AddItemCommand request, 
            CancellationToken cancellationToken
        )
        {
            //1. Buscar un producto del catalogo - pasandole como parametro el code
            var producto = await _catalogoApiClient
            .GetProductByCodeAsync(request.ProductCode, cancellationToken);

            if(producto is null)
            {
                //throw new Exception("No se encontro el producto en la api externa");
                return Results.Problem(
                    title: "Es un mensaje de error",
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: $"El product code no existe en catalogo: {request.ProductCode}"
                );
            }

            //2. Crear un objeto de tipo item basado en la clase entidad Item
            var itemEntity = Item.Create(
                request.ProductCode,
                producto.ImageUrl!,
                request.Quantity,
                producto.Price ?? producto.Price!.Value,
                producto.Name!,
                producto.Description!,
                request.CartId
            );

            //3. Insertar el producto en la base de datos usando el SaveChangesAsync
            _shoppingDbContext.Items.Add(itemEntity);
            await _shoppingDbContext.SaveChangesAsync(cancellationToken);


            //4. Devolver un objeto result, que contenga la url que identifica el api
            //para obtener ese item por id

            return Results.Created(
                $"api/items/{itemEntity.Id}",
                _mapper.Map<ItemDTO>(itemEntity)
            );

        }
    }


    public sealed class ItemDTO
    {
        public required Guid Id {get;set;}
        public required string ProductCode {get;set;}

        public required string ImageUrl {get;set;}

        public required decimal Price {get;set;}

        public required int Quantity {get;set;}

        public required string Name {get;set;}

        public required string Description {get;set;}

        public required Guid CartId {get;set;}
    }

    public sealed class ItemMapProfileAfterCreation : Profile
    {
        public ItemMapProfileAfterCreation()
        {
            CreateMap<Item, ItemDTO>();
        }
    }

}