var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPokeapiIntegration(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
