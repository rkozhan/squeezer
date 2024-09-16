using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;
using Squeezer.Client;
using Squeezer.Client.Interfaces;
using Squeezer.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

builder.Services.AddTransient<ILinkService, LinkApiProxy>();

builder.Services.AddRefitClient<ILinkApi>()
    .ConfigureHttpClient(httpClient =>
    {
        var apiUrl = builder.HostEnvironment.BaseAddress;
        httpClient.BaseAddress = new Uri(apiUrl);
    });

builder.Services.AddScoped<SessionStorageService>();

await builder.Build().RunAsync();
