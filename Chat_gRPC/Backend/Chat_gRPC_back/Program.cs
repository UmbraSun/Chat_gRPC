using Chat_Database;
using Chat_Database.Models;
using Chat_gRPC_back;
using Chat_gRPC_back.Auth;
using Chat_gRPC_back.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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

builder.Services.AddIdentity<ChatUser, IdentityRole>()
    .AddEntityFrameworkStores<ChatDbContext>()
    .AddDefaultTokenProviders();

TokenParameters tokenParams = new();

builder.Services.AddSingleton(tokenParams);
builder.Services.AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = true;
        o.SecurityTokenValidators.Add(new ChatJWTValidator(tokenParams));
    });
// easy password for testing. Don't use it...
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
});
// options for CORS
builder.Services.AddCors(o => o.AddPolicy(
    "AllowAll", builderPolicy =>
    {
        builderPolicy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("Grpc-Status", "Grps-Message", "Grps-Encoding", "Grps-Accept-Encoding");
    }));
builder.Services.AddAuthorization();

builder.Services.AddSingleton<ChatRoomState>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(options =>
{
    // Configure the HTTP request pipeline.
    app.MapGrpcService<ChatRoomService>().EnableGrpcWeb();
    app.MapGrpcService<AccountService>().EnableGrpcWeb();

    app.MapFallbackToFile("index.html");
});
app.Services.GetService<ChatDbContext>().Database.EnsureCreated();
app.Run();


// maybe there are some prop from config parametrs, but I hope you will forgive me for the hardcode
public class TokenParameters
{
    public string Issuer => "issuer";
    public string Audience => "audience";
    public string SecretKey => "secretKeysecretKeysecretKey";

    public DateTime Expire => DateTime.Now.AddDays(1);
}