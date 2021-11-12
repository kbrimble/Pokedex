using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddValidators()
    .AddCommandAndQueryHandlers()
    .AddPokeapiIntegration(builder.Configuration);

builder.Services
    .AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
