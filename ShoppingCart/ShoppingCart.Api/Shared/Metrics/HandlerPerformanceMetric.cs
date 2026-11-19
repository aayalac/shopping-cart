using System.Diagnostics.Metrics;

namespace ShoppingCart.Api.Shared.Metrics;

public sealed class HandlerPerformanceMetric
{
    private readonly Counter<long> _milliSecondsElapsed;

    public HandlerPerformanceMetric(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("ShoppingCart.Store");
        _milliSecondsElapsed = meter.CreateCounter<long>(
            "shoppingcart.api.requesthandler.millisecondselapsed"
        );
    }

    public void MilliSecondsElapsed(long millisecondselapsed)
    => _milliSecondsElapsed.Add(millisecondselapsed);
}