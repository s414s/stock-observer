using Akka.Actor;

namespace StockObserver.Actors;

public sealed record TradeReceived(string Symbol, decimal Price, decimal Quantity, long TimeStamp, long TradeId);

public sealed class SymbolRouterActor : ReceiveActor
{
    public SymbolRouterActor()
    {
        Receive<TradeReceived>(msg =>
        {
            var childName = NormalizeActorName(msg.Symbol);

            var child = Context.Child(childName);
            if (Equals(child, ActorRefs.Nobody))
            {
                child = Context.ActorOf(
                    Props.Create(() => new SymbolTradeActor(msg.Symbol)),
                    childName);
            }

            child.Forward(msg);
        });
    }

    private static string NormalizeActorName(string symbol)
    {
        return symbol
            .ToUpperInvariant()
            .Replace("/", "_")
            .Replace("-", "_");
    }
}
