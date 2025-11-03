namespace Backend.DTOs;

public class AtivoListaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Mercado { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Timeframe { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
}


