var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello from ECS Fargate! パイプラインでデプロイテスト中");
app.MapGet("/health", () => Results.Ok("healthy"));

app.Run();
