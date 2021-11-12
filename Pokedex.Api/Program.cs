using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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
app.UseExceptionHandler(GetExceptionHandler());

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program
{
    static Action<IApplicationBuilder> GetExceptionHandler()
    {
        return errorApp => errorApp.Run(async context =>
        {
            var errorMessage = "Something went wrong";
            var statusCode = 500;
            var detail = "";
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (exceptionHandlerFeature?.Error is ValidationException validationException)
            {
                errorMessage = "Bad request";
                statusCode = 400;
                detail = validationException.Message;
            }

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails { Status = statusCode, Title = errorMessage, Detail = detail }));
        });
    }
}


