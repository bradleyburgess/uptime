using Uptime.Web.Data.Repositories;
using Uptime.Web.Services;
using Uptime.Web.TemplateBuilders;

namespace Uptime.Web.Routes;

public class HomeRoute
{
    public const string Route = "/";

    public static async Task<IResult> HandleAsync(
        PingResultsRepository pingResultRepo,
        OutagesRepository outagesRepo,
        UrlResolver urlResolver
    )
    {
        var model = new HomeTemplateModel(
            AppName: "Uptime",
            UrlToPing: urlResolver.Url,
            Outages: await outagesRepo.GetRecentListAsync(),
            MostRecentPing: await pingResultRepo.GetMostRecentAsync(),
            MostRecentSuccessfulPing: await pingResultRepo.GetMostRecentSuccessAsync()
        );
        var html = new HomeTemplateBuilder(model).Build();
        return Results.Content(html, "text/html");
    }
}
