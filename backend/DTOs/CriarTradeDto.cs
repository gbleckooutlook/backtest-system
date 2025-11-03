namespace Backend.DTOs;

public class CriarTradeDto
{
    public int DayTradeId { get; set; }
    public int Gatilho1 { get; set; }
    public int Gatilho2 { get; set; }
    public int? Regiao { get; set; }
    public string Operacao { get; set; } = "Compra";
    public string? Estrategia { get; set; }
}

