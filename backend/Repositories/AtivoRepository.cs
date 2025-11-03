using Dapper;
using Npgsql;
using Backend.Models;
using Backend.DTOs;

namespace Backend.Repositories;

public class AtivoRepository
{
    private readonly string _connectionString;

    public AtivoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string não configurada");
    }

    private NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    public async Task<int> CriarAtivoAsync(Ativo ativo)
    {
        using var connection = GetConnection();
        var sql = @"
            INSERT INTO Ativos (Nome, Mercado, Codigo, Timeframe, NomeArquivoCsv, DataCriacao)
            VALUES (@Nome, @Mercado, @Codigo, @Timeframe, @NomeArquivoCsv, @DataCriacao)
            RETURNING Id";
        
        return await connection.ExecuteScalarAsync<int>(sql, ativo);
    }

    public async Task<PaginacaoDto<AtivoListaDto>> ListarAtivosAsync(int page, int pageSize)
    {
        using var connection = GetConnection();
        
        var countSql = "SELECT COUNT(*) FROM Ativos";
        var totalItems = await connection.ExecuteScalarAsync<int>(countSql);

        var offset = (page - 1) * pageSize;
        var sql = @"
            SELECT Id, Nome, Mercado, Codigo, Timeframe, DataCriacao
            FROM Ativos
            ORDER BY DataCriacao DESC
            LIMIT @PageSize OFFSET @Offset";

        var ativos = await connection.QueryAsync<AtivoListaDto>(sql, new { PageSize = pageSize, Offset = offset });

        return new PaginacaoDto<AtivoListaDto>
        {
            Items = ativos.ToList(),
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Ativo?> ObterAtivoPorIdAsync(int id)
    {
        using var connection = GetConnection();
        var sql = "SELECT * FROM Ativos WHERE Id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Ativo>(sql, new { Id = id });
    }

    public async Task InserirCandlesAsync(List<Candle> candles)
    {
        using var connection = GetConnection();
        var sql = @"
            INSERT INTO Candles (AtivoId, Data, Abertura, Maxima, Minima, Fechamento, ContadorCandles)
            VALUES (@AtivoId, @Data, @Abertura, @Maxima, @Minima, @Fechamento, @ContadorCandles)";
        
        await connection.ExecuteAsync(sql, candles);
    }

    public async Task AtualizarAtivoAsync(Ativo ativo)
    {
        using var connection = GetConnection();
        var sql = @"
            UPDATE Ativos
            SET Nome = @Nome,
                Mercado = @Mercado,
                Codigo = @Codigo,
                Timeframe = @Timeframe,
                NomeArquivoCsv = @NomeArquivoCsv
            WHERE Id = @Id";
        
        await connection.ExecuteAsync(sql, ativo);
    }

    public async Task DeletarAtivoAsync(int id)
    {
        using var connection = GetConnection();
        var sql = "DELETE FROM Ativos WHERE Id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task DeletarCandlesPorAtivoAsync(int ativoId)
    {
        using var connection = GetConnection();
        var sql = "DELETE FROM Candles WHERE AtivoId = @AtivoId";
        await connection.ExecuteAsync(sql, new { AtivoId = ativoId });
    }

    public async Task InicializarBancoDadosAsync()
    {
        using var connection = GetConnection();
        var scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "init.sql");
        
        if (File.Exists(scriptPath))
        {
            var sql = await File.ReadAllTextAsync(scriptPath);
            await connection.ExecuteAsync(sql);
        }
    }
}


