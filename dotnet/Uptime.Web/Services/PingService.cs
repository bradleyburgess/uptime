using System.Net.NetworkInformation;

namespace Uptime.Web.Services;

public interface IPingService
{
    Task<bool> PingAsync();
}

public class HttpPingService(string url) : IPingService
{
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(5) };

    public async Task<bool> PingAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Head, url);
        try
        {
            var response = await HttpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead
            );
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

public class IcmpPingService(string ipOrHost) : IPingService
{
    public async Task<bool> PingAsync()
    {
        try
        {
            using var ping = new Ping();
            PingReply reply = await ping.SendPingAsync(ipOrHost, timeout: 2000);
            return reply.Status == IPStatus.Success;
        }
        catch
        {
            return false;
        }
    }
}

public static class PingServiceFactory
{
    public static IPingService Create(UrlResolver urlResolver)
    {
        return urlResolver.GetUrlType() switch
        {
            UrlType.HttpResource => new HttpPingService(urlResolver.Url),
            UrlType.IpOrHostname => new IcmpPingService(urlResolver.Url),
            _ => throw new InvalidOperationException(
                $"Unsupported URL type: {urlResolver.GetUrlType()}"
            ),
        };
    }
}
