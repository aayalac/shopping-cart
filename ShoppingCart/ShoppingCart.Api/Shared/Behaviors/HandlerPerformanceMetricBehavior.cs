using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using ShoppingCart.Api.Shared.Metrics;

namespace ShoppingCart.Api.Shared.Behaviors;


public sealed class HandlerPerformanceMetricBehavior<TRequest, TResponse>(
    HandlerPerformanceMetric handlerPerformanceMetric
)
: IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer = new();

    private readonly HandlerPerformanceMetric 
    _handlerPerformanceMetric = handlerPerformanceMetric;
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken
    )
    {
        _timer.Start();
        var response = await next();
        _timer.Stop();

        _handlerPerformanceMetric.MilliSecondsElapsed(_timer.ElapsedMilliseconds);

        return response;
    }
}