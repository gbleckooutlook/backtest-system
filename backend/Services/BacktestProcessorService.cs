using Backend.Models;
using Backend.Repositories;
using System.Text.Json;

namespace Backend.Services;

/// <summary>
/// Serviço em background que processa backtests com status "Iniciado".
/// Executa polling periódico e processa backtests enfileirados.
/// </summary>
public class BacktestProcessorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BacktestProcessorService> _logger;
    private readonly IConfiguration _configuration;

    public BacktestProcessorService(
        IServiceProvider serviceProvider,
        ILogger<BacktestProcessorService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BacktestProcessorService iniciado");

        var pollingInterval = _configuration.GetValue<int>("BacktestConfig:PollingIntervalSeconds", 5);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var backtestRepository = scope.ServiceProvider.GetRequiredService<BacktestRepository>();
                    
                    // Buscar Backtests com Status = "Iniciado"
                    var backtests = await backtestRepository.BuscarPorStatusAsync("Iniciado");

                    if (backtests.Count > 0)
                    {
                        _logger.LogInformation($"Encontrados {backtests.Count} backtests para processar");

                        foreach (var backtest in backtests)
                        {
                            try
                            {
                                await ProcessarBacktestAsync(backtest, scope, stoppingToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Erro ao processar Backtest #{backtest.Id}");
                                
                                // Marcar como "Erro" e continuar
                                await backtestRepository.AtualizarResultadoAsync(
                                    backtest.Id,
                                    "Erro",
                                    JsonSerializer.Serialize(new { erro = ex.Message }),
                                    DateTime.UtcNow);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no BacktestProcessorService");
            }

            // Aguardar intervalo configurado antes de próxima iteração
            await Task.Delay(TimeSpan.FromSeconds(pollingInterval), stoppingToken);
        }

        _logger.LogInformation("BacktestProcessorService encerrado");
    }

    /// <summary>
    /// Processa um backtest individual.
    /// </summary>
    private async Task ProcessarBacktestAsync(Backtest backtest, IServiceScope scope, CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Processando Backtest #{backtest.Id}");

        var analyzer = scope.ServiceProvider.GetRequiredService<BacktestAnalyzer>();
        var tradeRepository = scope.ServiceProvider.GetRequiredService<TradeRepository>();
        var backtestRepository = scope.ServiceProvider.GetRequiredService<BacktestRepository>();

        // 1. Deserializar estratégias
        List<string> estrategias;
        try
        {
            estrategias = JsonSerializer.Deserialize<List<string>>(backtest.Estrategias) ?? new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao deserializar estratégias do Backtest #{backtest.Id}");
            throw new Exception("Erro ao deserializar estratégias", ex);
        }

        if (estrategias.Count == 0)
        {
            throw new Exception("Nenhuma estratégia selecionada");
        }

        _logger.LogInformation($"Backtest #{backtest.Id}: Estratégias selecionadas: {string.Join(", ", estrategias)}");

        // 2. Buscar Trades do período filtrados por estratégias e ativo
        var trades = await tradeRepository.BuscarPorPeriodoEEstrategiasAsync(
            backtest.DataInicio,
            backtest.DataFim,
            estrategias,
            backtest.AtivoId);

        _logger.LogInformation($"Backtest #{backtest.Id}: Encontrados {trades.Count} trades para análise");

        if (trades.Count == 0)
        {
            _logger.LogWarning($"Backtest #{backtest.Id}: Nenhum trade encontrado no período");
            
            // Salvar resultado vazio
            var resultadoVazio = new BacktestResultado
            {
                Erros = new List<string> { "Nenhum trade encontrado no período com as estratégias selecionadas" }
            };
            
            await backtestRepository.AtualizarResultadoAsync(
                backtest.Id,
                "Finalizado",
                JsonSerializer.Serialize(resultadoVazio, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                }),
                DateTime.UtcNow);
            
            return;
        }

        // 3. Analisar trades
        var resultado = await analyzer.AnalisarTradesAsync(backtest, trades);

        // 4. Serializar e salvar resultado
        var resultadoJson = JsonSerializer.Serialize(resultado, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });

        await backtestRepository.AtualizarResultadoAsync(
            backtest.Id,
            "Finalizado",
            resultadoJson,
            DateTime.UtcNow);

        _logger.LogInformation($"Backtest #{backtest.Id} finalizado com sucesso. " +
            $"Saldo: {resultado.SaldoPts} PTS / R$ {resultado.SaldoReais:F2}");
    }
}





