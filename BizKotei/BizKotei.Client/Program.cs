using BizClientLib.Auth;
using BizClientLib.Services;
using BizClientLib.Services.Interfaces;
using BizKotei.Client.Services;
using BizKotei.Client.Services.Interfaces;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var services = builder.Services;

services.AddBlazoredLocalStorage();

services.AddScoped<IAppRequest, AppRequest>();

services.AddScoped(sp =>
new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

services.AddScoped<IDotGenkaSetting, DotGenkaSetting>();

services.AddAuthorizationCore();
services.AddCascadingAuthenticationState();

services.AddSingleton<AuthenticationStateProvider, BizClientAuthenticationStateProvider>();
services.AddSingleton<LayoutService>();

// ���O�Ƀw���X�`�F�b�N
var appRequest = builder.Build().Services.GetRequiredService<IAppRequest>();
var response = await appRequest.Get<string>("api/Healthz/get-healthz");

await builder.Build().RunAsync();
