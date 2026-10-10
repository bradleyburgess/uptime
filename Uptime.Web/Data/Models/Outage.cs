namespace Uptime.Web.Data.Models;

public class Outage
{
    public int Id { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
}
