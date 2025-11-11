<template>
  <div>
    <div class="level mb-5">
      <div class="level-left">
        <div class="level-item">
          <b-button tag="router-link" to="/backtests" icon-left="arrow-left" type="is-text">
            Voltar
          </b-button>
        </div>
        <div class="level-item">
          <h1 class="title">Visualizar Backtest #{{ backtestId }}</h1>
        </div>
      </div>
    </div>

    <div v-if="loading && !backtest" class="has-text-centered">
      <b-loading :active="loading" :is-full-page="false"></b-loading>
      <p class="mt-5">Carregando...</p>
    </div>

    <div v-else-if="backtest">
      <!-- Informações do Backtest -->
      <div class="box">
        <div class="level mb-4">
          <div class="level-left">
            <h2 class="title is-4">Informações do Backtest</h2>
          </div>
          <div class="level-right">
            <span 
              class="tag is-large" 
              :class="{
                'is-warning': backtest.status === 'Iniciado',
                'is-success': backtest.status === 'Finalizado',
                'is-danger': backtest.status === 'Erro'
              }"
            >
              {{ backtest.status }}
            </span>
          </div>
        </div>

        <div class="columns">
          <div class="column">
            <b-field label="Período">
              <b-input 
                :value="`${formatarData(backtest.dataInicio)} - ${formatarData(backtest.dataFim)}`" 
                disabled
              />
            </b-field>
          </div>

          <div class="column">
            <b-field label="Ativo">
              <b-input 
                :value="`${backtest.ativoNome} - ${backtest.ativoCodigo}`" 
                disabled
              />
            </b-field>
          </div>
        </div>

        <div class="columns">
          <div class="column">
            <b-field label="Entrada (PTS)">
              <b-input :value="backtest.entrada" disabled />
            </b-field>
          </div>

          <div class="column">
            <b-field label="Alvo (PTS)">
              <b-input :value="backtest.alvo" disabled />
            </b-field>
          </div>

          <div class="column">
            <b-field label="Nº de Contratos">
              <b-input :value="backtest.numeroContratos" disabled />
            </b-field>
          </div>
        </div>

        <div class="columns">
          <div class="column">
            <b-field label="Stop (PTS)">
              <b-input :value="backtest.stop" disabled />
            </b-field>
          </div>

          <div class="column">
            <b-field label="Proteger (1:1)">
              <b-input :value="backtest.proteger ? 'Sim' : 'Não'" disabled />
            </b-field>
          </div>
        </div>

        <div class="columns">
          <div class="column">
            <b-field label="Estratégias Selecionadas">
              <div class="tags">
                <span 
                  v-for="estrategia in estrategiasParsed" 
                  :key="estrategia" 
                  class="tag is-info is-medium"
                >
                  {{ estrategia }}
                </span>
              </div>
            </b-field>
          </div>
        </div>

        <div class="columns">
          <div class="column">
            <b-field label="Data de Criação">
              <b-input :value="formatarDataHora(backtest.dataCriacao)" disabled />
            </b-field>
          </div>

          <div class="column" v-if="backtest.dataFinalizacao">
            <b-field label="Data de Finalização">
              <b-input :value="formatarDataHora(backtest.dataFinalizacao)" disabled />
            </b-field>
          </div>
        </div>
      </div>

      <!-- Resultados (se houver) -->
      <div v-if="backtest.status === 'Finalizado' && resultadoParsed">
        <!-- Sugestão de Otimização -->
        <div class="box mt-5" v-if="sugestaoOtimizacao && resultadoParsed.Trades.length >= 3">
          <h2 class="title is-4 mb-4">💡 Sugestão de Otimização (Análise Conservadora)</h2>
          
          <div class="notification is-info is-light">
            <p class="mb-3">
              <strong>Análise baseada em {{ resultadoParsed.Trades.length }} trade(s)</strong> 
              usando MEDIANA e PERCENTIS para evitar distorções por trades extremos.
            </p>
          </div>

          <div class="columns is-multiline">
            <!-- Setup Atual -->
            <div class="column is-6">
              <div class="card">
                <div class="card-content">
                  <h3 class="subtitle is-5 mb-3">⚙️ Setup Atual</h3>
                  <div class="content">
                    <p><strong>Entrada:</strong> +{{ backtest.entrada }} PTS</p>
                    <p><strong>Stop:</strong> {{ backtest.stop }} PTS</p>
                    <p><strong>Alvo:</strong> {{ backtest.alvo }} PTS</p>
                    <p class="has-text-info"><strong>Risk/Reward:</strong> 1:{{ (backtest.alvo / backtest.stop).toFixed(1) }}</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Setup Sugerido -->
            <div class="column is-6">
              <div class="card has-background-success-light">
                <div class="card-content">
                  <h3 class="subtitle is-5 mb-3 has-text-success">✨ Setup Otimizado Sugerido</h3>
                  <div class="content">
                    <p>
                      <strong>Entrada:</strong> +{{ sugestaoOtimizacao.entradaSugerida }} PTS 
                      <span class="tag is-success is-small">{{ sugestaoOtimizacao.melhoriaEntrada }} PTS melhor</span>
                    </p>
                    <p>
                      <strong>Stop:</strong> {{ sugestaoOtimizacao.stopSugerido }} PTS
                      <span class="tag is-success is-small">-{{ sugestaoOtimizacao.reducaoStop }} PTS</span>
                    </p>
                    <p>
                      <strong>Alvo:</strong> {{ sugestaoOtimizacao.alvoSugerido }} PTS
                      <span class="tag is-info is-small">+{{ sugestaoOtimizacao.aumentoAlvo }} PTS</span>
                    </p>
                    <p class="has-text-success">
                      <strong>Risk/Reward:</strong> 1:{{ sugestaoOtimizacao.novoRiskReward }} 
                      <span class="tag is-success">🚀 {{ sugestaoOtimizacao.melhoriaRR }}% melhor</span>
                    </p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Impacto Projetado -->
            <div class="column is-12" v-if="sugestaoOtimizacao.impactoProjetado">
              <div class="card has-background-warning-light">
                <div class="card-content">
                  <h3 class="subtitle is-5 mb-3 has-text-warning-dark">📈 Impacto Projetado</h3>
                  <div class="columns">
                    <div class="column">
                      <p><strong>Com parâmetros atuais:</strong></p>
                      <p class="title is-5">{{ sugestaoOtimizacao.impactoProjetado.saldoAtual }} PTS</p>
                    </div>
                    <div class="column">
                      <p><strong>Com parâmetros otimizados:</strong></p>
                      <p class="title is-5 has-text-success">{{ sugestaoOtimizacao.impactoProjetado.saldoOtimizado }} PTS</p>
                    </div>
                    <div class="column">
                      <p><strong>Melhoria potencial:</strong></p>
                      <p class="title is-5 has-text-info">+{{ sugestaoOtimizacao.impactoProjetado.melhoria }} PTS ({{ sugestaoOtimizacao.impactoProjetado.melhoriaPercentual }}%)</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Explicação -->
            <div class="column is-12">
              <div class="message is-info">
                <div class="message-body">
                  <p class="mb-2"><strong>Como chegamos nesses valores:</strong></p>
                  <ul>
                    <li><strong>Entrada:</strong> {{ sugestaoOtimizacao.explicacaoEntrada }}</li>
                    <li><strong>Alvo:</strong> {{ sugestaoOtimizacao.explicacaoAlvo }}</li>
                  </ul>
                  <p class="mt-3 has-text-info">
                    <b-icon icon="information" size="is-small"></b-icon>
                    <strong>Importante:</strong> Teste esses parâmetros em um período diferente (out-of-sample) antes de usar em operação real.
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Cards de Resumo -->
        <div class="box mt-5">
          <h2 class="title is-4 mb-5">📊 Resumo do Backtest</h2>
          
          <div class="columns is-multiline">
            <!-- Saldo Final -->
            <div class="column is-4">
              <div class="card" :class="resultadoParsed.SaldoPts >= 0 ? 'has-background-success-light' : 'has-background-danger-light'">
                <div class="card-content">
                  <p class="heading">Saldo Final</p>
                  <p class="title" :class="resultadoParsed.SaldoPts >= 0 ? 'has-text-success' : 'has-text-danger'">
                    {{ resultadoParsed.SaldoPts }} PTS
                  </p>
                  <p class="subtitle is-6" :class="resultadoParsed.SaldoReais >= 0 ? 'has-text-success' : 'has-text-danger'">
                    R$ {{ resultadoParsed.SaldoReais.toFixed(2) }}
                  </p>
                </div>
              </div>
            </div>

            <!-- Win Rate -->
            <div class="column is-4">
              <div class="card has-background-info-light">
                <div class="card-content">
                  <p class="heading">Win Rate</p>
                  <p class="title has-text-info">{{ resultadoParsed.WinRate.toFixed(1) }}%</p>
                  <p class="subtitle is-6 has-text-info">
                    {{ resultadoParsed.TotalGains }}G / {{ resultadoParsed.TotalStops }}S
                  </p>
                </div>
              </div>
            </div>

            <!-- Múltiplo do Stop -->
            <div class="column is-4">
              <div class="card has-background-warning-light">
                <div class="card-content">
                  <p class="heading">Múltiplo do Stop</p>
                  <p class="title has-text-warning">{{ resultadoParsed.MultiploStop.toFixed(1) }}x</p>
                  <p class="subtitle is-6 has-text-warning">
                    Stop: {{ backtest.stop }} PTS
                  </p>
                </div>
              </div>
            </div>

            <!-- Gains -->
            <div class="column is-6">
              <div class="card has-background-success-light">
                <div class="card-content">
                  <p class="heading">💰 Total Gains</p>
                  <p class="title has-text-success">{{ resultadoParsed.TotalGains }} trades</p>
                  <p class="subtitle is-6 has-text-success">
                    {{ resultadoParsed.TotalGainsPts }} PTS • R$ {{ resultadoParsed.TotalGainsReais.toFixed(2) }}
                  </p>
                </div>
              </div>
            </div>

            <!-- Stops -->
            <div class="column is-6">
              <div class="card has-background-danger-light">
                <div class="card-content">
                  <p class="heading">🛑 Total Stops</p>
                  <p class="title has-text-danger">{{ resultadoParsed.TotalStops }} trades</p>
                  <p class="subtitle is-6 has-text-danger">
                    {{ resultadoParsed.TotalStopsPts }} PTS • R$ {{ resultadoParsed.TotalStopsReais.toFixed(2) }}
                  </p>
                </div>
              </div>
            </div>

            <!-- Sequências -->
            <div class="column is-6">
              <div class="card">
                <div class="card-content">
                  <p class="heading">📈 Maior Sequência de Gains</p>
                  <p class="title">{{ resultadoParsed.MaiorSequenciaGains }}</p>
                </div>
              </div>
            </div>

            <div class="column is-6">
              <div class="card">
                <div class="card-content">
                  <p class="heading">📉 Maior Sequência de Stops</p>
                  <p class="title">{{ resultadoParsed.MaiorSequenciaStops }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Tabela de Trades -->
        <div class="box mt-5" v-if="resultadoParsed.Trades && resultadoParsed.Trades.length > 0">
          <h2 class="title is-4 mb-4">📋 Trades Executados</h2>
          
          <b-table 
            :data="resultadoParsed.Trades" 
            striped 
            hoverable
            :mobile-cards="false"
            detailed
            detail-key="TradeId"
            :show-detail-icon="true"
          >
            <b-table-column field="Data" label="Data" v-slot="props">
              {{ formatarDataSimples(props.row.Data) }}
            </b-table-column>

            <b-table-column field="Horario" label="Horário" v-slot="props">
              {{ props.row.Horario }}
            </b-table-column>

            <b-table-column field="Estrategia" label="Estratégia" v-slot="props">
              <span class="tag is-info">{{ props.row.Estrategia }}</span>
            </b-table-column>

            <b-table-column field="Operacao" label="Operação" v-slot="props">
              <span 
                class="tag" 
                :class="props.row.Operacao === 'Compra' ? 'is-success' : 'is-danger'"
              >
                {{ props.row.Operacao }}
              </span>
            </b-table-column>

            <b-table-column field="PrecoEntrada" label="Entrada" v-slot="props" numeric>
              {{ props.row.PrecoEntrada.toFixed(2) }}
            </b-table-column>

            <b-table-column field="PrecoSaida" label="Saída" v-slot="props" numeric>
              {{ props.row.PrecoSaida.toFixed(2) }}
            </b-table-column>

            <b-table-column field="Resultado" label="Resultado" v-slot="props" centered>
              <span 
                class="tag is-medium" 
                :class="props.row.Resultado === 'Gain' ? 'is-success' : 'is-danger'"
              >
                {{ props.row.Resultado }}
              </span>
            </b-table-column>

            <b-table-column field="Pts" label="PTS" v-slot="props" numeric>
              <strong :class="props.row.Pts >= 0 ? 'has-text-success' : 'has-text-danger'">
                {{ props.row.Pts > 0 ? '+' : '' }}{{ props.row.Pts }}
              </strong>
            </b-table-column>

            <b-table-column field="Reais" label="R$" v-slot="props" numeric>
              <strong :class="props.row.Reais >= 0 ? 'has-text-success' : 'has-text-danger'">
                {{ props.row.Reais > 0 ? '+' : '' }}{{ props.row.Reais.toFixed(2) }}
              </strong>
            </b-table-column>

            <b-table-column label="Análises" v-slot="props" centered>
              <b-tooltip 
                v-if="props.row.MelhorOportunidadeEntrada" 
                :label="`Preço voltou ${props.row.MelhorOportunidadeEntrada} PTS a favor após entrada`"
                type="is-warning"
                position="is-left"
              >
                <b-tag type="is-warning">↩️ {{ props.row.MelhorOportunidadeEntrada }}</b-tag>
              </b-tooltip>
              
              <b-tooltip 
                v-if="props.row.ExtensaoAposAlvo" 
                :label="`Preço continuou ${props.row.ExtensaoAposAlvo} PTS após alvo`"
                type="is-info"
                position="is-left"
                class="ml-1"
              >
                <b-tag type="is-info">📈 {{ props.row.ExtensaoAposAlvo }}</b-tag>
              </b-tooltip>
              
              <b-tooltip 
                v-if="props.row.MaximaEvolucaoFavor" 
                :label="`Evoluiu ${props.row.MaximaEvolucaoFavor} PTS a favor antes do stop`"
                type="is-primary"
                position="is-left"
                class="ml-1"
              >
                <b-tag type="is-primary">⬆️ {{ props.row.MaximaEvolucaoFavor }}</b-tag>
              </b-tooltip>

              <span v-if="!props.row.MelhorOportunidadeEntrada && !props.row.ExtensaoAposAlvo && !props.row.MaximaEvolucaoFavor" class="has-text-grey">
                -
              </span>
            </b-table-column>

            <template #detail="props">
              <div class="detail-container">
                <div class="columns is-multiline">
                  <div class="column is-12">
                    <h4 class="subtitle is-5 mb-3 has-text-white">🎯 Candles de Referência</h4>
                  </div>
                  
                  <div class="column is-3">
                    <div class="candle-card has-background-info-light">
                      <p class="candle-label has-text-info">🔔 ATENÇÃO</p>
                      <p class="candle-number has-text-info">{{ props.row.CandleAtencao }}</p>
                      <p class="candle-description has-text-grey">Primeiro gatilho</p>
                    </div>
                  </div>

                  <div class="column is-3">
                    <div class="candle-card has-background-success-light">
                      <p class="candle-label has-text-success">✅ CONFIRMAÇÃO</p>
                      <p class="candle-number has-text-success">{{ props.row.CandleConfirmacao }}</p>
                      <p class="candle-description has-text-grey">Segundo gatilho</p>
                    </div>
                  </div>

                  <div class="column is-3" v-if="props.row.CandleRegiao">
                    <div class="candle-card has-background-warning-light">
                      <p class="candle-label has-text-warning">📍 REGIÃO</p>
                      <p class="candle-number has-text-warning">{{ props.row.CandleRegiao }}</p>
                      <p class="candle-description has-text-grey">Pavio de região</p>
                    </div>
                  </div>

                  <div class="column is-3" v-else>
                    <div class="candle-card has-background-light">
                      <p class="candle-label has-text-grey">📍 REGIÃO</p>
                      <p class="candle-number has-text-grey">-</p>
                      <p class="candle-description has-text-grey">Não especificado</p>
                    </div>
                  </div>

                  <div class="column is-3">
                    <div class="candle-card has-background-primary-light">
                      <p class="candle-label has-text-primary">🚀 ENTRADA</p>
                      <p class="candle-number has-text-primary">{{ props.row.CandleEntrada }}</p>
                      <p class="candle-description has-text-grey">Candle que ativou</p>
                    </div>
                  </div>
                </div>
              </div>
            </template>
          </b-table>
        </div>

        <!-- Análise por Estratégia -->
        <div class="box mt-5" v-if="Object.keys(resultadoParsed.PorEstrategia || {}).length > 0">
          <h2 class="title is-4 mb-4">🎯 Análise por Estratégia</h2>
          
          <b-table 
            :data="Object.entries(resultadoParsed.PorEstrategia).map(([nome, stats]) => ({ nome, ...stats }))" 
            striped
          >
            <b-table-column field="nome" label="Estratégia" v-slot="props">
              <span class="tag is-info is-medium">{{ props.row.nome }}</span>
            </b-table-column>

            <b-table-column field="TotalTrades" label="Total" v-slot="props" centered>
              {{ props.row.TotalTrades }}
            </b-table-column>

            <b-table-column field="Gains" label="Gains" v-slot="props" centered>
              <span class="has-text-success">{{ props.row.Gains }}</span>
            </b-table-column>

            <b-table-column field="Stops" label="Stops" v-slot="props" centered>
              <span class="has-text-danger">{{ props.row.Stops }}</span>
            </b-table-column>

            <b-table-column field="WinRate" label="Win Rate" v-slot="props" centered>
              <strong>{{ props.row.WinRate.toFixed(1) }}%</strong>
            </b-table-column>

            <b-table-column field="SaldoPts" label="Saldo (PTS)" v-slot="props" numeric>
              <strong :class="props.row.SaldoPts >= 0 ? 'has-text-success' : 'has-text-danger'">
                {{ props.row.SaldoPts > 0 ? '+' : '' }}{{ props.row.SaldoPts }}
              </strong>
            </b-table-column>

            <b-table-column field="SaldoReais" label="Saldo (R$)" v-slot="props" numeric>
              <strong :class="props.row.SaldoReais >= 0 ? 'has-text-success' : 'has-text-danger'">
                {{ props.row.SaldoReais > 0 ? '+' : '' }}{{ props.row.SaldoReais.toFixed(2) }}
              </strong>
            </b-table-column>
          </b-table>
        </div>

        <!-- Análise por Dia da Semana -->
        <div class="box mt-5" v-if="Object.keys(resultadoParsed.PorDiaSemana || {}).length > 0">
          <h2 class="title is-4 mb-4">📅 Análise por Dia da Semana</h2>
          
          <b-table 
            :data="formatarDiasSemana(resultadoParsed.PorDiaSemana)" 
            striped
          >
            <b-table-column field="dia" label="Dia" v-slot="props">
              <strong class="has-text-white">{{ props.row.dia }}</strong>
            </b-table-column>

            <b-table-column field="TotalTrades" label="Total" v-slot="props" centered>
              {{ props.row.TotalTrades }}
            </b-table-column>

            <b-table-column field="Gains" label="Gains" v-slot="props" centered>
              <span class="has-text-success">{{ props.row.Gains }}</span>
            </b-table-column>

            <b-table-column field="Stops" label="Stops" v-slot="props" centered>
              <span class="has-text-danger">{{ props.row.Stops }}</span>
            </b-table-column>

            <b-table-column field="SaldoPts" label="Saldo (PTS)" v-slot="props" numeric>
              <strong :class="props.row.SaldoPts >= 0 ? 'has-text-success' : 'has-text-danger'">
                {{ props.row.SaldoPts > 0 ? '+' : '' }}{{ props.row.SaldoPts }}
              </strong>
            </b-table-column>

            <b-table-column field="SaldoReais" label="Saldo (R$)" v-slot="props" numeric>
              <strong :class="props.row.SaldoReais >= 0 ? 'has-text-success' : 'has-text-danger'">
                {{ props.row.SaldoReais > 0 ? '+' : '' }}{{ props.row.SaldoReais.toFixed(2) }}
              </strong>
            </b-table-column>
          </b-table>
        </div>

        <!-- Análise por Horário -->
        <div class="box mt-5" v-if="Object.keys(resultadoParsed.PorHorario || {}).length > 0">
          <h2 class="title is-4 mb-4">🕐 Análise por Horário</h2>
          
          <b-table 
            :data="formatarHorarios(resultadoParsed.PorHorario)" 
            striped
          >
            <b-table-column field="horario" label="Horário" v-slot="props">
              <strong class="has-text-white">{{ props.row.horario }}h</strong>
            </b-table-column>

            <b-table-column field="TotalTrades" label="Total" v-slot="props" centered>
              {{ props.row.TotalTrades }}
            </b-table-column>

            <b-table-column field="Gains" label="Gains" v-slot="props" centered>
              <span class="has-text-success">{{ props.row.Gains }}</span>
            </b-table-column>

            <b-table-column field="Stops" label="Stops" v-slot="props" centered>
              <span class="has-text-danger">{{ props.row.Stops }}</span>
            </b-table-column>

            <b-table-column field="SaldoPts" label="Saldo (PTS)" v-slot="props" numeric>
              <strong :class="props.row.SaldoPts >= 0 ? 'has-text-success' : 'has-text-danger'">
                {{ props.row.SaldoPts > 0 ? '+' : '' }}{{ props.row.SaldoPts }}
              </strong>
            </b-table-column>

            <b-table-column field="SaldoReais" label="Saldo (R$)" v-slot="props" numeric>
              <strong :class="props.row.SaldoReais >= 0 ? 'has-text-success' : 'has-text-danger'">
                {{ props.row.SaldoReais > 0 ? '+' : '' }}{{ props.row.SaldoReais.toFixed(2) }}
              </strong>
            </b-table-column>
          </b-table>
        </div>

        <!-- Avisos -->
        <div class="box mt-5" v-if="resultadoParsed.TradesNaoEntraram > 0 || (resultadoParsed.Erros && resultadoParsed.Erros.length > 0)">
          <h2 class="title is-4 mb-4">⚠️ Avisos</h2>
          
          <div v-if="resultadoParsed.TradesNaoEntraram > 0" class="notification is-warning">
            <strong>{{ resultadoParsed.TradesNaoEntraram }}</strong> trade(s) não entraram (entrada não foi ativada).
          </div>

          <div v-if="resultadoParsed.Erros && resultadoParsed.Erros.length > 0" class="notification is-danger">
            <p><strong>Erros encontrados:</strong></p>
            <ul>
              <li v-for="(erro, index) in resultadoParsed.Erros" :key="index">{{ erro }}</li>
            </ul>
          </div>
        </div>
      </div>

      <!-- Mensagem de Processamento -->
      <div v-if="backtest.status === 'Iniciado'" class="box mt-5 has-text-centered">
        <b-loading :active="true" :is-full-page="false"></b-loading>
        <p class="mt-5 is-size-5">
          <b-icon icon="clock-outline" size="is-medium"></b-icon>
          Processando backtest... Aguarde.
        </p>
        <p class="has-text-grey mt-2">
          Esta página será atualizada automaticamente a cada 5 segundos.
        </p>
      </div>

      <!-- Mensagem de Erro -->
      <div v-if="backtest.status === 'Erro'" class="box mt-5 has-background-danger-light">
        <h2 class="title is-4 has-text-danger">
          <b-icon icon="alert-circle" size="is-medium"></b-icon>
          Erro no Processamento
        </h2>
        <p class="has-text-danger">{{ backtest.resultado || 'Ocorreu um erro ao processar o backtest.' }}</p>
      </div>
    </div>

    <div v-else class="box">
      <p class="has-text-centered has-text-danger">Backtest não encontrado.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Backtest } from '~/composables/useBacktests'

