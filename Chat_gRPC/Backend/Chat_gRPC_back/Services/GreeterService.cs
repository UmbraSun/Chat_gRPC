using Chat_Protos;
using Grpc.Core;

namespace Chat_gRPC_back.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Massage from " + request.Name
            });
        }

        public override async Task JoinChat(
            HelloRequest request, 
            IServerStreamWriter<HelloReply> responseStream, 
            ServerCallContext context)
        {
            var messages = new string[]
            {
                "Message1",
                "Message2",
                "Message3",
            };

            await responseStream.WriteAsync(new HelloReply
            {
                Message = string.Join(" ", messages)
            });

            int index = 4;

            while (!context.CancellationToken.IsCancellationRequested)
            {
                await responseStream.WriteAsync(new HelloReply
                {
                    Message = "Message " + index++
                });

                await Task.Delay(2000, context.CancellationToken);
            }
        }
    }
}