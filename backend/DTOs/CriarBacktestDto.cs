namespace Backend.DTOs;

public class CriarBacktestDto
{
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public int Entrada { get; set; }
    public int Alvo { get; set; }
    public int NumeroContratos { get; set; }
    public int AtivoId { get; set; }
    public int Stop { get; set; }
    public List<string> Estrategias { get; set; } = new List<string>();
    public bool Proteger { get; set; } = false;
}

