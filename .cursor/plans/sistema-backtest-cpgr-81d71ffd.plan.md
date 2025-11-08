<!-- 81d71ffd-edda-4290-8fd3-a47dd60f7557 3f1ced59-731d-4f35-849a-7633e362836f -->
# Plano: Serviço de Backtest em Background

## 1. Remover Campo "Folga"

### Frontend

- `frontend/app/composables/useBacktests.ts`: Remover `folga` da interface `Backtest` e parâmetros de `criarBacktest`
- `frontend/app/pages/backtests/criar.vue`: Remover campo "Folga PTS", validação `folgaInvalida`, e todas referências no `form.value`
- `frontend/app/pages/backtests/[id].vue`: Remover exibição do campo "Folga PTS"

### Backend

- `backend/Models/Backtest.cs`: Remover propriedade `Folga`
- `backend/DTOs/CriarBacktestDto.cs`: Remover propriedade `Folga`
- `backend/Services/BacktestService.cs`: Remover `Folga` do método `CriarBacktestAsync`
- `backend/Repositories/BacktestRepository.cs`: Remover `Folga` do SQL INSERT

### Database

- `backend/Database/init.sql`: Remover coluna `Folga` da tabela `Backtests`

## 2. Criar Estrutura de Classes para Análise

### `backend/Interfaces/ITaxaCalculator.cs` (preparar para futuro)

```csharp
public interface ITaxaCalculator
{
    decimal CalcularTaxa(decimal valor, int numeroContratos);
}
```

### `backend/Services/ZeroTaxaCalculator.cs` (implementação inicial)

```csharp
public class ZeroTaxaCalculator : ITaxaCalculator
{
    public decimal CalcularTaxa(decimal valor, int numeroContratos) => 0;
}
```

### `backend/Models/TradeResultado.cs` (detalhes de cada trade analisado)

```csharp
public class TradeResultado
{
    public int TradeId { get; set; }
    public DateTime Data { get; set; }
    public TimeSpan Horario { get; set; }
    public string Estrategia { get; set; }
    public string Operacao { get; set; } // Compra/Venda
    public int CandleEntrada { get; set; }
    public decimal PrecoEntrada { get; set; }
    public decimal PrecoSaida { get; set; }
    public string Resultado { get; set; } // "Gain" ou "Stop"
    public int Pts { get; set; }
    public decimal Reais { get; set; }
    
    // Análises de Otimização
    public int? MelhorOportunidadeEntrada { get; set; } // PTS que voltou a favor (trades com Gain)
    public int? ExtensaoAposAlvo { get; set; } // PTS que continuou após alvo (trades com Gain)
    public int? MaximaEvolucaoFavor { get; set; } // PTS a favor antes do stop (trades com Stop)
}
```

### `backend/Models/BacktestResultado.cs` (resultado completo)

```csharp
public class BacktestResultado
{
    // Resultado Geral
    public int TotalStops { get; set; }
    public int TotalStopsPts { get; set; }
    public decimal TotalStopsReais { get; set; }
    
    public int TotalGains { get; set; }
    public int TotalGainsPts { get; set; }
    public decimal TotalGainsReais { get; set; }
    
    public int SaldoPts { get; set; }
    public decimal SaldoReais { get; set; }
    public decimal MultiploStop { get; set; } // Ex: 10x
    
    public int MaiorSequenciaStops { get; set; }
    public int MaiorSequenciaGains { get; set; }
    
    public decimal WinRate { get; set; }
    public decimal TaxasTotal { get; set; }
    
    // Detalhes de Cada Trade
    public List<TradeResultado> Trades { get; set; }
    
    // Agrupamentos
    public Dictionary<string, EstatisticaEstrategia> PorEstrategia { get; set; }
    public Dictionary<DayOfWeek, EstatisticaDia> PorDiaSemana { get; set; }
    public Dictionary<int, EstatisticaHorario> PorHorario { get; set; }
    
    // Erros
    public List<string> Erros { get; set; }
    public int TradesNaoEntraram { get; set; }
}
```

