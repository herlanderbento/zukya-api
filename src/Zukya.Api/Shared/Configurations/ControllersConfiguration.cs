using System.Text.Json.Serialization;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Zukya.Api.Shared.Filters;
using Zukya.Api.Shared.Validators;

namespace Zukya.Api.Shared.Configurations;

public static class ControllersConfiguration
{
    public static IServiceCollection AddAndConfigureControllers(this IServiceCollection services)
    {
        services
            .AddControllers(options => options.Filters.Add(typeof(ApiGlobalExceptionFilter)))
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
        });

        services.AddTransient<RequestValidator>();

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "Zukya API", Version = "v1", Description = "Marketplace Zukya - Backend"
                };

                document.Components ??= new OpenApiComponents();

                document.Components.SecuritySchemes?.Add("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Enter the JWT token in the format: Bearer {your_token}"
                    });

                document.Security ??= [];
                document.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });

                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static WebApplication UseDocumentation(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return app;
        app.MapOpenApi();

        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle("Zukya API Documentation")
                .WithTheme(ScalarTheme.Default)
                .WithClassicLayout()
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });

        return app;
    }
}
