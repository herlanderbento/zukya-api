using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Zukya.Infra.Persistence;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

        var basePath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "../Zukya.Api")
        );

        var envFiles = new[] { ".env", ".env.development" };
        foreach (var envFile in envFiles) TryLoadEnvFile(Path.Combine(basePath, envFile));

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        var connectionString = configuration.GetConnectionString("ZukyaDb");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception("Connection string is null or empty!");

        connectionString = ExpandEnvironmentVariables(connectionString);

        optionsBuilder.UseNpgsql(
            connectionString,
            npgsqlOptions => npgsqlOptions.UseNetTopologySuite()
        );

        return new DatabaseContext(optionsBuilder.Options);
    }

    private static string ExpandEnvironmentVariables(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return connectionString;

        var result = connectionString;

        const string pattern = @"\$\{(\w+)\}";
        MatchCollection matches = Regex.Matches(result, pattern);

        foreach (Match match in matches)
        {
            var envVarName = match.Groups[1].Value;
            var envVarValue = Environment.GetEnvironmentVariable(envVarName);

            if (!string.IsNullOrEmpty(envVarValue)) result = result.Replace(match.Value, envVarValue);
        }

        return result;
    }

    private static void TryLoadEnvFile(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        try
        {
            LoadEnvFile(filePath);
        }
        catch
        {
            // ignored
        }
    }

    private static void LoadEnvFile(string filePath)
    {
        foreach (var line in File.ReadAllLines(filePath))
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("#"))
                continue;

            var equalIndex = trimmedLine.IndexOf('=');
            if (equalIndex <= 0)
                continue;

            var key = trimmedLine.Substring(0, equalIndex).Trim();
            var value = trimmedLine.Substring(equalIndex + 1).Trim();

            if (
                (value.StartsWith("\"") && value.EndsWith("\""))
                || (value.StartsWith("'") && value.EndsWith("'"))
            )
                value = value.Substring(1, value.Length - 2);

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
                Environment.SetEnvironmentVariable(key, value);
        }
    }
}
