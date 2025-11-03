using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;

namespace Backend.Services;

public class TradeService
{
    private readonly TradeRepository _repository;
    private readonly ILogger<TradeService> _logger;

    public TradeService(TradeRepository repository, ILogger<TradeService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Trade> CriarTradeAsync(CriarTradeDto dto)
    {
        var trade = new Trade
        {
            DayTradeId = dto.DayTradeId,
            Gatilho1 = dto.Gatilho1,
            Gatilho2 = dto.Gatilho2,
            Regiao = dto.Regiao,
            Operacao = dto.Operacao,
            Estrategia = dto.Estrategia,
            DataCriacao = DateTime.UtcNow
        };

        var resultado = await _repository.CriarTradeAsync(trade);
        
        _logger.LogInformation($"Trade {dto.Operacao} ({dto.Estrategia}) criado com sucesso. ID: {resultado.Id}, DayTrade: {dto.DayTradeId}");
        
        return resultado;
    }

    public async Task<List<Trade>> ListarTradesPorDayTradeAsync(int dayTradeId)
    {
        return await _repository.ListarTradesPorDayTradeAsync(dayTradeId);
    }

    public async Task<Trade?> ObterTradePorIdAsync(int id)
    {
        return await _repository.ObterTradePorIdAsync(id);
    }

    public async Task DeletarTradeAsync(int id)
    {
        var trade = await _repository.ObterTradePorIdAsync(id);
        if (trade == null)
        {
            throw new ArgumentException("Trade não encontrado");
        }

        await _repository.DeletarTradeAsync(id);
        
        _logger.LogInformation($"Trade {id} foi deletado");
    }
}

