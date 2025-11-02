namespace BacktestSystem.DTOs;

public class CriarAtivoDto
{
    public string Nome { get; set; } = string.Empty;
    public string Mercado { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Timeframe { get; set; } = string.Empty;
    public IFormFile? ArquivoCsv { get; set; }
}


