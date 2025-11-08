using Backend.Models;
using Dapper;
using Npgsql;

namespace Backend.Repositories;

public class CandleRepository
{
    private readonly string _connectionString;

    public CandleRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string não configurada");
    }

    private NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    /// <summary>
    /// Busca todos os candles de um ativo em um período específico.
    /// </summary>
    public async Task<List<Candle>> BuscarPorAtivoEPeriodoAsync(int ativoId, DateTime dataInicio, DateTime dataFim)
    {
        using var connection = GetConnection();
        var sql = @"
            SELECT * FROM Candles
            WHERE AtivoId = @AtivoId 
              AND Data >= @DataInicio 
              AND Data <= @DataFim
            ORDER BY Data ASC, ContadorCandles ASC";
        
        var candles = await connection.QueryAsync<Candle>(sql, new 
        { 
            AtivoId = ativoId, 
            DataInicio = dataInicio.Date, 
            DataFim = dataFim.Date.AddDays(1).AddTicks(-1) // Incluir o dia completo
        });
        return candles.ToList();
    }

    /// <summary>
    /// Busca um candle específico por número do contador em uma data específica.
    /// </summary>
    public async Task<Candle?> BuscarPorNumeroCandleAsync(int ativoId, DateTime data, int numeroCandle)
    {
        using var connection = GetConnection();
        var sql = @"
            SELECT * FROM Candles
            WHERE AtivoId = @AtivoId 
              AND DATE(Data) = DATE(@Data)
              AND ContadorCandles = @NumeroCandle";
        
        return await connection.QueryFirstOrDefaultAsync<Candle>(sql, new 
        { 
            AtivoId = ativoId, 
            Data = data.Date,
            NumeroCandle = numeroCandle
        });
    }
}

