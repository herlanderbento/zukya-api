using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Movu.Infra.Storage;
using Zukya.Application.Common.Interfaces;
using Zukya.Infra.Services.Storage;

namespace Zukya.Api.Shared.Configurations;

public static class StorageConfiguration
{
    public static IServiceCollection AddStorage(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<StorageServiceOptions>(
            configuration.GetSection(StorageServiceOptions.ConfigurationSection)
        );

        StorageServiceOptions? options = configuration
            .GetSection(StorageServiceOptions.ConfigurationSection)
            .Get<StorageServiceOptions>();

        if (options is null)
            throw new InvalidOperationException(
                $"Storage configuration section '{StorageServiceOptions.ConfigurationSection}' is missing or invalid."
            );

        services.AddSingleton<IAmazonS3>(_ =>
        {
            var awsOptions = new AmazonS3Config();

            // If EndpointUrl is provided (e.g., Cloudflare R2), use it
            if (!string.IsNullOrWhiteSpace(options.EndpointUrl))
            {
                awsOptions.ServiceURL = options.EndpointUrl;
                awsOptions.ForcePathStyle = true; // Cloudflare R2 requires path-style addressing
                awsOptions.DisableHostPrefixInjection = true; // Required for custom endpoints like R2
                awsOptions.UseHttp = false; // Use HTTPS
                awsOptions.AuthenticationRegion = "auto"; // Required for R2
            }
            else
            {
                // Default AWS S3 configuration
                var region = string.IsNullOrWhiteSpace(options.Region)
                    ? "us-east-1"
                    : options.Region;
                awsOptions.RegionEndpoint = RegionEndpoint.GetBySystemName(region);
            }

            // Use BasicAWSCredentials explicitly for better compatibility with R2
            var credentials = new BasicAWSCredentials(options.AccessKey, options.SecretKey);
            return new AmazonS3Client(credentials, awsOptions);
        });

        services.AddTransient<IStorageService, StorageService>();

        return services;
    }
}
