using Chat_Database;
using Chat_Database.Models;
using Chat_Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;

namespace Chat_gRPC_back.Services
{
    public class ChatRoomService : ChatRoom.ChatRoomBase
    {
        private readonly UserManager<ChatUser> _userManager;
        private readonly ChatApp _chatApp;
        private readonly ILogger<ChatRoomService> _logger;
        private IServerStreamWriter<ChatMessage> _responseStream = null!;


        public ChatRoomService(UserManager<ChatUser> userManager,
            //ChatApp chatApp,
            ILogger<ChatRoomService> logger)
        {
            _userManager = userManager;
            //_chatApp = chatApp;
            _logger = logger;
        }

        public override async Task JoinChat(ChatRequest request,
            IServerStreamWriter<ChatMessage> responseStream,
            ServerCallContext context)
        {
            var user = await _userManager.GetUserAsync(context.GetHttpContext().User);

            var session = _chatApp.JoinUser(user);

            _responseStream = responseStream;

            session.NewMessageSended += SessionOnNewMessageSended;

            session.Init();

            try
            {
                await Task.Delay(int.MaxValue, context.CancellationToken);
            }
            catch (TaskCanceledException e)
            {
                _logger.Log(LogLevel.Information, e, "ChatRoomService.JoinChat.Cancelled");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, "ChatRoomService.JoinChat");
            }
            finally
            {
                session.NewMessageSended -= SessionOnNewMessageSended;

                session.Dispose();
                _responseStream = null!;
            }
        }

        public override async Task<ChatRequest> Send(ChatMessage request, ServerCallContext context)
        {
            var user = await _userManager.GetUserAsync(context.GetHttpContext().User);

            var chatMessage = new DomainModels.ChatMessage
            {
                Message = request.Message,
                User = user
            };

            await _chatApp.SendAsync(user, chatMessage);

            return new ChatRequest();
        }



        private async void SessionOnNewMessageSended(DomainModels.ChatMessage message)
        {
            await _responseStream.WriteAsync(new ChatMessage { Message = message.Message });
        }
    }
}