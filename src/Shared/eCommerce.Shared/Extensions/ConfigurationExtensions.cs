using Microsoft.Extensions.Configuration;

namespace eCommerce.Shared.Extensions;

public static class ConfigurationExtensions
{
    public static T GetOptions<T>(this IConfiguration configuration, string section) where T : class, new()
    {
        var options = new T();
        configuration.GetSection(section).Bind(options);
        return options;
    }

    public static T GetOptions<T>(this IConfiguration configuration, params string[] names) where T : class, new()
    {
        if (!names.NotNullOrEmpty())
            throw new IndexOutOfRangeException(nameof(names));

        return configuration.GetOptions<T>(string.Join(':', names));
    }

    public static T GetOptions<T>(this IConfiguration configuration) where T : class, new()
        => GetOptions<T>(configuration, typeof(T).Name);
}