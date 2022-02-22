using Grpc.Core;

namespace GrpcEcho.Services
{
    public class EchoService : EchoServer.EchoServerBase
    {
        private readonly ILogger<EchoService> _logger;

        public EchoService(ILogger<EchoService> logger)
        {
            _logger = logger;
        }

        public override async Task<EchoMessage> Echo(EchoMessage request, ServerCallContext context)
        {
            _logger.LogInformation("FROM {peer} RECV {message}", context.Peer, request.Message);

            await Task.Delay(1000);

            _logger.LogInformation("  TO {peer} SEND {message}", context.Peer, request.Message);

            return new EchoMessage { Message = request.Message };
        }
    }
}
