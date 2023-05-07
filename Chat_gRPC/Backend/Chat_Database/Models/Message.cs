namespace Chat_Database.Models
{
    public class Message
    {
        public string Id { get; set; }

        public string ChatVal { get; set; }

        public ChatUser WriterUser { get; set; }

        public Message()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
