namespace Backend.Models;

public class DayTrade
{
    public int Id { get; set; }
    public DateTime DiaDayTrade { get; set; }
    public int AtivoId { get; set; }
    public DateTime DataCriacao { get; set; }
    
    // Propriedades de navegação
    public string? AtivoNome { get; set; }
    public string? AtivoCodigo { get; set; }
}


