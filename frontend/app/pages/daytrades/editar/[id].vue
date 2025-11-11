<template>
  <div>
    <div class="level mb-5">
      <div class="level-left">
        <div class="level-item">
          <b-button tag="router-link" to="/daytrades" icon-left="arrow-left" type="is-text">
            Voltar
          </b-button>
        </div>
        <div class="level-item">
          <h1 class="title">Editar DayTrade</h1>
        </div>
      </div>
    </div>

    <div v-if="loading" class="has-text-centered">
      <b-loading :active="loading" :is-full-page="false"></b-loading>
      <p class="mt-5">Carregando...</p>
    </div>

    <div v-else-if="dayTrade">
      <!-- Informações do DayTrade (Readonly) -->
      <div class="box">
        <h2 class="title is-4 mb-4">Informações do DayTrade</h2>
        
        <b-field label="Dia do DayTrade">
          <b-input 
            :value="formatarData(dayTrade.diaDayTrade)" 
            disabled
          />
        </b-field>

        <b-field label="Ativo">
          <b-input 
            :value="`${dayTrade.ativoNome} - ${dayTrade.ativoCodigo}`" 
            disabled
          />
        </b-field>
      </div>

      <!-- Seção de Trades -->
      <div class="box mt-5">
        <h2 class="title is-4 mb-4">Trades:</h2>

        <!-- Tabela de Trades -->
        <div class="table-container">
          <table class="table is-fullwidth is-striped">
            <thead>
            <tr>
              <th>#</th>
              <th>Operação</th>
              <th>Estratégia</th>
              <th>
                Atenção
                <b-icon 
                  icon="help-circle-outline" 
                  size="is-small" 
                  class="ml-1 has-tooltip" 
                  title="Gatilho de Atenção"
                ></b-icon>
              </th>
              <th>
                Confirmação
                <b-icon 
                  icon="help-circle-outline" 
                  size="is-small" 
                  class="ml-1 has-tooltip" 
                  title="Gatilho de Confirmação"
                ></b-icon>
              </th>
              <th>
                Regiao
                <b-icon 
                  icon="help-circle-outline" 
                  size="is-small" 
                  class="ml-1 has-tooltip" 
                  title="Candle com pavil de Região"
                ></b-icon>
              </th>
              <th>Ações</th>
            </tr>
            </thead>
            <tbody>
              <!-- Trades salvos -->
              <tr v-for="(trade, index) in tradesSalvos" :key="trade.id">
                <td>{{ index + 1 }}</td>
                <td>
                  <span 
                    class="tag" 
                    :class="{
                      'is-success': trade.operacao === 'Compra',
                      'is-danger': trade.operacao === 'Venda'
                    }"
                  >
                    {{ trade.operacao }}
                  </span>
                </td>
                <td>{{ trade.estrategia || '-' }}</td>
                <td>{{ trade.gatilho1 }}</td>
                <td>{{ trade.gatilho2 }}</td>
                <td>{{ trade.regiao || '-' }}</td>
                <td>
                  <b-button
                    type="is-danger"
                    size="is-small"
                    icon-left="delete"
                    @click="confirmarDeletarTrade(trade.id)"
                  />
                </td>
              </tr>
              
              <!-- Linha para novo trade -->
              <tr>
                <td>{{ tradesSalvos.length + 1 }}</td>
                <td>
                  <b-select
                    v-model="novoTrade.operacao"
                    size="is-small"
                    expanded
                  >
                    <option value="Compra">Compra</option>
                    <option value="Venda">Venda</option>
                  </b-select>
                </td>
                <td>
                  <b-select
                    v-model="novoTrade.estrategia"
                    size="is-small"
                    expanded
                    placeholder="Selecione"
                  >
                    <option value="">Selecione uma estratégia</option>
                    <option v-for="estrategia in estrategiasDisponiveis" :key="estrategia" :value="estrategia">
                      {{ estrategia }}
                    </option>
                  </b-select>
                </td>
                <td>
                  <b-input
                    v-model.number="novoTrade.gatilho1"
                    type="number"
                    placeholder="Gatilho 1"
                    size="is-small"
                  />
                </td>
                <td>
                  <b-input
                    v-model.number="novoTrade.gatilho2"
                    type="number"
                    placeholder="Gatilho 2"
                    size="is-small"
                  />
                </td>
                <td>
                  <b-input
                    v-model.number="novoTrade.regiao"
                    type="number"
                    placeholder="Regiao"
                    size="is-small"
                  />
                </td>
                <td>
                  <b-button
                    type="is-success"
                    size="is-small"
                    icon-left="plus"
                    @click="salvarNovoTrade"
                    :loading="loadingSalvarTrade"
                  />
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <b-field grouped class="mt-5">
          <b-button
            type="is-light"
            icon-left="arrow-left"
            @click="voltar"
          >
            Voltar para Lista
          </b-button>
        </b-field>
      </div>

      <!-- Botão Flutuante para Criar Novo DayTrade -->
      <button 
        class="fab-button" 
        @click="criarNovoDayTrade" 
        title="Criar novo daytrade"
      >
        <b-icon icon="plus" size="is-medium"></b-icon>
      </button>
    </div>

    <div v-else class="box">
      <p class="has-text-centered has-text-danger">DayTrade não encontrado.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { DayTrade } from '~/composables/useDayTrades'
