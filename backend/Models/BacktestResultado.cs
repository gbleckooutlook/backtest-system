namespace Backend.Models;

/// <summary>
/// Resultado completo do processamento de um backtest.
/// Contém estatísticas gerais, detalhes de cada trade e agrupamentos.
/// </summary>
public class BacktestResultado
{
    // Resultado Geral - Stops
    public int TotalStops { get; set; }
    public int TotalStopsPts { get; set; }
    public decimal TotalStopsReais { get; set; }
    
    // Resultado Geral - Gains
    public int TotalGains { get; set; }
    public int TotalGainsPts { get; set; }
    public decimal TotalGainsReais { get; set; }
    
    // Saldo Final
    public int SaldoPts { get; set; }
    public decimal SaldoReais { get; set; }
    
    /// <summary>
    /// Múltiplo do Stop. Exemplo: se ganhou 1000 e stop era 100, MultiploStop = 10x
    /// </summary>
    public decimal MultiploStop { get; set; }
    
    // Sequências
    public int MaiorSequenciaStops { get; set; }
    public int MaiorSequenciaGains { get; set; }
    
    // Taxas e Win Rate
    public decimal WinRate { get; set; }
    public decimal TaxasTotal { get; set; } // Preparado para futuro
    
    // Detalhes de Cada Trade
    public List<TradeResultado> Trades { get; set; } = new List<TradeResultado>();
    
    // Agrupamentos
    public Dictionary<string, EstatisticaEstrategia> PorEstrategia { get; set; } = new Dictionary<string, EstatisticaEstrategia>();
    public Dictionary<DayOfWeek, EstatisticaDia> PorDiaSemana { get; set; } = new Dictionary<DayOfWeek, EstatisticaDia>();
    public Dictionary<int, EstatisticaHorario> PorHorario { get; set; } = new Dictionary<int, EstatisticaHorario>(); // Chave = hora (9, 10, 11...)
    
    // Erros e Trades Não Executados
    public List<string> Erros { get; set; } = new List<string>();
    public int TradesNaoEntraram { get; set; } // Trades que não ativaram entrada
}





