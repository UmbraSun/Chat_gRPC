using Chat_Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Chat_Database
{
    public class ChatDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public ChatDbContext()
        {

        }

        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {

        }
    }
}