using Chat_Database;
using Chat_gRPC_back;
using Chat_gRPC_back.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
// I'm sorry for this shit *ServiceLifetime.Singleton*
// it need for ChatRoomState
// I don't know how to write a chat :(
builder.Services.AddDbContext<ChatDbContext>(options => 
    options.UseSqlite("Data Source=chat.db"), ServiceLifetime.Singleton);
builder.Services.AddSingleton<ChatRoomState>();

var app = builder.Build();

//app.UseHttpsRedirection();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>().EnableGrpcWeb();
app.MapGrpcService<ChatRoomService>().EnableGrpcWeb();
app.MapFallbackToFile("index.html");

app.Services.GetService<ChatDbContext>().Database.EnsureCreated();
app.Run();
