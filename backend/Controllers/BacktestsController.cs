using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/backtests")]
public class BacktestsController : ControllerBase
{
    private readonly BacktestService _backtestService;
    private readonly ILogger<BacktestsController> _logger;

    public BacktestsController(BacktestService backtestService, ILogger<BacktestsController> logger)
    {
        _backtestService = backtestService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> ListarBacktests([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var resultado = await _backtestService.ListarBacktestsAsync(page, pageSize);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar backtests");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> ObterBacktest(int id)
    {
        try
        {
            var backtest = await _backtestService.ObterBacktestPorIdAsync(id);
            if (backtest == null)
                return NotFound(new { erro = "Backtest não encontrado" });
            
            return Ok(backtest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter backtest");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }

    [HttpPost]
    public async Task<ActionResult> CriarBacktest([FromBody] CriarBacktestDto dto)
    {
        try
        {
            var backtest = await _backtestService.CriarBacktestAsync(dto);
            return Ok(backtest);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar backtest");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult> AtualizarStatus(int id, [FromBody] AtualizarStatusDto dto)
    {
        try
        {
            await _backtestService.AtualizarStatusAsync(id, dto.Status, dto.Resultado);
            return Ok(new { mensagem = "Status atualizado com sucesso" });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar status");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletarBacktest(int id)
    {
        try
        {
            await _backtestService.DeletarBacktestAsync(id);
            return Ok(new { mensagem = "Backtest deletado com sucesso" });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar backtest");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }
}

public class AtualizarStatusDto
{
    public string Status { get; set; } = "";
    public string? Resultado { get; set; }
}


