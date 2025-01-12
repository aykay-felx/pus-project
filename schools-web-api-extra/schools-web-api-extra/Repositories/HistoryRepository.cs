using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using schools_web_api_extra.Interface;
using schools_web_api_extra.Models;

namespace schools_web_api_extra.Repositories;

public class HistoryRepository : IHistoryService
{
    private readonly string _connectionString;
    private readonly IHttpClientFactory _httpClientFactory;

    public HistoryRepository(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Get history logs by rsponumer
    /// </summary>
    /// <param name="rspoNumer"></param>
    /// <returns>historyList</returns>
    public async Task<IEnumerable<SchoolHistory>> GetHistoryByRspoAsync(string rspoNumer)
    {
        var historyList = new List<SchoolHistory>();

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
        SELECT Id, RspoNumer, ChangedAt, Changes
        FROM SchoolHistory
        WHERE RspoNumer = @rspo
        ORDER BY ChangedAt DESC;
    ";

        using var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.AddWithValue("rspo", rspoNumer);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var item = new SchoolHistory
            {
                Id = reader.GetInt32(0),
                RspoNumer = reader.GetString(1),
                ChangedAt = reader.GetDateTime(2),
                Changes = reader.GetString(3)
            };
            historyList.Add(item);
        }

        return historyList;
    }
}