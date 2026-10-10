using System.Net.NetworkInformation;
using Uptime.Web.Data.Repositories;
using Uptime.Web.Services;

namespace Uptime.Web.BackgroundServices;

public class PingHostedService(
    UrlResolver urlResolver,
    PingResultsRepository pingResultsRepo,
    OutagesRepository outagesRepo,
    IConfiguration configuration
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var urlToPing = urlResolver.Url;
        var pollInterval = TimeSpan.FromSeconds(configuration.GetValue<int>("PollInterval"));
        while (!stoppingToken.IsCancellationRequested)
        {
            var result = urlResolver.IsIpOrHostname()
                ? await PerformPing(urlToPing)
                : await PerformHttpGet(urlToPing);
            await (result ? HandleSuccess() : HandleFailure());
            await Task.Delay(pollInterval, stoppingToken);
        }
    }

    private async Task HandleFailure()
    {
        var mostRecentOutage = await outagesRepo.GetMostRecentAsync();
        if (mostRecentOutage == null || mostRecentOutage.EndTime != null)
        {
            await outagesRepo.AddAsync();
        }
        await pingResultsRepo.UpdateMostRecentAsync(false);
    }

    private async Task HandleSuccess()
    {
        var mostRecentOutage = await outagesRepo.GetMostRecentAsync();
        if (mostRecentOutage != null && mostRecentOutage.EndTime == null)
        {
            await outagesRepo.SetEndTimeAsync(mostRecentOutage.Id);
        }
        await pingResultsRepo.UpdateMostRecentAsync(true);
        await pingResultsRepo.UpdateMostRecentSuccessAsync();
    }

    private static async Task<bool> PerformPing(string ipOrHost)
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

    private static async Task<bool> PerformHttpGet(string url)
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
