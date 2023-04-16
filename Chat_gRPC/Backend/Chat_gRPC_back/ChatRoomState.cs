using Chat_Database;
using Chat_Database.Models;

namespace Chat_gRPC_back
{
    public class ChatRoomState
    {
        private ChatDbContext _context;
        // Forbidden Ninjutsu Technique: "Infernal Crutch"
        public event Action<string> MessageSended;

        public ChatRoomState(ChatDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Message> GetMessages() 
        {
            return _context.Messages; 
        }

        public async Task AddMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            MessageSended?.Invoke(message.ChatVal);
        }
    }
}
