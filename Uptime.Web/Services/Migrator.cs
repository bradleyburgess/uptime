using Dapper;
using Microsoft.Data.Sqlite;
using Uptime.Web.Data.Migrations;

[module: DapperAot]

namespace Uptime.Web.Services;

public record VersionParam(string Version);

public class Migrator(IEnumerable<IMigration> migrations, IConfiguration configuration)
{
    public async Task MigrateAsync()
    {
        var orderedMigrations = migrations.OrderBy(m => m.Version);

        using var connection = new SqliteConnection(configuration.GetConnectionString("Sqlite"));
        await connection.OpenAsync();
        await connection.ExecuteAsync("CREATE TABLE IF NOT EXISTS __Migrations (Version TEXT);");

        foreach (var migration in orderedMigrations)
        {
            var alreadyApplied = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM __Migrations WHERE Version = @Version;",
                new VersionParam(migration.Version)
            );
            if (alreadyApplied == 0)
            {
                await connection.ExecuteAsync(migration.Script);
                await connection.ExecuteAsync(
                    "INSERT INTO __Migrations (Version) VALUES (@Version);",
                    new VersionParam(migration.Version)
                );
            }
        }
        await connection.CloseAsync();
    }
}
