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
          <h1 class="title">Criar DayTrade</h1>
        </div>
      </div>
    </div>

    <div class="box">
      <form @submit.prevent="salvarDayTrade">
        <b-field
          label="Dia do DayTrade"
          :type="submitted && !form.diaDayTrade ? 'is-danger' : ''"
          :message="submitted && !form.diaDayTrade ? 'Dia do DayTrade é obrigatório' : ''"
        >
          <b-datepicker
            v-model="form.diaDayTrade"
            placeholder="Selecione uma data"
            icon="calendar"
            :date-formatter="formatarData"
            :date-parser="parseData"
            editable
            trap-focus
            :disabled="dayTradeSalvo"
          >
          </b-datepicker>
        </b-field>

        <b-field
          label="Ativo"
          :type="submitted && !form.ativoId ? 'is-danger' : ''"
          :message="submitted && !form.ativoId ? 'Ativo é obrigatório' : ''"
        >
          <b-select
            v-model="form.ativoId"
            placeholder="Selecione um ativo"
            expanded
            :loading="loadingAtivos"
            :disabled="dayTradeSalvo"
          >
            <option 
              v-for="ativo in ativos" 
              :key="ativo.id" 
              :value="ativo.id"
            >
              {{ ativo.nome }} - {{ ativo.codigo }} ({{ ativo.timeframe }})
            </option>
          </b-select>
        </b-field>

        <b-field grouped class="mt-5" v-if="!dayTradeSalvo">
          <b-button
            native-type="submit"
            type="is-primary"
            icon-left="check"
            :loading="loading"
            class="mr-3"
          >
            Salvar
          </b-button>
          <b-button
            type="is-light"
            icon-left="close"
            @click="voltar"
            :disabled="loading"
          >
            Cancelar
          </b-button>
        </b-field>
      </form>
    </div>

    <!-- Seção de Trades -->
    <div v-if="dayTradeSalvo" class="box mt-5">
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
                  <option value="Pullback de lado">Pullback de lado</option>
                  <option value="Pullback na inversão">Pullback na inversão</option>
                  <option value="Pullback no 50% macro">Pullback no 50% macro</option>
                  <option value="Pullback no 50% micro">Pullback no 50% micro</option>
                  <option value="Consolidação">Consolidação</option>
                  <option value="Quebra micro">Quebra micro</option>
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
      v-if="dayTradeSalvo" 
      class="fab-button" 
      @click="criarNovoDayTrade" 
      title="Criar novo daytrade"
    >
      <b-icon icon="plus" size="is-medium"></b-icon>
    </button>
  </div>
</template>

<script setup lang="ts">
import type { Ativo } from '~/composables/useAtivos'
import type { Trade } from '~/composables/useTrades'

definePageMeta({
  layout: 'default',
  ssr: false
})

const router = useRouter()
const Toast = useBuefyToast()
const Dialog = useBuefyDialog()
const { listarAtivos } = useAtivos()
const { criarDayTrade } = useDayTrades()
const { criarTrade, listarTradesPorDayTrade, deletarTrade } = useTrades()

const form = ref({
  diaDayTrade: null as Date | null,
  ativoId: null as number | null
})

const ativos = ref<Ativo[]>([])
const tradesSalvos = ref<Trade[]>([])
const novoTrade = ref({
  gatilho1: null as number | null,
  gatilho2: null as number | null,
  regiao: null as number | null,
  operacao: 'Compra' as string,
  estrategia: '' as string
})
const dayTradeId = ref<number | null>(null)
const dayTradeSalvo = ref(false)
const submitted = ref(false)
const loading = ref(false)
const loadingAtivos = ref(false)
const loadingSalvarTrade = ref(false)

const formatarData = (date: Date) => {
  if (!date) return ''
  const dia = String(date.getDate()).padStart(2, '0')
  const mes = String(date.getMonth() + 1).padStart(2, '0')
  const ano = date.getFullYear()
  return `${dia}/${mes}/${ano}`
}

const parseData = (dateString: string) => {
  if (!dateString) return null
  
  // Aceita formato dd/mm/yyyy
  const parts = dateString.split('/')
  if (parts.length === 3) {
    const dia = parseInt(parts[0], 10)
    const mes = parseInt(parts[1], 10) - 1
    const ano = parseInt(parts[2], 10)
    return new Date(ano, mes, dia)
  }
  
  return null
}

const carregarAtivos = async () => {
  loadingAtivos.value = true
  try {
    const response = await listarAtivos(1, 1000)
    ativos.value = response.items
  } catch (error) {
    Toast.open({
      message: 'Erro ao carregar ativos',
      type: 'is-danger',
      duration: 3000
    })
  } finally {
    loadingAtivos.value = false
  }
}

const salvarDayTrade = async () => {
  submitted.value = true

  if (!form.value.diaDayTrade || !form.value.ativoId) {
    Toast.open({
      message: 'Preencha todos os campos obrigatórios',
      type: 'is-warning',
      duration: 3000
    })
    return
  }

  loading.value = true

  try {
    const resultado = await criarDayTrade(form.value.diaDayTrade, form.value.ativoId)
    
    dayTradeId.value = resultado.id
    dayTradeSalvo.value = true
    
    Toast.open({
      message: 'DayTrade criado com sucesso! Agora adicione os trades.',
      type: 'is-success',
      duration: 3000
    })
  } catch (error: any) {
    Toast.open({
      message: error.message || 'Erro ao criar daytrade',
      type: 'is-danger',
      duration: 5000
    })
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

  if (!dayTradeId.value) {
    Toast.open({
      message: 'Erro: ID do daytrade não encontrado',
      type: 'is-danger',
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
  // Resetar formulário
  form.value = {
    diaDayTrade: null,
    ativoId: null
  }
  
  // Resetar estados
  dayTradeSalvo.value = false
  dayTradeId.value = null
  submitted.value = false
  tradesSalvos.value = []
  novoTrade.value = { 
    gatilho1: null, 
    gatilho2: null, 
    regiao: null,
    operacao: 'Compra', 
    estrategia: '' 
  }
  
  Toast.open({
    message: 'Pronto para criar um novo DayTrade!',
    type: 'is-info',
    duration: 2000
  })
  
  // Scroll para o topo
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

onMounted(() => {
  carregarAtivos()
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

