using FluentValidation;
using MediatR;

namespace ShoppingCart.Api.Shared.Behaviors;

public sealed class ModelValidationBehavior<TRequest, IResult>
(IEnumerable<IValidator<TRequest>> validators)
: IPipelineBehavior<TRequest, IResult>
where TRequest : IRequest<IResult>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;
    public async Task<IResult> Handle(
        TRequest request, 
        RequestHandlerDelegate<IResult> next, 
        CancellationToken cancellationToken
    )
    {
       
        if(!_validators.Any())
        {
            return await next();
        }
    
        var context = new ValidationContext<TRequest>(request);

        var validationResults = _validators.Select(x => x.Validate(context)).ToList();

        var groupValidations = validationResults
        .SelectMany(v => v.Errors)
        .GroupBy(e => e.PropertyName)
        .Select(g => new {
            PropertyName = g.Key,
            ValidationFailures = g.Select(v => new {v.ErrorMessage})
        }).ToList();

        if(groupValidations.Count !=0)
        {
            var validationDictionary = new Dictionary<string, string[]>();
            foreach(var group in groupValidations)
            {
                var errorMessages = group.ValidationFailures.Select(v => v.ErrorMessage);
                validationDictionary.Add(group.PropertyName, errorMessages.ToArray());
            }
            return (IResult)Results.ValidationProblem(validationDictionary);
        }

        return await next();
    }
}