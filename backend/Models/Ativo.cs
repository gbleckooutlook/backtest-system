namespace Backend.Models;

public class Ativo
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Mercado { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Timeframe { get; set; } = string.Empty;
    public string? NomeArquivoCsv { get; set; }
    public DateTime DataCriacao { get; set; }
}


