using System.Data;
using Microsoft.Data.Sqlite;

namespace Uptime.Web.Data.Repositories;

public abstract class BaseRepository(IConfiguration configuration)
{
    protected SqliteConnection CreateConnection() =>
        new SqliteConnection(configuration.GetConnectionString("Sqlite"));
}
