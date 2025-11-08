using Backend.Interfaces;
using Backend.Models;
using Backend.Repositories;
using System.Text.Json;

namespace Backend.Services;

/// <summary>
/// Serviço responsável pela análise e processamento de backtests.
/// Contém toda a lógica de cálculo de entrada, stop, alvo e análises de otimização.
/// </summary>
public class BacktestAnalyzer
{
    private readonly ITaxaCalculator _taxaCalculator;
    private readonly CandleRepository _candleRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BacktestAnalyzer> _logger;

    public BacktestAnalyzer(
        ITaxaCalculator taxaCalculator,
        CandleRepository candleRepository,
        IConfiguration configuration,
        ILogger<BacktestAnalyzer> logger)
    {
        _taxaCalculator = taxaCalculator;
        _candleRepository = candleRepository;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Método principal que analisa todos os trades do backtest.
    /// </summary>
    public async Task<BacktestResultado> AnalisarTradesAsync(Backtest backtest, List<Trade> trades)
    {
        var resultado = new BacktestResultado();
        var valorPonto = _configuration.GetValue<decimal>("BacktestConfig:ValorPontoWIN", 0.20m);
        
        _logger.LogInformation($"Iniciando análise de {trades.Count} trades para Backtest #{backtest.Id}");

        // Buscar todos os candles do período uma vez
        var candles = await _candleRepository.BuscarPorAtivoEPeriodoAsync(
            backtest.AtivoId, 
            backtest.DataInicio, 
            backtest.DataFim);

        _logger.LogInformation($"Carregados {candles.Count} candles para análise");

        foreach (var trade in trades)
        {
            try
            {
                var tradeResultado = await AnalisarTradeAsync(trade, backtest, candles, valorPonto);
                
                if (tradeResultado != null)
                {
                    resultado.Trades.Add(tradeResultado);
                }
                else
                {
                    resultado.TradesNaoEntraram++;
                }
            }
            catch (Exception ex)
            {
                resultado.Erros.Add($"Erro ao analisar Trade #{trade.Id}: {ex.Message}");
                _logger.LogError(ex, $"Erro ao analisar Trade #{trade.Id}");
            }
        }

        // Compilar resultados finais
        CompilarResultados(resultado, backtest);

        _logger.LogInformation($"Análise concluída. Total de trades processados: {resultado.Trades.Count}, " +
            $"Gains: {resultado.TotalGains}, Stops: {resultado.TotalStops}, " +
            $"Trades não entraram: {resultado.TradesNaoEntraram}");

        return resultado;
    }

    /// <summary>
    /// Analisa um trade individual.
    /// </summary>
    private async Task<TradeResultado?> AnalisarTradeAsync(
        Trade trade, 
        Backtest backtest, 
        List<Candle> todosCandles,
        decimal valorPonto)
    {
        // 1. Buscar candles de referência (Atenção, Confirmação, Região)
        // Usar a data do trade (DiaDayTrade) que vem do JOIN
        var candleAtencao = await _candleRepository.BuscarPorNumeroCandleAsync(
            backtest.AtivoId, 
            trade.DiaDayTrade,
            trade.Gatilho1);

        var candleConfirmacao = await _candleRepository.BuscarPorNumeroCandleAsync(
            backtest.AtivoId, 
            trade.DiaDayTrade,
            trade.Gatilho2);

        Candle? candleRegiao = null;
        if (trade.Regiao.HasValue)
        {
            candleRegiao = await _candleRepository.BuscarPorNumeroCandleAsync(
                backtest.AtivoId, 
                trade.DiaDayTrade,
                trade.Regiao.Value);
        }

        // Validação: se não encontrou candles essenciais, pula o trade
        if (candleAtencao == null || candleConfirmacao == null)
        {
            _logger.LogWarning($"Trade #{trade.Id}: Candles de referência não encontrados");
            return null;
        }

        // 2. Calcular preço de entrada
        var precoEntrada = CalcularPrecoEntrada(trade, candleAtencao, candleConfirmacao, candleRegiao, backtest);

        // 3. Verificar se entrada ativa no próximo candle após confirmação
        var candleEntrada = EncontrarCandleEntrada(candleConfirmacao, todosCandles);
        if (candleEntrada == null || !VerificarEntrada(candleEntrada, precoEntrada, trade.Operacao))
        {
            _logger.LogDebug($"Trade #{trade.Id}: Entrada não ativada");
            return null; // Trade não aconteceu
        }

        // 4. Simular execução do trade
        var tradeResultado = SimularExecucao(
            trade, 
            precoEntrada, 
            candleEntrada, 
            todosCandles, 
            backtest,
            valorPonto);

        return tradeResultado;
    }

    /// <summary>
    /// Calcula o preço de entrada baseado nos candles de referência.
    /// </summary>
    private decimal CalcularPrecoEntrada(
        Trade trade,
        Candle candleAtencao,
        Candle candleConfirmacao,
        Candle? candleRegiao,
        Backtest backtest)
    {
        if (trade.Operacao == "Compra")
        {
            // Menor mínima entre os candles
            var menorMinima = candleAtencao.Minima;
            menorMinima = Math.Min(menorMinima, candleConfirmacao.Minima);
            if (candleRegiao != null)
            {
                menorMinima = Math.Min(menorMinima, candleRegiao.Minima);
            }

            return menorMinima + backtest.Entrada;
        }
        else // Venda
        {
            // Maior máxima entre os candles
            var maiorMaxima = candleAtencao.Maxima;
            maiorMaxima = Math.Max(maiorMaxima, candleConfirmacao.Maxima);
            if (candleRegiao != null)
            {
                maiorMaxima = Math.Max(maiorMaxima, candleRegiao.Maxima);
            }

            return maiorMaxima - backtest.Entrada;
        }
    }

    /// <summary>
    /// Encontra o próximo candle após a confirmação.
    /// Método preparado para futuras validações (limite de candles, variação de preço, etc).
    /// </summary>
    private Candle? EncontrarCandleEntrada(Candle candleConfirmacao, List<Candle> todosCandles)
    {
        // Por enquanto: próximo candle após confirmação
        var index = todosCandles.FindIndex(c => c.Id == candleConfirmacao.Id);
        if (index >= 0 && index < todosCandles.Count - 1)
        {
            return todosCandles[index + 1];
        }
        return null;
    }

    /// <summary>
    /// Verifica se o preço de entrada foi atingido no candle.
    /// </summary>
    private bool VerificarEntrada(Candle candleEntrada, decimal precoEntrada, string operacao)
    {
        // Verifica se o preço de entrada está dentro do range do candle
        return precoEntrada >= candleEntrada.Minima && precoEntrada <= candleEntrada.Maxima;
    }

    /// <summary>
    /// Simula a execução completa do trade.
    /// </summary>
    private TradeResultado SimularExecucao(
        Trade trade,
        decimal precoEntrada,
        Candle candleEntrada,
        List<Candle> todosCandles,
        Backtest backtest,
        decimal valorPonto)
    {
        var resultado = new TradeResultado
        {
            TradeId = trade.Id,
            Data = candleEntrada.Data.Date,
            Horario = candleEntrada.Data.TimeOfDay,
            Estrategia = trade.Estrategia ?? "Não especificada",
            Operacao = trade.Operacao,
            CandleEntrada = candleEntrada.ContadorCandles,
            PrecoEntrada = precoEntrada
        };

        // Calcular Stop e Alvo
        decimal precoStop, precoAlvo;
        if (trade.Operacao == "Compra")
        {
            precoStop = precoEntrada - backtest.Stop;
            precoAlvo = precoEntrada + backtest.Alvo;
        }
        else // Venda
        {
            precoStop = precoEntrada + backtest.Stop;
            precoAlvo = precoEntrada - backtest.Alvo;
        }

        // Variável para controle do Proteger 1:1
        decimal stopAtual = precoStop;
        bool protegeuBreakeven = false;

        // Encontrar índice do candle de entrada
        var indexEntrada = todosCandles.FindIndex(c => c.Id == candleEntrada.Id);
        var candlesAposEntrada = todosCandles.Skip(indexEntrada + 1).ToList();

        // Iterar pelos candles após entrada
        foreach (var candle in candlesAposEntrada)
        {
            // Verificar Proteger 1:1
            if (backtest.Proteger && !protegeuBreakeven)
            {
                if (trade.Operacao == "Compra")
                {
                    // Se lucro atingiu o valor do stop, protege no breakeven
                    if (candle.Maxima - precoEntrada >= backtest.Stop)
                    {
                        stopAtual = precoEntrada;
                        protegeuBreakeven = true;
                        _logger.LogDebug($"Trade #{trade.Id}: Protegido no breakeven");
                    }
                }
                else // Venda
                {
                    if (precoEntrada - candle.Minima >= backtest.Stop)
                    {
                        stopAtual = precoEntrada;
                        protegeuBreakeven = true;
                        _logger.LogDebug($"Trade #{trade.Id}: Protegido no breakeven");
                    }
                }
            }

            // Verificar se atingiu Stop ou Alvo
            bool atingiuStop = false;
            bool atingiuAlvo = false;

            if (trade.Operacao == "Compra")
            {
                atingiuStop = candle.Minima <= stopAtual;
                atingiuAlvo = candle.Maxima >= precoAlvo;
            }
            else // Venda
            {
                atingiuStop = candle.Maxima >= stopAtual;
                atingiuAlvo = candle.Minima <= precoAlvo;
            }

            // O que acontecer primeiro
            if (atingiuStop)
            {
                resultado.Resultado = "Stop";
                resultado.PrecoSaida = stopAtual;
                resultado.Pts = -backtest.Stop;
                resultado.Reais = -backtest.Stop * backtest.NumeroContratos * valorPonto;
                
                // Calcular análise de otimização para stops
                CalcularMaximaEvolucaoFavor(resultado, todosCandles, indexEntrada, candlesAposEntrada.IndexOf(candle), precoEntrada, trade.Operacao);
                
                break;
            }
            
            if (atingiuAlvo)
            {
                resultado.Resultado = "Gain";
                resultado.PrecoSaida = precoAlvo;
                resultado.Pts = backtest.Alvo;
                resultado.Reais = backtest.Alvo * backtest.NumeroContratos * valorPonto;
                
                // Calcular análises de otimização para gains
                CalcularMelhorOportunidadeEntrada(resultado, todosCandles, indexEntrada, candlesAposEntrada.IndexOf(candle), precoEntrada, trade.Operacao);
                CalcularExtensaoAposAlvo(resultado, todosCandles, indexEntrada, candlesAposEntrada.IndexOf(candle), precoAlvo, trade.Operacao);
                
                break;
            }
        }

        // Se não saiu, considera que não terminou (erro de dados)
        if (string.IsNullOrEmpty(resultado.Resultado))
        {
            _logger.LogWarning($"Trade #{trade.Id}: Não atingiu Stop nem Alvo");
            return null;
        }

        return resultado;
    }

    /// <summary>
    /// Para trades com Gain: calcula se houve retorno a favor após entrada.
    /// INCLUI o próprio candle de entrada, pois o preço pode descer no mesmo candle.
    /// </summary>
    private void CalcularMelhorOportunidadeEntrada(
        TradeResultado resultado,
        List<Candle> todosCandles,
        int indexEntrada,
        int indexSaida,
        decimal precoEntrada,
        string operacao)
    {
        // INCLUI o candle de entrada (indexEntrada) até antes do alvo (indexSaida)
        var candlesAteAlvo = todosCandles.Skip(indexEntrada).Take(indexSaida + 1).ToList();
        
        if (candlesAteAlvo.Count == 0) return;
        
        if (operacao == "Compra")
        {
            // Menor Low atingida após entrada e antes do alvo (incluindo candle de entrada)
            var menorLow = candlesAteAlvo.Min(c => c.Minima);
            var retracao = precoEntrada - menorLow;
            
            if (retracao > 0)
            {
                resultado.MelhorOportunidadeEntrada = (int)Math.Round(retracao);
            }
        }
        else // Venda
        {
            // Maior High atingida após entrada e antes do alvo (incluindo candle de entrada)
            var maiorHigh = candlesAteAlvo.Max(c => c.Maxima);
            var retracao = maiorHigh - precoEntrada;
            
            if (retracao > 0)
            {
                resultado.MelhorOportunidadeEntrada = (int)Math.Round(retracao);
            }
        }
    }

    /// <summary>
    /// Para trades com Gain: calcula quanto o preço continuou após o alvo.
    /// </summary>
    private void CalcularExtensaoAposAlvo(
        TradeResultado resultado,
        List<Candle> todosCandles,
        int indexEntrada,
        int indexSaida,
        decimal precoAlvo,
        string operacao)
    {
        var candlesAposAlvo = todosCandles.Skip(indexEntrada + indexSaida + 1).ToList();
        
        if (candlesAposAlvo.Count == 0) return;

        if (operacao == "Compra")
        {
            // Maior High após atingir alvo
            var maiorHigh = candlesAposAlvo.Max(c => c.Maxima);
            var extensao = maiorHigh - precoAlvo;
            
            if (extensao > 0)
            {
                resultado.ExtensaoAposAlvo = (int)Math.Round(extensao);
            }
        }
        else // Venda
        {
            // Menor Low após atingir alvo
            var menorLow = candlesAposAlvo.Min(c => c.Minima);
            var extensao = precoAlvo - menorLow;
            
            if (extensao > 0)
            {
                resultado.ExtensaoAposAlvo = (int)Math.Round(extensao);
            }
        }
    }

    /// <summary>
    /// Para trades com Stop: calcula a máxima evolução a favor antes do stop.
    /// </summary>
    private void CalcularMaximaEvolucaoFavor(
        TradeResultado resultado,
        List<Candle> todosCandles,
        int indexEntrada,
        int indexStop,
        decimal precoEntrada,
        string operacao)
    {
        var candlesAteStop = todosCandles.Skip(indexEntrada + 1).Take(indexStop).ToList();
        
        if (candlesAteStop.Count == 0) return;

        if (operacao == "Compra")
        {
            // Maior High antes do stop
            var maiorHigh = candlesAteStop.Max(c => c.Maxima);
            var evolucao = maiorHigh - precoEntrada;
            
            if (evolucao > 0)
            {
                resultado.MaximaEvolucaoFavor = (int)Math.Round(evolucao);
            }
        }
        else // Venda
        {
            // Menor Low antes do stop
            var menorLow = candlesAteStop.Min(c => c.Minima);
            var evolucao = precoEntrada - menorLow;
            
            if (evolucao > 0)
            {
                resultado.MaximaEvolucaoFavor = (int)Math.Round(evolucao);
            }
        }
    }

    /// <summary>
    /// Compila os resultados finais e calcula estatísticas agregadas.
    /// </summary>
    private void CompilarResultados(BacktestResultado resultado, Backtest backtest)
    {
        // Totais gerais
        resultado.TotalGains = resultado.Trades.Count(t => t.Resultado == "Gain");
        resultado.TotalStops = resultado.Trades.Count(t => t.Resultado == "Stop");
        
        resultado.TotalGainsPts = resultado.Trades.Where(t => t.Resultado == "Gain").Sum(t => t.Pts);
        resultado.TotalStopsPts = Math.Abs(resultado.Trades.Where(t => t.Resultado == "Stop").Sum(t => t.Pts));
        
        resultado.TotalGainsReais = resultado.Trades.Where(t => t.Resultado == "Gain").Sum(t => t.Reais);
        resultado.TotalStopsReais = Math.Abs(resultado.Trades.Where(t => t.Resultado == "Stop").Sum(t => t.Reais));
        
        resultado.SaldoPts = resultado.TotalGainsPts - resultado.TotalStopsPts;
        resultado.SaldoReais = resultado.TotalGainsReais - resultado.TotalStopsReais;
        
        // Win Rate
        var totalTrades = resultado.Trades.Count;
        resultado.WinRate = totalTrades > 0 ? (decimal)resultado.TotalGains / totalTrades * 100 : 0;
        
        // Múltiplo do Stop
        if (backtest.Stop > 0)
        {
            resultado.MultiploStop = resultado.SaldoReais / (backtest.Stop * backtest.NumeroContratos * 0.20m); // Assumindo 0.20 por ponto
        }
        
        // Maior sequência de Stops e Gains
        CalcularSequencias(resultado);
        
        // Agrupar por Estratégia
        resultado.PorEstrategia = resultado.Trades
            .GroupBy(t => t.Estrategia)
            .ToDictionary(
                g => g.Key,
                g => new EstatisticaEstrategia
                {
                    TotalTrades = g.Count(),
                    Gains = g.Count(t => t.Resultado == "Gain"),
                    Stops = g.Count(t => t.Resultado == "Stop"),
                    WinRate = g.Count() > 0 ? (decimal)g.Count(t => t.Resultado == "Gain") / g.Count() * 100 : 0,
                    SaldoPts = g.Sum(t => t.Pts),
                    SaldoReais = g.Sum(t => t.Reais)
                });
        
        // Agrupar por Dia da Semana
        resultado.PorDiaSemana = resultado.Trades
            .GroupBy(t => t.Data.DayOfWeek)
            .ToDictionary(
                g => g.Key,
                g => new EstatisticaDia
                {
                    TotalTrades = g.Count(),
                    Gains = g.Count(t => t.Resultado == "Gain"),
                    Stops = g.Count(t => t.Resultado == "Stop"),
                    SaldoPts = g.Sum(t => t.Pts),
                    SaldoReais = g.Sum(t => t.Reais)
                });
        
        // Agrupar por Horário
        resultado.PorHorario = resultado.Trades
            .GroupBy(t => t.Horario.Hours)
            .ToDictionary(
                g => g.Key,
                g => new EstatisticaHorario
                {
                    TotalTrades = g.Count(),
                    Gains = g.Count(t => t.Resultado == "Gain"),
                    Stops = g.Count(t => t.Resultado == "Stop"),
                    SaldoPts = g.Sum(t => t.Pts),
                    SaldoReais = g.Sum(t => t.Reais)
                });
    }

    /// <summary>
    /// Calcula a maior sequência consecutiva de Stops e Gains.
    /// </summary>
    private void CalcularSequencias(BacktestResultado resultado)
    {
        int sequenciaStopsAtual = 0;
        int sequenciaGainsAtual = 0;
        int maiorSequenciaStops = 0;
        int maiorSequenciaGains = 0;

        foreach (var trade in resultado.Trades)
        {
            if (trade.Resultado == "Stop")
            {
                sequenciaStopsAtual++;
                sequenciaGainsAtual = 0;
                maiorSequenciaStops = Math.Max(maiorSequenciaStops, sequenciaStopsAtual);
            }
            else if (trade.Resultado == "Gain")
            {
                sequenciaGainsAtual++;
                sequenciaStopsAtual = 0;
                maiorSequenciaGains = Math.Max(maiorSequenciaGains, sequenciaGainsAtual);
            }
        }

        resultado.MaiorSequenciaStops = maiorSequenciaStops;
        resultado.MaiorSequenciaGains = maiorSequenciaGains;
    }
}

