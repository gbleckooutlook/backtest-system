using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;

namespace Backend.Services;

public class DayTradeService
{
    private readonly DayTradeRepository _repository;
    private readonly ILogger<DayTradeService> _logger;

    public DayTradeService(DayTradeRepository repository, ILogger<DayTradeService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<DayTrade> CriarDayTradeAsync(CriarDayTradeDto dto)
    {
        var dayTrade = new DayTrade
        {
            DiaDayTrade = dto.DiaDayTrade.Date,
            AtivoId = dto.AtivoId,
            DataCriacao = DateTime.UtcNow
        };

        var resultado = await _repository.CriarDayTradeAsync(dayTrade);
        
        // Buscar o dayTrade completo com dados do ativo
        var dayTradeCompleto = await _repository.ObterDayTradePorIdAsync(resultado.Id);
        
        _logger.LogInformation($"DayTrade criado com sucesso. ID: {resultado.Id}");
        
        return dayTradeCompleto ?? resultado;
    }

    public async Task<DayTrade?> ObterDayTradePorIdAsync(int id)
    {
        return await _repository.ObterDayTradePorIdAsync(id);
    }

    public async Task<object> ListarDayTradesAsync(int page, int pageSize)
    {
        var (items, totalItems) = await _repository.ListarDayTradesAsync(page, pageSize);
        
        return new
        {
            items,
            totalItems,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public async Task DeletarDayTradeAsync(int id)
    {
        var dayTrade = await _repository.ObterDayTradePorIdAsync(id);
        if (dayTrade == null)
        {
            throw new ArgumentException("DayTrade não encontrado");
        }

        await _repository.DeletarDayTradeAsync(id);
        
        _logger.LogInformation($"DayTrade {id} e seus trades foram deletados");
    }
}


