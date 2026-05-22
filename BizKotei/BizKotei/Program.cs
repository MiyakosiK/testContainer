using BizKotei.Components;
using BizClientLib.Services;
using BizClientLib.Services.Interfaces;
using BizServerLib.Auth;
using BizServerLib.Filters;
using BizServerLib.Interfaces;
using BizServerLib.Models;
using BizServerLib.Services;
using BizServerLib.Servicies;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

//�ݒ�̓ǂݍ��݁@�ݒ�t�@�C�������ݒ�̏��ɎQ��
IConfiguration config = builder.Configuration
//        .AddJsonFile("appsettings.json", optional: false, true)
#if DEBUG
        .AddJsonFile($"appsettings.{builder.Configuration["Environment"]}.json", optional: true, true)
#endif
        .AddEnvironmentVariables()
        .Build();

// Config�̏��𕔕��I�ɓo�^
// �S�Ă�ǂ݂��ނ͔̂�����Ȃ��߁B
services.Configure<AppSettingsModel.WebApi>(config.GetSection("WebApi"));
services.Configure<AppSettingsModel.Env>(config.GetSection("Env"));

// �T�[�o�[�T�C�h�ł͗��p����z��ł͂Ȃ����APrerender�̌��ˍ����ŃG���[���������邽�ߐݒ�
services.AddScoped<IAppRequest, AppRequest>();

// �T�[�o�[�T�C�h����WebApi�փR�[�������߂̃T�[�r�X
// �����I��HttpClient��Inject����邪�AAddHttpClient�ǉ���������O�̍s�Őݒ肪�K�v
// �����łȂ���΁ABaseAddress��Null������B
services.AddScoped<IWebApiRequest, WebApiRequest>();

// WebAPI���N�G�X�g�p
services.AddHttpClient<IWebApiRequest, WebApiRequest>(client =>
{
    client.BaseAddress = new Uri($"https://{config["WebApi:Host"]}");
});

services.AddHttpContextAccessor();

// Add services to the container.
services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();
services.AddControllers();


using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = factory.CreateLogger("Startup");
logger.LogInformation("=== CreateBuilder: {a} ===", "start");

//AWS�ݒ�����Q�Ƃ���
var awsOptions = config.GetAWSOptions();
services.AddDefaultAWSOptions(awsOptions);
logger.LogInformation("AWS Profile:{pro}", awsOptions.Profile);
logger.LogInformation("AWS Region:{reg}", awsOptions.Region?.DisplayName);
#if !DEBUG
    // �V�[�N���b�g�ނ�SSM����擾��Configuration���č\�z
    config = builder.Configuration.AddSystemsManager("/biz-order/").Build();  //Biz���i�ɂ���ĕύX
#endif
//Cognito�̔F�؃v���o�C�_���������i���󃍃O�A�E�g�ɂ̂ݎg�p
services.AddCognitoIdentity();

// DataProtection�L�[��AWS Systems Manager�ɕۑ�
services.AddDataProtection()
    .PersistKeysToAWSSystemsManager("/bizapp/dataprotection")
    .SetApplicationName("BizApp");

// Cookie�F�؂��\�z����
var ExpirationSpan = TimeSpan.FromMinutes(60 * 24); // �����Dynamo��Cookie�𓝈�

/// DynamoDB�ŃZ�b�V�����̕��U�L���b�V�����\�z����https://github.com/awslabs/aws-dotnet-distributed-cache-provider
/// https://ap-northeast-1.console.aws.amazon.com/dynamodbv2/home?region=ap-northeast-1#table?name=SessionStore
services.AddAWSDynamoDBDistributedCache(options =>
{
    options.TableName = "SessionStore";
    options.PartitionKeyName = "Session";
    options.TTLAttributeName = "Ttl";
    options.PartitionKeyPrefix = Assembly.GetEntryAssembly()?.GetName().Name;
    //options.UseConsistentReads = true;//�����ȃf�[�^�擾��L���ɂ���i�L���p�V�e�B���j�b�g�̃R�X�g���j
});
services.AddSingleton(new DistributedCacheEntryOptions()
{
    AbsoluteExpirationRelativeToNow = ExpirationSpan, // �L���b�V���̗L�������iTTL�j
});
services.AddSingleton<ITicketStore, DistributedCacheSessionStore>(); //ASP.NET���F�؏�Ԃ��Ǘ�����

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.HttpOnly = true; // js�����Cookie�A�N�Z�X���u���b�N
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.MaxAge = ExpirationSpan; //�Œ��L������
    options.Cookie.IsEssential = true;

    options.Events.OnRedirectToLogin = ctx =>
    {
        ctx.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});

// ���U�L���b�V���Ƃ���DynamoDB�����Ƃ����f�[�^�X�g�A��ݒ�
services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme).Configure<ITicketStore>((options, store) =>
{
    options.SessionStore = store;
});

services.AddAuthorization();

//Biz�����F�؃T�[�o��SSO�Ɋւ���ݒ�
services.AddSingleton(obj => new AuthSettingModel(
        new AuthSettingServer()
        {
            AuthorizeEndpoint = $"https://{config["SSO:AuthServerHost"]}/oauth/authorize",
            TokenEndpoint = $"https://{config["SSO:AuthServerHost"]}/oauth/token",
            TokenType = "Bearer",
            Issuer = $"https://cognito-idp.{config["AWS:Region"]}.amazonaws.com/{config["AWS:UserPoolId"]}",
            JwksUrl = $"https://cognito-idp.{config["AWS:Region"]}.amazonaws.com/{config["AWS:UserPoolId"]}/.well-known/jwks.json",
            ClientId = config["AWS:UserPoolClientId"] ?? "",
            UserPoolId = config["AWS:UserPoolId"] ?? "",
        },
        new AuthSettingClient()
        {
            ClientId = config["SSO:ClientId"] ?? "",
            Secret = config["SSO:Secret"] ?? "",
            RedirectUri = $"https://{config["SSO:CallbackHost"]}/callback",
            Scope = "openid email",
            ResponseType = "code",
            PkceHashSecret = config["SSO:PkceHashSecret"] ?? "",
        }
    ));

services.AddScoped<AuthorizeService>();

services.AddControllersWithViews(options =>
{
    options.Filters.Add<ExceptionFilter>();
});

// Routes��AuthorizeView�Ő��䂷�邽�߂̐ݒ�
// �F�؏�Ԃ�����Component�ɓ`����B
services.AddScoped<AuthenticationStateProvider, BizServerAuthenticationStateProvider>();
services.AddCascadingAuthenticationState();

// Crypto�T�[�r�X
services.AddScoped<CryptoService>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    var isDev = app.Environment.IsDevelopment();

    string csp;
    if (app.Environment.IsDevelopment())
    {
        csp = "base-uri 'self'; " +
              "default-src 'self'; " +
              "img-src 'self' data: https:; " +
              "connect-src *; " +
              "script-src 'self' 'wasm-unsafe-eval' 'unsafe-inline'; " +
              "media-src 'self' data:; " +
              "frame-ancestors 'self';";
    }
    else
    {
        csp = "base-uri 'self'; " +
              "default-src 'self'; " +
              "img-src 'self' data: https:; " +
              "connect-src 'self'; " +
              "script-src 'self' 'wasm-unsafe-eval' 'unsafe-inline'; " +
              "media-src 'self' data:; " +
              "frame-ancestors 'self';";
    }

    context.Response.Headers["Content-Security-Policy"] = csp;
    context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";

    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BizKotei.Client._Imports).Assembly);

app.MapControllers();

app.Run();