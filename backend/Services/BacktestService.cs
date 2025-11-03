using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using System.Text.Json;

namespace Backend.Services;

public class BacktestService
{
    private readonly BacktestRepository _repository;
    private readonly ILogger<BacktestService> _logger;

    public BacktestService(BacktestRepository repository, ILogger<BacktestService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Backtest> CriarBacktestAsync(CriarBacktestDto dto)
    {
        var backtest = new Backtest
        {
            DataInicio = dto.DataInicio.Date,
            DataFim = dto.DataFim.Date,
            Entrada = dto.Entrada,
            Alvo = dto.Alvo,
            NumeroContratos = dto.NumeroContratos,
            AtivoId = dto.AtivoId,
            Stop = dto.Stop,
            Folga = dto.Folga,
            Estrategias = JsonSerializer.Serialize(dto.Estrategias),
            Proteger = dto.Proteger,
            Status = "Iniciado",
            DataCriacao = DateTime.UtcNow
        };

        var resultado = await _repository.CriarBacktestAsync(backtest);
        
        // Buscar o backtest completo com dados do ativo
        var backtestCompleto = await _repository.ObterBacktestPorIdAsync(resultado.Id);
        
        _logger.LogInformation($"Backtest criado com sucesso. ID: {resultado.Id}, Estrategias: {string.Join(", ", dto.Estrategias)}, Stop: {dto.Stop}, Folga: {dto.Folga}, Proteger: {dto.Proteger}, Status: Iniciado");
        
        // TODO: Aqui você vai adicionar a lógica para processar o backtest em background
        // Por enquanto, só retorna com status "Iniciado"
        
        return backtestCompleto ?? resultado;
    }

    public async Task<Backtest?> ObterBacktestPorIdAsync(int id)
    {
        return await _repository.ObterBacktestPorIdAsync(id);
    }

    public async Task<object> ListarBacktestsAsync(int page, int pageSize)
    {
        var (items, totalItems) = await _repository.ListarBacktestsAsync(page, pageSize);
        
        return new
        {
            items,
            totalItems,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public async Task AtualizarStatusAsync(int id, string status, string? resultado = null)
    {
        var backtest = await _repository.ObterBacktestPorIdAsync(id);
        if (backtest == null)
        {
            throw new ArgumentException("Backtest não encontrado");
        }

        await _repository.AtualizarStatusAsync(id, status, resultado);
        
        _logger.LogInformation($"Backtest {id} atualizado para status: {status}");
    }

    public async Task DeletarBacktestAsync(int id)
    {
        var backtest = await _repository.ObterBacktestPorIdAsync(id);
        if (backtest == null)
        {
            throw new ArgumentException("Backtest não encontrado");
        }

        await _repository.DeletarBacktestAsync(id);
        
        _logger.LogInformation($"Backtest {id} deletado");
    }
}

