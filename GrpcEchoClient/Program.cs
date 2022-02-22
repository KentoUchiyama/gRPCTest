using Grpc.Core;
using Grpc.Net.Client;
using GrpcEcho;
using Microsoft.Extensions.Configuration;

// gRPCサーバアドレス設定
var channelAddress = "https://localhost:10002";
// キープアライブ設定（秒）
var channelKeepAlivePingDelay = 1;
var channelKeepAlivePingTimeout = 1;
// リクエストタイムアウト設定（ミリ秒）
var defaultTimeout = 5000;

// gRPCチャンネルを作成する（gRPCチャンネルはコネクションプールなので再利用する）
using var channel = GrpcChannel.ForAddress(channelAddress, new GrpcChannelOptions
{
    HttpHandler = new SocketsHttpHandler()
    {
        // 再利用可能と見なされるプールで接続がアイドル状態でいられる時間
        PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
        // キープアライブ設定（秒）
        KeepAlivePingDelay = TimeSpan.FromSeconds(channelKeepAlivePingDelay),
        KeepAlivePingTimeout = TimeSpan.FromSeconds(channelKeepAlivePingTimeout),
        // すべての既存の接続で同時実行ストリームの最大数に達したときに、同じサーバーに対して追加の HTTP/2 接続を確立できるか
        EnableMultipleHttp2Connections = true,
        // SSL設定
        SslOptions = new()
        {
            // SSL証明書の検証をスキップする
            RemoteCertificateValidationCallback = delegate { return true; }
        },
    }
});
var client = new EchoServer.EchoServerClient(channel);

for (int i = 0; i < 10; i++)
{
    EchoMessage request = new() { Message = $"message {i + 1}" };
    Console.WriteLine($"SEND {request.Message}");

    try
    {
        EchoMessage response = await client.EchoAsync(request, deadline: DateTime.UtcNow.AddMilliseconds(defaultTimeout));
        Console.WriteLine($"RECV {response.Message}");
    }
    catch (RpcException ex)
    {
        Console.WriteLine($"!!!ERROR StatusCode=[{ex.StatusCode}] Message=[{ex.Message}]");
    }
}
Console.WriteLine($"Finish");

Console.ReadKey();
