using DotNetEnv;
using Microsoft.AspNetCore.HttpLogging;
using Zukya.Api.Shared.Configurations;

Env.Load();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

ConfigurationManager configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("ZukyaDb");
if (!string.IsNullOrEmpty(connectionString))
{
    connectionString = connectionString
        .Replace("${DB_HOST}", Environment.GetEnvironmentVariable("DB_HOST"))
        .Replace("${DB_PORT}", Environment.GetEnvironmentVariable("DB_PORT"))
        .Replace("${DB_NAME}", Environment.GetEnvironmentVariable("DB_NAME"))
        .Replace("${DB_USER}", Environment.GetEnvironmentVariable("DB_USER"))
        .Replace("${DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD"));

    configuration["ConnectionStrings:ZukyaDb"] = connectionString;
}

configuration["Jwt:PrivateKey"] = Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY") ?? configuration["Jwt:PrivateKey"];
configuration["Jwt:PublicKey"] = Environment.GetEnvironmentVariable("JWT_PUBLIC_KEY") ?? configuration["Jwt:PublicKey"];
configuration["Jwt:SecretKey"] = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? configuration["Jwt:SecretKey"];
configuration["Jwt:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "Zukya";
configuration["Jwt:Audience"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "Zukya";

configuration["Storage:AccessKey"] = Environment.GetEnvironmentVariable("STORAGE_ACCESS_KEY");
configuration["Storage:SecretKey"] = Environment.GetEnvironmentVariable("STORAGE_SECRET_KEY");
configuration["Storage:BucketName"] = Environment.GetEnvironmentVariable("STORAGE_BUCKET_NAME");
configuration["Storage:EndpointUrl"] = Environment.GetEnvironmentVariable("STORAGE_ENDPOINT_URL");
configuration["Storage:PublicUrl"] = Environment.GetEnvironmentVariable("STORAGE_PUBLIC_URL");


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services
    .AddAppConnections(configuration)
    .AddSecurity(configuration)
    .AddUseCases()
    .AddStorage(configuration)
    .AddAndConfigureControllers()
    .AddMemoryCache()
    .AddCors(options =>
    {
        options.AddPolicy("CORS", policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
    })
    .AddHttpLogging(logging =>
    {
        logging.LoggingFields = HttpLoggingFields.All;
        logging.RequestBodyLogLimit = 4096;
        logging.ResponseBodyLogLimit = 4096;
    })
    .AddMemoryCache();

WebApplication app = builder.Build();

app.UseHttpLogging();
app.UseDocumentation();
app.UseCors("CORS");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
