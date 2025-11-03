using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/daytrades")]
public class DayTradesController : ControllerBase
{
    private readonly DayTradeService _dayTradeService;
    private readonly ILogger<DayTradesController> _logger;

    public DayTradesController(DayTradeService dayTradeService, ILogger<DayTradesController> logger)
    {
        _dayTradeService = dayTradeService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> ListarDayTrades([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var resultado = await _dayTradeService.ListarDayTradesAsync(page, pageSize);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar daytrades");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> ObterDayTrade(int id)
    {
        try
        {
            var dayTrade = await _dayTradeService.ObterDayTradePorIdAsync(id);
            if (dayTrade == null)
                return NotFound(new { erro = "DayTrade não encontrado" });
            
            return Ok(dayTrade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter daytrade");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }

    [HttpPost]
    public async Task<ActionResult> CriarDayTrade([FromBody] CriarDayTradeDto dto)
    {
        try
        {
            var dayTrade = await _dayTradeService.CriarDayTradeAsync(dto);
            return Ok(dayTrade);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar daytrade");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletarDayTrade(int id)
    {
        try
        {
            await _dayTradeService.DeletarDayTradeAsync(id);
            return Ok(new { mensagem = "DayTrade deletado com sucesso" });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar daytrade");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }
}


