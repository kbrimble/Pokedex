var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddValidators()
    .AddPokeapiIntegration(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
