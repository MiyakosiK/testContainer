var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello from ECS Fargate!");
app.MapGet("/health", () => Results.Ok("healthy"));

app.Run();
