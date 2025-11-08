namespace Backend.Models;

/// <summary>
/// Estatísticas agrupadas por estratégia
/// </summary>
public class EstatisticaEstrategia
{
    public int TotalTrades { get; set; }
    public int Gains { get; set; }
    public int Stops { get; set; }
    public decimal WinRate { get; set; }
    public int SaldoPts { get; set; }
    public decimal SaldoReais { get; set; }
}