definePageMeta({
  layout: 'default',
  ssr: false
})

const route = useRoute()
const router = useRouter()
const Toast = useBuefyToast()
const { obterBacktest } = useBacktests()

const backtestId = computed(() => parseInt(route.params.id as string))
const backtest = ref<Backtest | null>(null)
const loading = ref(false)
let pollingInterval: NodeJS.Timeout | null = null

const estrategiasParsed = computed(() => {
  if (!backtest.value || !backtest.value.estrategias) return []
  try {
    return JSON.parse(backtest.value.estrategias) as string[]
  } catch {
    return []
  }
})

const resultadoParsed = computed(() => {
  if (!backtest.value || !backtest.value.resultado) return null
  try {
    return JSON.parse(backtest.value.resultado)
  } catch {
    return null
  }
})

const sugestaoOtimizacao = computed(() => {
  if (!resultadoParsed.value || !backtest.value || resultadoParsed.value.Trades.length < 3) return null
  
  const trades = resultadoParsed.value.Trades
  
  // Coletar dados de otimização apenas de trades com Gain
  const melhorOportunidadeValues = trades
    .filter(t => t.Resultado === 'Gain' && t.MelhorOportunidadeEntrada)
    .map(t => t.MelhorOportunidadeEntrada)
  
  const extensaoAposAlvoValues = trades
    .filter(t => t.Resultado === 'Gain' && t.ExtensaoAposAlvo)
    .map(t => t.ExtensaoAposAlvo)
  
  // Se não tiver dados suficientes, não mostra sugestão
  if (melhorOportunidadeValues.length === 0 && extensaoAposAlvoValues.length === 0) return null
  
  // Função para calcular percentil
  const percentil = (arr: number[], p: number) => {
    if (arr.length === 0) return 0
    const sorted = [...arr].sort((a, b) => a - b)
    const index = Math.ceil((p / 100) * sorted.length) - 1
    return sorted[Math.max(0, index)]
  }
  
  // Calcular melhorias CONSERVADORAS
  // Entrada: Percentil 75 (75% dos trades voltaram ATÉ esse valor)
  const melhoriaEntradaConservadora = melhorOportunidadeValues.length > 0
    ? Math.round(percentil(melhorOportunidadeValues, 75))
    : 0
  
  // Alvo: Percentil 30 (30% dos trades foram PELO MENOS esse valor) - conservador
  const aumentoAlvoConservador = extensaoAposAlvoValues.length > 0
    ? Math.round(percentil(extensaoAposAlvoValues, 30))
    : 0
  
  // Calcular novos parâmetros
  const entradaSugerida = Math.max(10, backtest.value.entrada - melhoriaEntradaConservadora)
  const stopSugerido = Math.max(10, backtest.value.stop - melhoriaEntradaConservadora)
  const alvoSugerido = backtest.value.alvo + aumentoAlvoConservador
  
  const reducaoStop = backtest.value.stop - stopSugerido
  
  const rrAtual = backtest.value.alvo / backtest.value.stop
  const rrNovo = alvoSugerido / stopSugerido
  const melhoriaRR = Math.round(((rrNovo - rrAtual) / rrAtual) * 100)
  
  // Calcular impacto projetado (simulação simples)
  const totalTrades = trades.length
  const saldoAtual = resultadoParsed.value.SaldoPts
  
  // Simula: mesmos trades, mas com novos parâmetros
  let saldoOtimizado = 0
  trades.forEach(trade => {
    if (trade.Resultado === 'Gain') {
      saldoOtimizado += alvoSugerido // Alvo maior
    } else {
      saldoOtimizado -= stopSugerido // Stop menor
    }
  })
  
  const melhoria = saldoOtimizado - saldoAtual
  const melhoriaPercentual = saldoAtual !== 0 ? Math.round((melhoria / Math.abs(saldoAtual)) * 100) : 0
  
  // Explicações
  let explicacaoEntrada = ''
  if (melhorOportunidadeValues.length > 0) {
    explicacaoEntrada = `75% dos trades com gain voltaram pelo menos ${melhoriaEntradaConservadora} PTS a favor após entrada. Reduzindo a entrada, você diminui o risco.`
  } else {
    explicacaoEntrada = 'Sem dados suficientes de pullback para sugerir otimização.'
  }
  
  let explicacaoAlvo = ''
  if (extensaoAposAlvoValues.length > 0) {
    explicacaoAlvo = `30% dos trades com gain continuaram pelo menos ${aumentoAlvoConservador} PTS após o alvo. Aumentando conservadoramente o alvo, você captura mais lucro consistente.`
  } else {
    explicacaoAlvo = 'Sem dados suficientes de extensão para sugerir otimização.'
  }
  
  return {
    entradaSugerida,
    stopSugerido,
    alvoSugerido,
    melhoriaEntrada: melhoriaEntradaConservadora,
    reducaoStop,
    aumentoAlvo: aumentoAlvoConservador,
    novoRiskReward: rrNovo.toFixed(1),
    melhoriaRR,
    impactoProjetado: {
      saldoAtual,
      saldoOtimizado,
      melhoria,
      melhoriaPercentual
    },
    explicacaoEntrada,
    explicacaoAlvo
  }
})

