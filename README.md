# Xena Exchange official websocket client for .netstandard 2.0

For API documentation check out [Help Center](https://support.xena.exchange/support/solutions/folders/44000161002)


#### Install

Add to .csproj:
```xml
<PackageReference Include="XenaExchange.Client.Websocket" Version="0.1.0" />
```


#### Market Data example

```csharp
var options = new MarketDataWsClientOptions { Uri = "wss://api.xena.exchange/ws/market-data" };
var serializer = new FixSerializer();
var logger = CreateLogger(); // Any logger implementation returning ILogger abstraction.
var wsClient = new MarketDataWsClient(options, serializer, logger);

// Disconnections handling
wsClient.OnDisconnect.Subscribe(async info =>
{
	// Don't reconnect here in a loop
	// OnDisconnect will fire on each WsClient.ConnectAsync() failure
	var reconnectInterval = TimeSpan.FromSeconds(5);
	try
	{
		await Task.Delay(reconnectInterval).ConfigureAwait(false);
		await info.WsClient.ConnectAsync().ConfigureAwait(false);
		logger.LogInformation("Reconnected");

		// Reubscribe on all streams after reconnect
	}
	catch (Exception ex)
	{
		logger.LogError(ex, $"Reconnect attempt failed, trying again after {reconnectInterval.ToString()}");
	}
});

await wsClient.ConnectAsync().ConfigureAwait(false);

// Subscribe on dom stream.
var streamId = var symbol = "BTC/USDT";
var timeframe = CandlesTimeframe.Timeframe1m;
var streamId = await wsClient.SubscribeCandlesAsync(symbol, timeframe, async (client, message) =>
{
	switch (message)
	{
		case MarketDataRequestReject reject:
			logger.LogWarning($"Market data candles request for {symbol}:{timeframe} was rejected: {reject.RejectText}");
			break;
		case MarketDataRefresh refresh:
			var updateType = refresh.IsSnapshot() ? "snapshot" : "update";
			logger.LogInformation($"Received candles {updateType}: {refresh}");
			break;
		default:
			logger.LogWarning($"Message of type {message.GetType().Name} not supported for candles stream");
			break;
	}
	await Task.Delay(10).ConfigureAwait(false); // Any async action inside handler.
}, throttlingMs: ThrottlingMs.Candles.Throttling1s).ConfigureAwait(false);

// Unsubscribe from dom stream later.
await Task.Delay(5000).ConfigureAwait(false);
await wsClient.Unsubscribe(streamId).ConfigureAwait(false);
```

#### Trading Example

Register an account with [Xena](https://trading.xena.exchange/registration). Generate an API Key and assign relevant permissions.
	
```csharp
var spotAccountId = 1;
var marginAccountId = 2;
var options = new TradingWsClientOptions()
{
	Uri = "wss://api.xena.exchange/ws/trading",
	Accounts = new List<long> { spotAccountId, marginAccountId },
	ApiKey = "TO_FILL",
	ApiSecret = "TO_FILL",
};

var serializer = new FixSerializer();
var logger = CreateLogger(); // Any logger implementation returning ILogger abstraction.
var wsClient = new TradingWsClient(options, serializer, logger);

wsClient.OnDisconnect.Subscribe(async info =>
{
	// Don't reconnect here in a loop
	// OnDisconnect will fire on each wsClient.ConnectAndLogonAsync() failure
	var reconnectInterval = TimeSpan.FromSeconds(5);
	try
	{
		await Task.Delay(reconnectInterval).ConfigureAwait(false);
		var response = await info.WsClient.ConnectAndLogonAsync().ConfigureAwait(false);
		logger.LogInformation($"Reconnected, logon response: {response}");

		// Don't call again Listen methods after reconnect
	}
	catch (Exception ex)
	{
		logger.LogError(ex, $"Reconnect attempt failed, trying again after {reconnectInterval.ToString()}");
	}
});

var logonResponse = await wsClient.ConnectAndLogonAsync().ConfigureAwait(false);
logger.LogInformation($"Connected, logon response: {logonResponse}");

wsClient.Listen<ExecutionReport>((client, er) =>
{
	logger.LogInformation($"Received execution report: {er}");
	return Task.CompletedTask;
});

await wsClient.NewMarketOrderAsync(
	"market-order-client-id",
	"BTC/USDT",
	Side.Sell,
	0.01M,
	SpotAccountId).ConfigureAwait(false);
```

For more examples check out "examples" folder.