import type { Trade } from '~/composables/useTrades'
import { ESTRATEGIAS } from '~/constants/estrategias'

definePageMeta({
  layout: 'default',
  ssr: false
})

const estrategiasDisponiveis = [...ESTRATEGIAS]

const route = useRoute()
const router = useRouter()
const Toast = useBuefyToast()
const Dialog = useBuefyDialog()
const { obterDayTrade } = useDayTrades()
const { criarTrade, listarTradesPorDayTrade, deletarTrade } = useTrades()

const dayTradeId = computed(() => parseInt(route.params.id as string))
const dayTrade = ref<DayTrade | null>(null)
const tradesSalvos = ref<Trade[]>([])
const novoTrade = ref({
  gatilho1: null as number | null,
  gatilho2: null as number | null,
  regiao: null as number | null,
  operacao: 'Compra' as string,
  estrategia: '' as string
})

const loading = ref(false)
const loadingSalvarTrade = ref(false)

const formatarData = (dataString: string) => {
  const data = new Date(dataString)
  return data.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

const carregarDayTrade = async () => {
  loading.value = true
  try {
    dayTrade.value = await obterDayTrade(dayTradeId.value)
    
    if (dayTrade.value) {
      // Carregar trades
      tradesSalvos.value = await listarTradesPorDayTrade(dayTradeId.value)
    }
  } catch (error: any) {
    Toast.open({
      message: error.message || 'Erro ao carregar daytrade',
      type: 'is-danger',
      duration: 3000
    })
    
    // Redirecionar para lista após 2 segundos
    setTimeout(() => {
      router.push('/daytrades')
    }, 2000)
  } finally {
    loading.value = false
  }
}

const salvarNovoTrade = async () => {
  if (!novoTrade.value.gatilho1 || !novoTrade.value.gatilho2) {
    Toast.open({
      message: 'Preencha gatilho1 e gatilho2 antes de salvar',
      type: 'is-warning',
      duration: 3000
    })
    return
  }

  loadingSalvarTrade.value = true

  try {
    const tradeCriado = await criarTrade(
      dayTradeId.value,
      novoTrade.value.gatilho1,
      novoTrade.value.gatilho2,
      novoTrade.value.regiao,
      novoTrade.value.operacao,
      novoTrade.value.estrategia || undefined
    )
    
    tradesSalvos.value.push(tradeCriado)
    
    novoTrade.value = { gatilho1: null, gatilho2: null, regiao: null, operacao: 'Compra', estrategia: '' }
    
    Toast.open({
      message: 'Trade salvo com sucesso!',
      type: 'is-success',
      duration: 2000
    })
  } catch (error: any) {
    Toast.open({
      message: error.message || 'Erro ao salvar trade',
      type: 'is-danger',
      duration: 5000
    })
  } finally {
    loadingSalvarTrade.value = false
  }
}

const confirmarDeletarTrade = (id: number) => {
  Dialog.confirm({
    title: 'Confirmar exclusão',
    message: 'Tem certeza que deseja deletar este trade?',
    confirmText: 'Deletar',
    cancelText: 'Cancelar',
    type: 'is-danger',
    hasIcon: true,
    onConfirm: () => deletarTradeConfirmado(id)
  })
}

const deletarTradeConfirmado = async (id: number) => {
  try {
    await deletarTrade(id)
    
    tradesSalvos.value = tradesSalvos.value.filter(t => t.id !== id)
    
    Toast.open({
      message: 'Trade deletado com sucesso!',
      type: 'is-success',
      duration: 2000
    })
  } catch (error: any) {
    Toast.open({
      message: error.message || 'Erro ao deletar trade',
      type: 'is-danger',
      duration: 3000
    })
  }
}

const voltar = () => {
  router.push('/daytrades')
}

const criarNovoDayTrade = () => {
  router.push('/daytrades/criar')
}

onMounted(() => {
  carregarDayTrade()
})
</script>

<style scoped>
.fab-button {
  position: fixed;
  bottom: 2rem;
  right: 2rem;
  width: 56px;
  height: 56px;
  border-radius: 50%;
  background-color: #3273dc;
  color: white;
  border: none;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.4);
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
  z-index: 100;
}

.fab-button:hover {
  background-color: #2366d1;
  transform: scale(1.1);
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.5);
}

.fab-button:active {
  transform: scale(0.95);
}

/* Estilo para tooltip */
.has-tooltip {
  cursor: help;
  color: #7a7a7a;
  transition: color 0.2s;
}

.has-tooltip:hover {
  color: #3273dc;
}
</style>

