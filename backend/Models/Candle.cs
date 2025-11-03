namespace Backend.Models;

public class Candle
{
    public int Id { get; set; }
    public int AtivoId { get; set; }
    public DateTime Data { get; set; }
    public decimal Abertura { get; set; }
    public decimal Maxima { get; set; }
    public decimal Minima { get; set; }
    public decimal Fechamento { get; set; }
    public int ContadorCandles { get; set; }
}


