using Uptime.Web.Data.Constants;

namespace Uptime.Web.Data.Migrations;

public class Migration_0001_AddPingResult : IMigration
{
    public string Version => "0001_AddPingResult";

    public string Script =>
        @$"
            CREATE TABLE IF NOT EXISTS PingResults (
                Type VARCHAR(100),
                LastDateUtc TEXT,
                Success INTEGER
            );

            INSERT INTO PingResults (Type) VALUES
                ('{PingResultTypes.MostRecent}'),
                ('{PingResultTypes.MostRecentSuccess}');
            ";
}
