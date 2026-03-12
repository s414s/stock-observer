using Akka.Actor;

namespace StockObserver.Actors;

public sealed record TradeReceived(string Symbol, decimal Price, decimal Quantity);

public sealed class TradeActor : ReceiveActor
{
    public TradeActor()
    {
        Receive<TradeReceived>(msg =>
        {
            Console.WriteLine( $"[AKKA] Trade received -> Symbol: {msg.Symbol}, Price: {msg.Price}, Qty: {msg.Quantity}");
        });

        Receive<string>(msg =>
        {
            Console.WriteLine($"[AKKA] String message: {msg}");
        });
    }
}