## 3. Criar Serviço de Análise - BacktestAnalyzer

### `backend/Services/BacktestAnalyzer.cs`

**Métodos principais:**

1. **AnalisarTrades**: Método principal que orquestra toda a análise
2. **AnalisarTrade**: Processa um trade individual
3. **CalcularPrecoEntrada**: Calcula preço de entrada (compra/venda)
4. **VerificarEntrada**: Valida se entrada ativa no próximo candle
5. **SimularExecucao**: Executa o trade e retorna resultado
6. **CalcularAnalisesOtimizacao**: Calcula métricas de otimização
7. **CompilarResultados**: Agrega estatísticas finais

### Lógica Detalhada

#### CalcularPrecoEntrada

**COMPRA:**

- Menor Mínima = MIN(Atenção.Low, Confirmação.Low, Região?.Low)
- Preço Entrada = Menor Mínima + backtest.Entrada

**VENDA:**

- Maior Máxima = MAX(Atenção.High, Confirmação.High, Região?.High)
- Preço Entrada = Maior Máxima - backtest.Entrada

#### VerificarEntrada

- Pega **próximo candle após Confirmação**
- **COMPRA**: Verifica se `candle.Low <= PrecoEntrada <= candle.High`
- **VENDA**: Mesma lógica
- **Se NÃO ativar**: Retorna null, trade não aconteceu

#### SimularExecucao

**Calcular Stop e Alvo:**

- **COMPRA**: Stop = Entrada - Stop PTS, Alvo = Entrada + Alvo PTS
- **VENDA**: Stop = Entrada + Stop PTS, Alvo = Entrada - Alvo PTS

**Iterar candles após entrada:**

- Para cada candle, verificar se atingiu Stop ou Alvo
- **Proteger 1:1** (se `backtest.Proteger == true`):
  - **COMPRA**: Se lucro >= Stop PTS, mover Stop para Entrada (breakeven)
  - **VENDA**: Se lucro >= Stop PTS, mover Stop para Entrada (breakeven)
- Sair quando atingir Stop ou Alvo (o que acontecer primeiro)
- Calcular resultado em PTS e R$

#### CalcularAnalisesOtimizacao

**Para trades com GAIN:**

1. **MelhorOportunidadeEntrada** (após entrada, antes do alvo):

   - **COMPRA**: Menor Low atingida - PrecoEntrada (se negativo, é oportunidade)
   - **VENDA**: PrecoEntrada - Maior High atingida (se positivo, é oportunidade)
   - Indica: "Se entrada fosse X pts menor, pegaria melhor preço"

2. **ExtensaoAposAlvo** (após atingir alvo):

   - **COMPRA**: Maior High após alvo - PrecoAlvo
   - **VENDA**: PrecoAlvo - Menor Low após alvo
   - Indica: "Poderia ter pegado mais X pts se prolongasse o trade"

**Para trades com STOP:**

3. **MaximaEvolucaoFavor** (antes de atingir stop):

   - **COMPRA**: Maior High antes do stop - PrecoEntrada
   - **VENDA**: PrecoEntrada - Menor Low antes do stop
   - Indica: "Trade evoluiu X pts a favor antes de virar contra"

## 4. Criar Background Service

### `backend/Services/BacktestProcessorService.cs`

Implementa `BackgroundService` do .NET:

**ExecuteAsync:**

- Loop infinito com polling a cada 5 segundos
- Busca Backtests com `Status = "Iniciado"`
- Para cada backtest, chama `ProcessarBacktest`
- Try-catch por backtest: se erro, marca como "Erro" e continua

**ProcessarBacktest:**

