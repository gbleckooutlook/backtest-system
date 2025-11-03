using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/trades")]
public class TradesController : ControllerBase
{
    private readonly TradeService _tradeService;
    private readonly ILogger<TradesController> _logger;

    public TradesController(TradeService tradeService, ILogger<TradesController> logger)
    {
        _tradeService = tradeService;
        _logger = logger;
    }

    [HttpGet("daytrade/{dayTradeId}")]
    public async Task<ActionResult> ListarTradesPorDayTrade(int dayTradeId)
    {
        try
        {
            var trades = await _tradeService.ListarTradesPorDayTradeAsync(dayTradeId);
            return Ok(trades);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar trades");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> ObterTrade(int id)
    {
        try
        {
            var trade = await _tradeService.ObterTradePorIdAsync(id);
            if (trade == null)
                return NotFound(new { erro = "Trade não encontrado" });
            
            return Ok(trade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter trade");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }

    [HttpPost]
    public async Task<ActionResult> CriarTrade([FromBody] CriarTradeDto dto)
    {
        try
        {
            var trade = await _tradeService.CriarTradeAsync(dto);
            return Ok(trade);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar trade");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletarTrade(int id)
    {
        try
        {
            await _tradeService.DeletarTradeAsync(id);
            return Ok(new { mensagem = "Trade deletado com sucesso" });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar trade");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }
}

