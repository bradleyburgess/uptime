using System.Text.RegularExpressions;

namespace Uptime.Web.Services;

public partial class UrlResolver
{
    private const string DefaultUrl = "https://1.1.1.1";
    public readonly string Url;

    [GeneratedRegex(
        @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$"
    )]
    private static partial Regex IpRegex();

    public UrlResolver(IConfiguration configuration)
    {
        Url = configuration.GetValue<string>("UrlToPing") ?? DefaultUrl;
        if (!Validate(Url))
        {
            throw new ArgumentException($"Invalid URL: {Url}");
        }
    }

    public UrlType GetUrlType()
    {
        if (IsHttpResource(Url))
            return UrlType.HttpResource;
        if (IsIpOrHostname(Url))
            return UrlType.IpOrHostname;
        throw new InvalidOperationException($"Unknown URL type: {Url}");
    }

    private static bool Validate(string url) => IsHttpResource(url) || IsIpOrHostname(url);

    private static bool IsHttpResource(string url)
    {
        return (
                url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            )
            && Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    private static bool IsIpOrHostname(string url) =>
        (IpRegex().IsMatch(url) && System.Net.IPAddress.TryParse(url, out _))
        || Uri.CheckHostName(url) != UriHostNameType.Unknown;
}

public enum UrlType
{
    HttpResource,
    IpOrHostname,
}
