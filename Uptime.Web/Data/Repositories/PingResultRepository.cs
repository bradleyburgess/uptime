using Dapper;
using Uptime.Web.Data.Constants;
using Uptime.Web.Data.Models;

namespace Uptime.Web.Data.Repositories;

public class PingResultsRepository(IConfiguration configuration) : BaseRepository(configuration)
{
    public async Task<PingResult?> GetMostRecentAsync()
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
        var sql =
            "SELECT Type, LastDateUtc, COALESCE(Success, 0) AS Success FROM PingResults WHERE Type = @Type";
        return await connection.QuerySingleOrDefaultAsync<PingResult>(
            sql,
            new { Type = PingResultTypes.MostRecent }
        );
    }

    public async Task<PingResult?> GetMostRecentSuccessAsync()
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
        var sql =
            "SELECT Type, LastDateUtc, COALESCE(Success, 0) AS Success FROM PingResults WHERE Type = @Type";
        return await connection.QuerySingleOrDefaultAsync<PingResult>(
            sql,
            new { Type = PingResultTypes.MostRecentSuccess }
        );
    }

    public async Task UpdateMostRecentAsync(bool isSuccess)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
        var sql =
            "UPDATE PingResults SET LastDateUtc = @LastDateUtc, Success = @Success WHERE Type = @Type";
        await connection.ExecuteAsync(
            sql,
            new
            {
                LastDateUtc = DateTimeOffset.UtcNow,
                Success = isSuccess,
                Type = PingResultTypes.MostRecent,
            }
        );
    }

    public async Task UpdateMostRecentSuccessAsync()
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
        var sql = "UPDATE PingResults SET LastDateUtc = @LastDateUtc WHERE Type = @Type";
        await connection.ExecuteAsync(
            sql,
            new { LastDateUtc = DateTimeOffset.UtcNow, Type = PingResultTypes.MostRecentSuccess }
        );
    }
}
