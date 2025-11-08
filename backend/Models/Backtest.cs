namespace Backend.Models;

public class Backtest
{
    public int Id { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public int Entrada { get; set; }
    public int Alvo { get; set; }
    public int NumeroContratos { get; set; }
    public int AtivoId { get; set; }
    public int Stop { get; set; }
    public string Estrategias { get; set; } = "[]"; // JSON array de estratégias
    public bool Proteger { get; set; } = false;
    public string Status { get; set; } = "Iniciado"; // Iniciado, Finalizado, Erro
    public DateTime DataCriacao { get; set; }
    public DateTime? DataFinalizacao { get; set; }
    public string? Resultado { get; set; }
    
    // Propriedades do Ativo (JOIN)
    public string? AtivoNome { get; set; }
    public string? AtivoCodigo { get; set; }
}

