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
            var ativo = await _ativoService.ObterAtivoPorIdAsync(id);
            if (ativo == null)
                return NotFound(new { erro = "Ativo não encontrado" });
            
            return Ok(ativo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter ativo");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> AtualizarAtivo(int id, [FromForm] CriarAtivoDto dto)
    {
        try
        {
            await _ativoService.AtualizarAtivoAsync(id, dto);
            return Ok(new { mensagem = "Ativo atualizado com sucesso" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar ativo");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletarAtivo(int id)
    {
        try
        {
            await _ativoService.DeletarAtivoAsync(id);
            return Ok(new { mensagem = "Ativo deletado com sucesso" });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar ativo");
            return StatusCode(500, new { erro = "Erro interno ao processar requisição" });
        }
    }
}


