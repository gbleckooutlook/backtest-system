using Microsoft.AspNetCore.Mvc;
using BacktestSystem.DTOs;
using BacktestSystem.Services;

namespace BacktestSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AtivosController : ControllerBase
{
    private readonly AtivoService _ativoService;
    private readonly ILogger<AtivosController> _logger;

    public AtivosController(AtivoService ativoService, ILogger<AtivosController> logger)
    {
        _ativoService = ativoService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<int>> CriarAtivo([FromForm] CriarAtivoDto dto)
    {
        try
        {
            var ativoId = await _ativoService.CriarAtivoComCsvAsync(dto);
            return Ok(new { id = ativoId, mensagem = "Ativo criado com sucesso" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar ativo");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<PaginacaoDto<AtivoListaDto>>> ListarAtivos(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var resultado = await _ativoService.ListarAtivosAsync(page, pageSize);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar ativos");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> ObterAtivo(int id)
    {
        try
        {
            // Implementar quando necessário
            return NotFound(new { erro = "Ativo não encontrado" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter ativo");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }
}


