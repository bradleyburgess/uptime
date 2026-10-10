namespace Uptime.Web.Data.Migrations;

public class Migration_0002_AddOutage : IMigration
{
    public string Version => "0002_AddOutage";

    public string Script =>
        """
            CREATE TABLE IF NOT EXISTS Outages(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StartTime TEXT NOT NULL,
                EndTime TEXT
            );
            """;
}