1. Deserializar estratégias do JSON
2. Buscar Trades do período filtrados por estratégias
3. Buscar Candles do Ativo para o período
4. Chamar `BacktestAnalyzer.AnalisarTrades`
5. Serializar resultado para JSON
6. Atualizar Status = "Finalizado", DataFinalizacao, Resultado

## 5. Novos Métodos nos Repositories

### TradeRepository

- `BuscarPorPeriodoEEstrategias(DateTime inicio, DateTime fim, List<string> estrategias)`: Busca trades por período e filtra por estratégias. Inclui join com DayTrade para pegar a data.

### CandleRepository

- `BuscarPorAtivoEPeriodo(int ativoId, DateTime inicio, DateTime fim)`: Retorna todos candles do ativo no período
- `BuscarPorNumeroCandle(int ativoId, DateTime data, int numeroCandle)`: Busca candle específico por número

### BacktestRepository

- `BuscarPorStatus(string status)`: Retorna backtests com status específico
- `AtualizarResultado(int id, string status, string resultado, DateTime dataFinalizacao)`: Atualiza resultado do backtest

## 6. Registrar Serviços

### `backend/Program.cs`

```csharp
builder.Services.AddHostedService<BacktestProcessorService>();
builder.Services.AddScoped<BacktestAnalyzer>();
builder.Services.AddSingleton<ITaxaCalculator, ZeroTaxaCalculator>();
```

## 7. Configurações

### `backend/appsettings.json`

```json
{
  "BacktestConfig": {
    "ValorPontoWIN": 0.20,
    "PollingIntervalSeconds": 5
  }
}
```

## Arquivos a Criar

**Interfaces:**

- `backend/Interfaces/ITaxaCalculator.cs`

**Models:**

- `backend/Models/TradeResultado.cs`
- `backend/Models/BacktestResultado.cs`
- `backend/Models/EstatisticaEstrategia.cs`
- `backend/Models/EstatisticaDia.cs`
- `backend/Models/EstatisticaHorario.cs`

**Services:**

- `backend/Services/BacktestAnalyzer.cs`
- `backend/Services/BacktestProcessorService.cs`
- `backend/Services/ZeroTaxaCalculator.cs`

## Arquivos a Modificar

**Frontend:**

- `frontend/app/composables/useBacktests.ts`
- `frontend/app/pages/backtests/criar.vue`
- `frontend/app/pages/backtests/[id].vue`

**Backend:**

- `backend/Models/Backtest.cs`
- `backend/DTOs/CriarBacktestDto.cs`
- `backend/Services/BacktestService.cs`
- `backend/Repositories/BacktestRepository.cs`
- `backend/Repositories/TradeRepository.cs`
- `backend/Repositories/CandleRepository.cs`
- `backend/Program.cs`

**Database:**

- `backend/Database/init.sql`

## Observações Importantes

1. **Entrada**: Apenas próximo candle após Confirmação. Se não ativar, trade não aconteceu
2. **Proteger 1:1**: Apenas se `backtest.Proteger == true`
3. **Taxas**: Estrutura preparada com interface, implementação inicial retorna 0
4. **Análises de Otimização**: Métricas completas para melhorar Entrada, Alvo e Stop
5. **Dados para Futuro**: JSON completo com detalhes para gráficos e mapas de calor
6. **Valor do Ponto**: Configurável via appsettings (R$ 0,20 para WIN)

### To-dos

- [ ] Remover campo Folga do frontend (composable, páginas criar e visualizar)
- [ ] Remover campo Folga do backend (Model, DTO, Service, Repository)
- [ ] Remover coluna Folga da tabela Backtests no init.sql
- [ ] Criar BacktestAnalyzer.cs com lógica de análise de trades
- [ ] Criar BacktestProcessorService.cs como BackgroundService
- [ ] Criar BacktestResultado.cs para estrutura JSON dos resultados
- [ ] Registrar BacktestProcessorService e BacktestAnalyzer no Program.cs
- [ ] Testar fluxo completo de criação e processamento de Backtest