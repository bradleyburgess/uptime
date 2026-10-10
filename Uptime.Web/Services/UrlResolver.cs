namespace Uptime.Web.Services;

public class UrlResolver
{
    private const string DefaultUrl = "https://cp.cloudflare.com/generate_204";
    public readonly string Url;

    public UrlResolver(IConfiguration configuration)
    {
        Url = configuration.GetValue<string>("UrlToPing") ?? DefaultUrl;
    }

    public bool IsIpOrHostname()
    {
        return System.Net.IPAddress.TryParse(Url, out _)
            || Uri.CheckHostName(Url) != UriHostNameType.Unknown;
    }
}
