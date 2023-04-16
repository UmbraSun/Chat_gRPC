using Chat_Protos;
using Chat_Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton(services =>
{
    var baseUri = services.GetRequiredService<NavigationManager>().BaseUri;
    var channel = new ChannelBase.ForAddress(baseUri,
        new GrpcChannelOption
        {
            HttpHandler = new GrpcWebHandler(new HttpClientHandler())
        }); 
    return new Greeter.GreeterClient(channel);
});

await builder.Build().RunAsync();