const formatarData = (dataString: string) => {
  const data = new Date(dataString)
  return data.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

const formatarDataHora = (dataString: string) => {
  const data = new Date(dataString)
  return data.toLocaleString('pt-BR', { 
    day: '2-digit', 
    month: '2-digit', 
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const formatarDataSimples = (dataString: string) => {
  const data = new Date(dataString)
  return data.toLocaleDateString('pt-BR')
}

const diasSemanaMap: Record<string, string> = {
  'Sunday': 'Domingo',
  'Monday': 'Segunda-feira',
  'Tuesday': 'Terça-feira',
  'Wednesday': 'Quarta-feira',
  'Thursday': 'Quinta-feira',
  'Friday': 'Sexta-feira',
  'Saturday': 'Sábado'
}

const formatarDiasSemana = (porDiaSemana: any) => {
  return Object.entries(porDiaSemana).map(([dia, stats]: [string, any]) => ({
    dia: diasSemanaMap[dia] || dia,
    ...stats
  }))
}

const formatarHorarios = (porHorario: any) => {
  return Object.entries(porHorario)
    .map(([hora, stats]: [string, any]) => ({
      horario: hora,
      ...stats
    }))
    .sort((a, b) => parseInt(a.horario) - parseInt(b.horario))
}

const carregarBacktest = async () => {
  loading.value = true
  try {
    backtest.value = await obterBacktest(backtestId.value)
    
    if (!backtest.value) {
      Toast.open({
        message: 'Backtest não encontrado',
        type: 'is-danger',
        duration: 3000
      })
      
      setTimeout(() => {
        router.push('/backtests')
      }, 2000)
    }
  } catch (error: any) {
    Toast.open({
      message: error.message || 'Erro ao carregar backtest',
      type: 'is-danger',
      duration: 3000
    })
    
    setTimeout(() => {
      router.push('/backtests')
    }, 2000)
  } finally {
    loading.value = false
  }
}

const iniciarPolling = () => {
  pollingInterval = setInterval(async () => {
    if (backtest.value && backtest.value.status === 'Iniciado') {
      try {
        const backtestAtualizado = await obterBacktest(backtestId.value)
        if (backtestAtualizado) {
          backtest.value = backtestAtualizado
          
          // Se mudou o status, para o polling
          if (backtestAtualizado.status !== 'Iniciado') {
            pararPolling()
            
            if (backtestAtualizado.status === 'Finalizado') {
              Toast.open({
                message: 'Backtest finalizado com sucesso!',
                type: 'is-success',
                duration: 3000
              })
            } else if (backtestAtualizado.status === 'Erro') {
              Toast.open({
                message: 'Erro ao processar backtest',
                type: 'is-danger',
                duration: 3000
              })
            }
          }
        }
      } catch (error) {
        console.error('Erro no polling:', error)
      }
    } else {
      // Se não está mais "Iniciado", para o polling
      pararPolling()
    }
  }, 5000) // Atualiza a cada 5 segundos
}

const pararPolling = () => {
  if (pollingInterval) {
    clearInterval(pollingInterval)
    pollingInterval = null
  }
}

onMounted(async () => {
  await carregarBacktest()
  
  // Inicia polling se o status for "Iniciado"
  if (backtest.value && backtest.value.status === 'Iniciado') {
    iniciarPolling()
  }
})

onUnmounted(() => {
  pararPolling()
})
</script>

<style scoped>
.detail-container {
  background-color: #1a1a1a;
  padding: 1.5rem;
  border-radius: 8px;
  margin: 0.5rem 0;
}

.candle-card {
  border-radius: 12px;
  padding: 1.5rem 1rem;
  text-align: center;
  min-height: 140px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  gap: 0.5rem;
}

.candle-label {
  font-size: 0.75rem;
  font-weight: 700;
  letter-spacing: 0.5px;
  text-transform: uppercase;
  margin: 0;
  line-height: 1.2;
}

.candle-number {
  font-size: 2.5rem;
  font-weight: 800;
  margin: 0.5rem 0;
  line-height: 1;
}

.candle-description {
  font-size: 0.875rem;
  margin: 0;
  line-height: 1.2;
}
</style>

