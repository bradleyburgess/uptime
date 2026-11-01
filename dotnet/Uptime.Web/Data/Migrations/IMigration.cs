namespace Uptime.Web.Data.Migrations;

public interface IMigration
{
    string Version { get; }
    string Script { get; }
}
