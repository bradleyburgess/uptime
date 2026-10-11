using System.Globalization;
using System.Net;
using System.Text;
using Uptime.Web.Data.Models;

namespace Uptime.Web.TemplateBuilders;

public record HomeTemplateModel(
    string AppName,
    string UrlToPing,
    IReadOnlyList<Outage> Outages,
    PingResult? MostRecentPing,
    PingResult? MostRecentSuccessfulPing,
    string Version
);

public class HomeTemplateBuilder(HomeTemplateModel model) : ITemplateBuilder
{
    public string Build()
    {
        var sb = new StringBuilder();
        sb.Append("<!DOCTYPE html>");
        sb.Append("<html lang=\"en\">");
        sb.Append("<head>");
        sb.Append("<meta charset=\"UTF-8\">");
        sb.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.Append($"<title>Uptime</title>");
        sb.Append(
            "<link rel=\"stylesheet\" href=\"https://cdn.jsdelivr.net/npm/@picocss/pico@2.1.1/css/pico.min.css\">"
        );
        sb.Append(
            "<style>.status{display:inline-block;padding:.25rem .75rem;border-radius:999px;color:#fff;font-weight:600}.status.online{background:#2e7d32}.status.offline{background:#c62828}.status.unknown{background:#757575}.status-row{display:flex;flex-wrap:wrap;align-items:center;gap:1rem 1rem}.status-details p{margin:0}main>section{margin-bottom:3rem}@media(max-width:575px){.status-row{row-gap:1.25rem}}</style>"
        );
        sb.Append("<style>html{height:100%;}body{min-height:100%;display:flex;flex-direction:column;}main{flex:1;}footer{text-align:center;}</style>");
        sb.Append("</head>");
        sb.Append("<body>");
        sb.Append("<main class=\"container\">");
        sb.Append(BuildHeader());
        sb.Append(BuildCurrentStatusSection());
        sb.Append(BuildOutagesSection());
        sb.Append("</main>");
        sb.Append(BuildFooter());
        sb.Append("</body>");
        sb.Append("</html>");
        return sb.ToString();
    }

    private string BuildHeader()
    {
        var sb = new StringBuilder();
        sb.Append("<header>");
        sb.Append("<h1>Uptime</h1>");
        sb.Append($"<p>Version: {WebUtility.HtmlEncode(model.Version)}</p>");
        sb.Append($"<p>Monitoring: <code>{WebUtility.HtmlEncode(model.UrlToPing)}</code></p>");
        sb.Append("</header>");
        return sb.ToString();
    }

    private string BuildCurrentStatusSection()
    {
        var sb = new StringBuilder();
        sb.Append("<section>");
        sb.Append("<article>");
        sb.Append("<header>Current Status</header>");
        sb.Append("<div class=\"status-row\">");
        sb.Append(
            model.MostRecentPing == null ? BuildUnknown()
            : model.MostRecentPing.Success ? BuildOnline()
            : BuildOffline()
        );
        sb.Append("<div class=\"status-details\">");
        if (model.MostRecentPing is { Success: false })
        {
            var ongoingOutage = model.Outages.FirstOrDefault(o => o.EndTime == null);
            if (ongoingOutage != null)
            {
                sb.Append(
                    $"<p><small>Offline since {FormatDate(ongoingOutage.StartTime)}</small></p>"
                );
            }
        }
        sb.Append(
            $"<p><small>Last successful ping: {(model.MostRecentSuccessfulPing == null ? "Never" : FormatDate(model.MostRecentSuccessfulPing.LastDateUtc))}</small></p>"
        );
        sb.Append("</div>");
        sb.Append("</div>");
        sb.Append("</article>");
        sb.Append("</section>");
        return sb.ToString();
    }

    private string BuildOnline() => "<span class=\"status online\">Online</span>";

    private string BuildOffline() => "<span class=\"status offline\">Offline</span>";

    private string BuildUnknown() => "<span class=\"status unknown\">Unknown</span>";

    private string BuildOutagesSection()
    {
        var sb = new StringBuilder();
        sb.Append("<section>");
        sb.Append("<h2>Recent Outages</h2>");
        if (model.Outages.Count == 0)
        {
            sb.Append("<p>No outages recorded.</p>");
        }
        else
        {
            sb.Append("<figure><table class=\"striped\">");
            sb.Append(
                "<thead><tr><th>Start Time</th><th>End Time</th><th>Duration</th></tr></thead>"
            );
            sb.Append("<tbody>");
            foreach (var outage in model.Outages)
            {
                sb.Append(
                    $"<tr><td>{FormatDate(outage.StartTime)}</td><td>{(outage.EndTime.HasValue ? FormatDate(outage.EndTime.Value) : "Ongoing")}</td><td>{FormatDuration(outage.EndTime ?? DateTimeOffset.UtcNow, outage.StartTime)}</td></tr>"
                );
            }
            sb.Append("</tbody>");
            sb.Append("</table></figure>");
        }
        sb.Append("</section>");
        return sb.ToString();
    }

    private string BuildFooter()
    {
        return $"<footer><p>v. {WebUtility.HtmlEncode(model.Version)} <br/>Copyright © {DateTimeOffset.UtcNow.Year} Bradley Burgess.</p></footer>";
    }

    private static string FormatDate(DateTimeOffset? date)
    {
        if (!date.HasValue)
            return "Unknown";
        var utc = date.Value.ToUniversalTime();
        return $"<time datetime=\"{utc:O}\">{utc.ToString("MMM d, yyyy HH:mm:ss", CultureInfo.InvariantCulture)} UTC</time>";
    }

    private static string FormatDuration(DateTimeOffset endTime, DateTimeOffset startTime)
    {
        var duration = endTime - startTime;
        if (duration < TimeSpan.Zero)
        {
            duration = TimeSpan.Zero;
        }
        var parts = new List<string>();
        if (duration.Days > 0)
            parts.Add($"{duration.Days}d");
        if (duration.Hours > 0)
            parts.Add($"{duration.Hours}h");
        if (duration.Minutes > 0)
            parts.Add($"{duration.Minutes}m");
        if (duration.Seconds > 0 || parts.Count == 0)
            parts.Add($"{duration.Seconds}s");
        return string.Join(" ", parts);
    }
}
