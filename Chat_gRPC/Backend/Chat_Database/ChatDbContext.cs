using Chat_Database.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chat_Database
{
    public class ChatDbContext : IdentityDbContext<ChatUser>
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