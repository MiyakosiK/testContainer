var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello from ECS Fargate! おはよう！14：00だよ！");
app.MapGet("/health", () => Results.Ok("healthy"));

app.Run();
