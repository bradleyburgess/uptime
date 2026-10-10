namespace Uptime.Web.Data.Models;

public class PingResult
{
    public string Type { get; init; } = "";
    public DateTimeOffset? LastDateUtc { get; set; }
    public bool Success { get; set; }
}
