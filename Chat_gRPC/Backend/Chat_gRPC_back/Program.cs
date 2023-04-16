using Chat_gRPC_back.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

//app.UseHttpsRedirection();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>().EnableGrpcWeb();
app.MapFallbackToFile("index.html");

app.Run();
