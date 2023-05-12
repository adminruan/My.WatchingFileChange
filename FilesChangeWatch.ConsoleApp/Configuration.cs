using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace FilesChangeWatch.ConsoleApp;

public static class Configuration
{
    internal readonly static IConfiguration _configuration;

    static Configuration()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
    }

    public static List<string?> GetCopyToPathString(string name)
    {
        List<string?> list = new List<string?>();

        var sections = _configuration?.GetSection($"WatcherSetting:{name}").GetChildren();
        if (sections != null && sections.Any())
        {
            list = sections.Select(p => p.Value).ToList();
        }

        return list;
    }

    public static string? GetWatcherSettingString(string name)
    {
        return _configuration?.GetSection("WatcherSetting")[name];
    }
}
