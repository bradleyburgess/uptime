using System.Net.NetworkInformation;
using Uptime.Web.Data.Models;
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
    private readonly int PollInterval = configuration.GetValue<int>("PollInterval", 30);
    private readonly int DowntimeThreshold = configuration.GetValue<int>("DowntimeThreshold", 60);
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(5) };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var urlToPing = urlResolver.Url;
        while (!stoppingToken.IsCancellationRequested)
        {
            var result = urlResolver.IsIpOrHostname()
                ? await PerformPing(urlToPing)
                : await PerformHttpGet(urlToPing);
            await (result ? HandleSuccess() : HandleFailure());
            await Task.Delay(TimeSpan.FromSeconds(PollInterval), stoppingToken);
        }
    }

    private async Task HandleFailure()
    {
        var mostRecentOutage = await outagesRepo.GetMostRecentAsync();
        var mostRecentRecordedPing = await pingResultsRepo.GetMostRecentAsync();
        if (IsNotOngoingOutage(mostRecentOutage))
        {
            var startTime = IsDowntimeExceeded(mostRecentRecordedPing!.LastDateUtc)
                ? mostRecentRecordedPing!.LastDateUtc
                : DateTimeOffset.UtcNow;
            await outagesRepo.AddAsync(startTime);
        }
        await pingResultsRepo.UpdateMostRecentAsync(false);
    }

    private async Task HandleSuccess()
    {
        var mostRecentOutage = await outagesRepo.GetMostRecentAsync();
        var mostRecentRecordedPing = await pingResultsRepo.GetMostRecentAsync();
        if (IsOngoingOutage(mostRecentOutage))
        {
            await outagesRepo.SetEndTimeAsync(mostRecentOutage!.Id);
        }
        else if (IsDowntimeExceeded(mostRecentRecordedPing!.LastDateUtc))
        {
            await outagesRepo.AddAsync(mostRecentRecordedPing.LastDateUtc, DateTimeOffset.UtcNow);
        }
        await pingResultsRepo.UpdateMostRecentAsync(true);
        await pingResultsRepo.UpdateMostRecentSuccessAsync();
    }

    private bool IsDowntimeExceeded(DateTimeOffset? lastDateUtc)
    {
        return lastDateUtc.HasValue
            && lastDateUtc.Value < DateTimeOffset.UtcNow.AddSeconds(-DowntimeThreshold);
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

    private static bool IsOngoingOutage(Outage? outage) => outage != null && outage.EndTime == null;

    private static bool IsNotOngoingOutage(Outage? outage) =>
        outage == null || outage.EndTime != null;
}
