namespace Backend.Models;

/// <summary>
/// Estatísticas agrupadas por horário (hora específica)
/// </summary>
public class EstatisticaHorario
{
    public int TotalTrades { get; set; }
    public int Gains { get; set; }
    public int Stops { get; set; }
    public int SaldoPts { get; set; }
    public decimal SaldoReais { get; set; }
}





