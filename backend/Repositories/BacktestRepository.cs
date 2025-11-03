using Backend.Models;
using Dapper;
using Npgsql;

namespace Backend.Repositories;

public class BacktestRepository
{
    private readonly string _connectionString;

    public BacktestRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string não configurada");
    }

    private NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    public async Task<Backtest> CriarBacktestAsync(Backtest backtest)
    {
        using var connection = GetConnection();
        var sql = @"
            INSERT INTO Backtests (DataInicio, DataFim, Entrada, Alvo, NumeroContratos, AtivoId, Stop, Folga, Estrategias, Proteger, Status, DataCriacao)
            VALUES (@DataInicio, @DataFim, @Entrada, @Alvo, @NumeroContratos, @AtivoId, @Stop, @Folga, @Estrategias, @Proteger, @Status, @DataCriacao)
            RETURNING Id";
        
        var id = await connection.ExecuteScalarAsync<int>(sql, backtest);
        backtest.Id = id;
        return backtest;
    }

    public async Task<Backtest?> ObterBacktestPorIdAsync(int id)
    {
        using var connection = GetConnection();
        var sql = @"
            SELECT b.*, a.Nome as AtivoNome, a.Codigo as AtivoCodigo
            FROM Backtests b
            INNER JOIN Ativos a ON b.AtivoId = a.Id
            WHERE b.Id = @Id";
        
        return await connection.QueryFirstOrDefaultAsync<Backtest>(sql, new { Id = id });
    }

    public async Task<(List<Backtest> items, int totalItems)> ListarBacktestsAsync(int page, int pageSize)
    {
        using var connection = GetConnection();
        
        var countSql = "SELECT COUNT(*) FROM Backtests";
        var totalItems = await connection.ExecuteScalarAsync<int>(countSql);

        var sql = @"
            SELECT b.*, a.Nome as AtivoNome, a.Codigo as AtivoCodigo
            FROM Backtests b
            INNER JOIN Ativos a ON b.AtivoId = a.Id
            ORDER BY b.DataCriacao DESC
            LIMIT @PageSize OFFSET @Offset";

        var items = await connection.QueryAsync<Backtest>(sql, new
        {
            PageSize = pageSize,
            Offset = (page - 1) * pageSize
        });

        return (items.ToList(), totalItems);
    }

    public async Task AtualizarStatusAsync(int id, string status, string? resultado = null)
    {
        using var connection = GetConnection();
        var sql = @"
            UPDATE Backtests 
            SET Status = @Status, 
                DataFinalizacao = @DataFinalizacao,
                Resultado = @Resultado
            WHERE Id = @Id";
        
        await connection.ExecuteAsync(sql, new 
        { 
            Id = id, 
            Status = status,
            DataFinalizacao = status == "Finalizado" || status == "Erro" ? DateTime.UtcNow : (DateTime?)null,
            Resultado = resultado
        });
    }

    public async Task DeletarBacktestAsync(int id)
    {
        using var connection = GetConnection();
        var sql = "DELETE FROM Backtests WHERE Id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }
}

