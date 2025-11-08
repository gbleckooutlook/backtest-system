namespace Backend.Models;

/// <summary>
/// Representa o resultado da análise de um trade individual no backtest.
/// </summary>
public class TradeResultado
{
    /// <summary>
    /// ID do trade original
    /// </summary>
    public int TradeId { get; set; }
    
    /// <summary>
    /// Data do trade
    /// </summary>
    public DateTime Data { get; set; }
    
    /// <summary>
    /// Horário do trade (apenas hora e minuto)
    /// </summary>
    public TimeSpan Horario { get; set; }
    
    /// <summary>
    /// Estratégia utilizada no trade
    /// </summary>
    public string Estrategia { get; set; } = string.Empty;
    
    /// <summary>
    /// Tipo de operação: "Compra" ou "Venda"
    /// </summary>
    public string Operacao { get; set; } = string.Empty;
    
    /// <summary>
    /// Número do candle onde a entrada foi ativada
    /// </summary>
    public int CandleEntrada { get; set; }
    
    /// <summary>
    /// Preço de entrada no trade
    /// </summary>
    public decimal PrecoEntrada { get; set; }
    
    /// <summary>
    /// Preço de saída do trade
    /// </summary>
    public decimal PrecoSaida { get; set; }
    
    /// <summary>
    /// Resultado do trade: "Gain" (lucro) ou "Stop" (perda)
    /// </summary>
    public string Resultado { get; set; } = string.Empty;
    
    /// <summary>
    /// Quantidade de pontos ganhos/perdidos
    /// </summary>
    public int Pts { get; set; }
    
    /// <summary>
    /// Valor em reais ganhos/perdidos
    /// </summary>
    public decimal Reais { get; set; }
    
    // Análises de Otimização
    
    /// <summary>
    /// Para trades com Gain: Pontos que o preço voltou a favor após entrada e antes do alvo.
    /// Indica oportunidade de entrada melhor (mais próxima do alvo).
    /// Valores positivos indicam que houve retorno a favor.
    /// </summary>
    public int? MelhorOportunidadeEntrada { get; set; }
    
    /// <summary>
    /// Para trades com Gain: Pontos que o preço continuou evoluindo após atingir o alvo.
    /// Indica potencial de prolongar o trade.
    /// </summary>
    public int? ExtensaoAposAlvo { get; set; }
    
    /// <summary>
    /// Para trades com Stop: Máxima evolução a favor antes de atingir o stop.
    /// Indica quanto o trade evoluiu favorável antes de reverter.
    /// </summary>
    public int? MaximaEvolucaoFavor { get; set; }
}





