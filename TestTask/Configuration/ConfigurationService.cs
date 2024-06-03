using Microsoft.Extensions.Configuration;

namespace TestTask.Configuration;

public static class ConfigurationService
{
    private static readonly IConfiguration _configuration;

    static ConfigurationService()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        _configuration = builder.Build();
    }

    public static T Get<T>() where T : class => _configuration.Get<T>();
}