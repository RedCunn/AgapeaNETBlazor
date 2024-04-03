using Agapea_Blazor.Client;
using Agapea_Blazor.Client.Models.Services;
using Agapea_Blazor.Client.Models.Services.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("Agapea_Blazor.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Agapea_Blazor.ServerAPI"));

//definicion de inyeccion de servicios propios...
builder.Services.AddScoped<IRestService, MyRestService>();
builder.Services.AddScoped<IStorageService, SubjectStorageService>();
//builder.Services.AddScoped<NavigatorService>();

await builder.Build().RunAsync();
