using Dapper;
using Uptime.Web.Data.Models;

namespace Uptime.Web.Data.Repositories;

public class OutagesRepository(IConfiguration configuration) : BaseRepository(configuration)
{
    public async Task<int> GetCountAsync()
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
        var sql = "SELECT COUNT(*) FROM Outages;";
        return await connection.ExecuteScalarAsync<int>(sql);
    }

    public async Task AddAsync()
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
        var sql = "INSERT INTO Outages (StartTime) VALUES (@StartTime);";
        await connection.ExecuteAsync(sql, new { StartTime = DateTimeOffset.UtcNow });
    }

    public async Task SetEndTimeAsync(int id)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
        var sql = "UPDATE Outages SET EndTime = @EndTime WHERE Id = @Id;";
        await connection.ExecuteAsync(sql, new { EndTime = DateTimeOffset.UtcNow, Id = id });
    }

    public async Task<Outage?> GetMostRecentAsync()
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
        var sql = "SELECT * FROM Outages ORDER BY StartTime DESC LIMIT 1;";
        return await connection.QuerySingleOrDefaultAsync<Outage>(sql);
    }

    public async Task<IReadOnlyList<Outage>> GetRecentListAsync(int limit = 10)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
        var sql = $"SELECT * FROM Outages ORDER BY StartTime DESC LIMIT {limit};";
        return (await connection.QueryAsync<Outage>(sql)).ToList();
    }
}
