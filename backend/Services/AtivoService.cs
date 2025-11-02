using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using BacktestSystem.Models;
using BacktestSystem.DTOs;
using BacktestSystem.Repositories;

namespace BacktestSystem.Services;

public class AtivoService
{
    private readonly AtivoRepository _repository;
    private readonly ILogger<AtivoService> _logger;

    public AtivoService(AtivoRepository repository, ILogger<AtivoService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<int> CriarAtivoComCsvAsync(CriarAtivoDto dto)
    {
        try
        {
            // Validar dados
            if (string.IsNullOrWhiteSpace(dto.Nome) || string.IsNullOrWhiteSpace(dto.Codigo))
            {
                throw new ArgumentException("Nome e Código são obrigatórios");
            }

            if (dto.Mercado != "B3" && dto.Mercado != "Forex")
            {
                throw new ArgumentException("Mercado deve ser B3 ou Forex");
            }

            // Criar o ativo
            var ativo = new Ativo
            {
                Nome = dto.Nome,
                Mercado = dto.Mercado,
                Codigo = dto.Codigo,
                Timeframe = dto.Timeframe,
                NomeArquivoCsv = dto.ArquivoCsv?.FileName,
                DataCriacao = DateTime.UtcNow
            };

            var ativoId = await _repository.CriarAtivoAsync(ativo);

            // Processar CSV se fornecido
            if (dto.ArquivoCsv != null && dto.ArquivoCsv.Length > 0)
            {
                var candles = await ProcessarCsvAsync(dto.ArquivoCsv, ativoId);
                await _repository.InserirCandlesAsync(candles);
                _logger.LogInformation($"Processados {candles.Count} candles para o ativo {ativoId}");
            }

            return ativoId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar ativo com CSV");
            throw;
        }
    }

    public async Task<PaginacaoDto<AtivoListaDto>> ListarAtivosAsync(int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        return await _repository.ListarAtivosAsync(page, pageSize);
    }

    private async Task<List<Candle>> ProcessarCsvAsync(IFormFile arquivo, int ativoId)
    {
        var candles = new List<Candle>();

        using var reader = new StreamReader(arquivo.OpenReadStream());
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ",",
            MissingFieldFound = null
        };

        using var csv = new CsvReader(reader, config);
        
        await csv.ReadAsync();
        csv.ReadHeader();

        while (await csv.ReadAsync())
        {
            try
            {
                var candle = new Candle
                {
                    AtivoId = ativoId,
                    Data = ParseDateTime(csv.GetField("Data")),
                    Abertura = ParseDecimal(csv.GetField("Abertura")),
                    Maxima = ParseDecimal(csv.GetField("Máxima")),
                    Minima = ParseDecimal(csv.GetField("Mínima")),
                    Fechamento = ParseDecimal(csv.GetField("Fechamento")),
                    ContadorCandles = int.Parse(csv.GetField("Contador de Candles") ?? "0")
                };

                candles.Add(candle);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Erro ao processar linha do CSV: {ex.Message}");
            }
        }

        return candles;
    }

    private DateTime ParseDateTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Data inválida");

        // Tenta parsear no formato dd/MM/yyyy HH:mm
        if (DateTime.TryParseExact(value, "dd/MM/yyyy HH:mm", 
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }

        // Tenta outros formatos comuns
        if (DateTime.TryParse(value, out var dateAlt))
        {
            return dateAlt;
        }

        throw new ArgumentException($"Formato de data inválido: {value}");
    }

    private decimal ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Valor decimal inválido");

        // Remove espaços e tenta converter
        value = value.Trim();
        
        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        throw new ArgumentException($"Formato de decimal inválido: {value}");
    }
}


