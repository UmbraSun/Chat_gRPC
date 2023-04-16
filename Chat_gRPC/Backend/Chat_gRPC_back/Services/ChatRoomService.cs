using Chat_Database;
using Chat_Database.Models;
using Chat_Protos;
using Grpc.Core;

namespace Chat_gRPC_back.Services
{
    public class ChatRoomService : ChatRoom.ChatRoomBase
    {
        private ChatRoomState _state;
        private List<IServerStreamWriter<ChatMessage>> _listeners = new List<IServerStreamWriter<ChatMessage>>();

        public ChatRoomService(ChatRoomState state)
        {
            _state = state;
            _state.MessageSended += ChatRoomService_MessageSended;
        }

        public override async Task JoinChat(ChatRequest request, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
        {
            foreach(var chatMessage in _state.GetMessages())
            {
                await responseStream.WriteAsync(new ChatMessage { Message = chatMessage.ChatVal });
            }

            _listeners.Add(responseStream);

            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100);
            }

            _listeners.Remove(responseStream);
        }

        private async void ChatRoomService_MessageSended(string message)
        {
            foreach(var streamWriter in _listeners)
            {
                await streamWriter.WriteAsync(new ChatMessage
                {
                    Message = message
                });
            }
        }

        public override async Task<ChatRequest> Send(ChatMessage request, ServerCallContext context)
        {
            var message = new Message
            {
                ChatVal = request.Message
            };

            await _state.AddMessageAsync(message);

            return new ChatRequest();
        }
    }
}
