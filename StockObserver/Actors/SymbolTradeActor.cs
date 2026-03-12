using Akka.Actor;

namespace StockObserver.Actors;

public sealed class SymbolTradeActor : ReceiveActor
{
    private readonly string _symbol;

    public SymbolTradeActor(string symbol)
    {
        _symbol = symbol;

        Receive<TradeReceived>(msg =>
        {
            Console.WriteLine( $"[Actor:{Self.Path.Name}] Symbol={msg.Symbol} Price={msg.Price} Qty={msg.Quantity} Id={msg.TradeId}");
        });
    }

    protected override void PreStart()
    {
        Console.WriteLine($"Created symbol actor for {_symbol} -> {Self.Path}");
        base.PreStart();
    }
}
