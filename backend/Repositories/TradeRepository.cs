using Backend.Models;
using Dapper;
using Npgsql;

namespace Backend.Repositories;

public class TradeRepository
{
    private readonly string _connectionString;

    public TradeRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string não configurada");
    }

    private NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    public async Task<Trade> CriarTradeAsync(Trade trade)
    {
        using var connection = GetConnection();
        var sql = @"
            INSERT INTO Trades (DayTradeId, Gatilho1, Gatilho2, Regiao, Operacao, Estrategia, DataCriacao)
            VALUES (@DayTradeId, @Gatilho1, @Gatilho2, @Regiao, @Operacao, @Estrategia, @DataCriacao)
            RETURNING Id";
        
        var id = await connection.ExecuteScalarAsync<int>(sql, trade);
        trade.Id = id;
        return trade;
    }

    public async Task<List<Trade>> ListarTradesPorDayTradeAsync(int dayTradeId)
    {
        using var connection = GetConnection();
        var sql = @"
            SELECT * FROM Trades
            WHERE DayTradeId = @DayTradeId
            ORDER BY Id ASC";
        
        var trades = await connection.QueryAsync<Trade>(sql, new { DayTradeId = dayTradeId });
        return trades.ToList();
    }

    public async Task<Trade?> ObterTradePorIdAsync(int id)
    {
        using var connection = GetConnection();
        var sql = "SELECT * FROM Trades WHERE Id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Trade>(sql, new { Id = id });
    }

    public async Task DeletarTradeAsync(int id)
    {
        using var connection = GetConnection();
        var sql = "DELETE FROM Trades WHERE Id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    /// <summary>
    /// Busca trades em um período específico e filtrados por estratégias.
    /// Inclui join com DayTrades para pegar a data.
    /// </summary>
    public async Task<List<Trade>> BuscarPorPeriodoEEstrategiasAsync(
        DateTime dataInicio, 
        DateTime dataFim, 
        List<string> estrategias,
        int ativoId)
    {
        using var connection = GetConnection();
        
        // Se não houver estratégias, retorna vazio
        if (estrategias == null || estrategias.Count == 0)
            return new List<Trade>();
        
        var sql = @"
            SELECT t.*, dt.DiaDayTrade
            FROM Trades t
            INNER JOIN DayTrades dt ON t.DayTradeId = dt.Id
            WHERE dt.DiaDayTrade >= @DataInicio 
              AND dt.DiaDayTrade <= @DataFim
              AND dt.AtivoId = @AtivoId
              AND t.Estrategia = ANY(@Estrategias)
            ORDER BY dt.DiaDayTrade ASC, t.Id ASC";
        
        var trades = await connection.QueryAsync<Trade>(sql, new 
        { 
            DataInicio = dataInicio.Date, 
            DataFim = dataFim.Date,
            AtivoId = ativoId,
            Estrategias = estrategias.ToArray()
        });
        
        return trades.ToList();
    }
}

