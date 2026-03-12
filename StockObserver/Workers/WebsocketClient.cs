using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StockObserver.Workers;

public class WebsocketClient : BackgroundService
{
    private readonly ILogger<WebsocketClient> _logger;

    public WebsocketClient(ILogger<WebsocketClient> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var client = new ClientWebSocket();

        await client.ConnectAsync( new Uri("wss://fstream.binance.com/stream?streams=btcusdt@trade/ethusdt@trade/dogeusdt@trade"), CancellationToken.None);

        using Stream wsStream = WebSocketStream.Create(client, WebSocketMessageType.Text, ownsWebSocket: true);

        var buffer = new byte[1024];

        while (!stoppingToken.IsCancellationRequested & client.State == WebSocketState.Open)
        {
            //var read = await wsStream.ReadAsync(buffer, 0, buffer.Length);
            var bytesRead = await wsStream.ReadAsync(buffer, stoppingToken);
            if(bytesRead == 0)
            {
                Console.WriteLine("Connection closed by server");
                break;
            }

            var payload = Encoding.UTF8.GetString(buffer.AsSpan(0, bytesRead));
            var dto = JsonSerializer.Deserialize<TradeStreamDto>(payload);
            if(dto is not null)
            {
                _logger.LogInformation("{Symbol} \t {Price} \t {Time}", dto.Data.Symbol, dto.Data.Price,dto.Data.EventTime);
            }
        }

        _logger.LogInformation("WebSocket closed");
    }
}

public record TradeStreamDto
{
    [JsonPropertyName("stream")]
    public string Stream { get; init; } = default!;

    [JsonPropertyName("data")]
    public TradeDataDto Data { get; init; } = default!;
}

public record TradeDataDto
{
    [JsonPropertyName("e")]
    public string EventType { get; init; } = default!;

    [JsonPropertyName("E")]
    public long EventTime { get; init; }

    [JsonPropertyName("T")]
    public long TradeTime { get; init; }

    [JsonPropertyName("s")]
    public string Symbol { get; init; } = default!;

    [JsonPropertyName("t")]
    public long TradeId { get; init; }

    [JsonPropertyName("p")]
    public string Price { get; init; } = default!;

    [JsonPropertyName("q")]
    public string Quantity { get; init; } = default!;

    [JsonPropertyName("X")]
    public string OrderType { get; init; } = default!;

    [JsonPropertyName("m")]
    public bool IsBuyerMarketMaker { get; init; }
}
