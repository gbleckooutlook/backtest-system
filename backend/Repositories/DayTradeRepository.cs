using Backend.Models;
using Dapper;
using Npgsql;

namespace Backend.Repositories;

public class DayTradeRepository
{
    private readonly string _connectionString;

    public DayTradeRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string não configurada");
    }

    private NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    public async Task<DayTrade> CriarDayTradeAsync(DayTrade dayTrade)
    {
        using var connection = GetConnection();
        var sql = @"
            INSERT INTO DayTrades (DiaDayTrade, AtivoId, DataCriacao)
            VALUES (@DiaDayTrade, @AtivoId, @DataCriacao)
            RETURNING Id";
        
        var id = await connection.ExecuteScalarAsync<int>(sql, dayTrade);
        dayTrade.Id = id;
        return dayTrade;
    }

    public async Task<DayTrade?> ObterDayTradePorIdAsync(int id)
    {
        using var connection = GetConnection();
        var sql = @"
            SELECT d.*, a.Nome as AtivoNome, a.Codigo as AtivoCodigo
            FROM DayTrades d
            INNER JOIN Ativos a ON d.AtivoId = a.Id
            WHERE d.Id = @Id";
        
        return await connection.QueryFirstOrDefaultAsync<DayTrade>(sql, new { Id = id });
    }

    public async Task<(List<DayTrade> items, int totalItems)> ListarDayTradesAsync(int page, int pageSize)
    {
        using var connection = GetConnection();
        
        var countSql = "SELECT COUNT(*) FROM DayTrades";
        var totalItems = await connection.ExecuteScalarAsync<int>(countSql);

        var sql = @"
            SELECT d.*, a.Nome as AtivoNome, a.Codigo as AtivoCodigo
            FROM DayTrades d
            INNER JOIN Ativos a ON d.AtivoId = a.Id
            ORDER BY d.DataCriacao DESC
            LIMIT @PageSize OFFSET @Offset";

        var items = await connection.QueryAsync<DayTrade>(sql, new
        {
            PageSize = pageSize,
            Offset = (page - 1) * pageSize
        });

        return (items.ToList(), totalItems);
    }

    public async Task DeletarDayTradeAsync(int id)
    {
        using var connection = GetConnection();
        var sql = "DELETE FROM DayTrades WHERE Id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }
}


