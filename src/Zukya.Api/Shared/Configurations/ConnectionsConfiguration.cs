using Microsoft.EntityFrameworkCore;
using Zukya.Infra.Persistence;

namespace Zukya.Api.Shared.Configurations;

public static class ConnectionsConfiguration
{
    public static IServiceCollection AddAppConnections(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbConnection(configuration);
        return services;
    }

    private static IServiceCollection AddDbConnection(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("ZukyaDb");

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

        services.AddDbContext<DatabaseContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions.UseNetTopologySuite()
            )
        );
        return services;
    }

    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environment == "EndToEndTest")
            return app;
        using IServiceScope scope = app.Services.CreateScope();
        DatabaseContext dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        dbContext.Database.Migrate();
        return app;
    }
}
