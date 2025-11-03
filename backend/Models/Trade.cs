namespace Backend.Models;

public class Trade
{
    public int Id { get; set; }
    public int DayTradeId { get; set; }
    public int Gatilho1 { get; set; }
    public int Gatilho2 { get; set; }
    public int? Regiao { get; set; }
    public string Operacao { get; set; } = "Compra"; // Compra ou Venda
    public string? Estrategia { get; set; }
    public DateTime DataCriacao { get; set; }
}

