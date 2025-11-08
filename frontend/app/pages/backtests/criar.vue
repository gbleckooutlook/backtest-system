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
          <h1 class="title">Criar Backtest</h1>
        </div>
      </div>
    </div>

    <div class="box">
      <form @submit.prevent="salvarBacktest">
        <!-- Período -->
        <div class="columns">
          <div class="column is-half">
            <b-field
              label="Data Início"
              :type="submitted && !form.dataInicio ? 'is-danger' : ''"
              :message="submitted && !form.dataInicio ? 'Data de início é obrigatória' : ''"
            >
              <b-datepicker
                v-model="form.dataInicio"
                placeholder="Selecione a data de início"
                icon="calendar"
                :date-formatter="formatarData"
                :date-parser="parseData"
                editable
                trap-focus
              >
              </b-datepicker>
            </b-field>
          </div>

          <div class="column is-half">
            <b-field
              label="Data Fim"
              :type="submitted && !form.dataFim ? 'is-danger' : ''"
              :message="submitted && !form.dataFim ? 'Data de fim é obrigatória' : ''"
            >
              <b-datepicker
                v-model="form.dataFim"
                placeholder="Selecione a data de fim"
                icon="calendar"
                :date-formatter="formatarData"
                :date-parser="parseData"
                editable
                trap-focus
              >
              </b-datepicker>
            </b-field>
          </div>
        </div>

        <!-- Ativo -->
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

        <!-- Entrada, Alvo e Nº de Contratos -->
        <div class="columns">
          <div class="column">
            <b-field
              label="Entrada (PTS)"
              :type="submitted && !form.entrada ? 'is-danger' : ''"
              :message="submitted && !form.entrada ? 'Entrada é obrigatória' : ''"
            >
              <b-input
                v-model.number="form.entrada"
                type="number"
                placeholder="Ex: 80, 90, 100"
                min="0"
              />
            </b-field>
          </div>

          <div class="column">
            <b-field
              label="Alvo (PTS)"
              :type="submitted && !form.alvo ? 'is-danger' : ''"
              :message="submitted && !form.alvo ? 'Alvo é obrigatório' : ''"
            >
              <b-input
                v-model.number="form.alvo"
                type="number"
                placeholder="Ex: 632"
                min="0"
              />
            </b-field>
          </div>

          <div class="column">
            <b-field
              label="Nº de Contratos"
              :type="submitted && !form.numeroContratos ? 'is-danger' : ''"
              :message="submitted && !form.numeroContratos ? 'Número de contratos é obrigatório' : ''"
            >
              <b-input
                v-model.number="form.numeroContratos"
                type="number"
                placeholder="Ex: 1, 2, 3"
                min="1"
              />
            </b-field>
          </div>
        </div>

        <!-- Stop -->
        <b-field
          label="Stop (PTS)"
          :type="submitted && !form.stop ? 'is-danger' : ''"
          :message="submitted && !form.stop ? 'Stop é obrigatório' : ''"
        >
          <b-input
            v-model.number="form.stop"
            type="number"
            placeholder="Ex: 100"
            min="0"
          />
        </b-field>

        <!-- Estratégias -->
        <b-field label="Estratégias" class="mt-4">
          <div>
            <b-checkbox v-model="selecionarTodos" @input="toggleTodasEstrategias">
              Selecionar Todas
            </b-checkbox>
            <hr class="my-2" />
            <div class="columns is-multiline">
              <div class="column is-half" v-for="estrategia in estrategiasDisponiveis" :key="estrategia">
                <b-checkbox v-model="form.estrategias" :native-value="estrategia">
                  {{ estrategia }}
                </b-checkbox>
              </div>
            </div>
          </div>
        </b-field>

        <!-- Proteger (1:1) -->
        <b-field class="mt-4">
          <b-checkbox v-model="form.proteger">
            Proteger (proteger no 1:1)
          </b-checkbox>
        </b-field>

        <!-- Botões -->
        <b-field grouped class="mt-5">
          <b-button
            native-type="submit"
            type="is-primary"
            icon-left="calculator-variant"
            :loading="loading"
            class="mr-3"
          >
            Processar Backtest
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

    <!-- Botão Flutuante -->
    <button class="fab-button" @click="criarNovoBacktest" title="Criar novo backtest">
      <b-icon icon="plus" size="is-medium"></b-icon>
    </button>
  </div>
</template>

<script setup lang="ts">
import type { Ativo } from '~/composables/useAtivos'

definePageMeta({
  layout: 'default',
  ssr: false
})

const router = useRouter()
const Toast = useBuefyToast()
const { listarAtivos } = useAtivos()
const { criarBacktest } = useBacktests()

const estrategiasDisponiveis = [
  'Pullback de lado',
  'Pullback na inversão',
  'Pullback no 50% macro',
  'Pullback no 50% micro',
  'Consolidação',
  'Quebra micro'
]

const form = ref({
  dataInicio: null as Date | null,
  dataFim: null as Date | null,
  entrada: null as number | null,
  alvo: null as number | null,
  numeroContratos: null as number | null,
  ativoId: null as number | null,
  stop: null as number | null,
  estrategias: [] as string[],
  proteger: false
})

const selecionarTodos = ref(false)

const toggleTodasEstrategias = (selecionado: boolean) => {
  if (selecionado) {
    form.value.estrategias = [...estrategiasDisponiveis]
  } else {
    form.value.estrategias = []
  }
}

watch(() => form.value.estrategias.length, (novoTamanho) => {
  selecionarTodos.value = novoTamanho === estrategiasDisponiveis.length
})

const ativos = ref<Ativo[]>([])
const submitted = ref(false)
const loading = ref(false)
const loadingAtivos = ref(false)

const formatarData = (date: Date) => {
  if (!date) return ''
  const dia = String(date.getDate()).padStart(2, '0')
  const mes = String(date.getMonth() + 1).padStart(2, '0')
  const ano = date.getFullYear()
  return `${dia}/${mes}/${ano}`
}

const parseData = (dateString: string) => {
  if (!dateString) return null
  
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

const salvarBacktest = async () => {
  submitted.value = true

  if (!form.value.dataInicio || !form.value.dataFim || !form.value.entrada || 
      !form.value.alvo || !form.value.numeroContratos || !form.value.ativoId || 
      !form.value.stop) {
    Toast.open({
      message: 'Preencha todos os campos obrigatórios',
      type: 'is-warning',
      duration: 3000
    })
    return
  }

  if (form.value.estrategias.length === 0) {
    Toast.open({
      message: 'Selecione pelo menos uma estratégia',
      type: 'is-warning',
      duration: 3000
    })
    return
  }

  if (form.value.dataInicio > form.value.dataFim) {
    Toast.open({
      message: 'A data de início deve ser anterior à data de fim',
      type: 'is-warning',
      duration: 3000
    })
    return
  }

  loading.value = true

  try {
    const backtestCriado = await criarBacktest(
      form.value.dataInicio,
      form.value.dataFim,
      form.value.entrada,
      form.value.alvo,
      form.value.numeroContratos,
      form.value.ativoId,
      form.value.stop,
      form.value.estrategias,
      form.value.proteger
    )
    
    Toast.open({
      message: 'Backtest criado com sucesso! Redirecionando para visualização...',
      type: 'is-success',
      duration: 2000
    })

    setTimeout(() => {
      router.push(`/backtests/${backtestCriado.id}`)
    }, 1000)
  } catch (error: any) {
    Toast.open({
      message: error.message || 'Erro ao criar backtest',
      type: 'is-danger',
      duration: 5000
    })
  } finally {
    loading.value = false
  }
}

const voltar = () => {
  router.push('/backtests')
}

const criarNovoBacktest = () => {
  form.value = {
    dataInicio: null,
    dataFim: null,
    entrada: null,
    alvo: null,
    numeroContratos: null,
    ativoId: null,
    stop: null,
    estrategias: [],
    proteger: false
  }
  selecionarTodos.value = false
  submitted.value = false
  
  Toast.open({
    message: 'Formulário resetado!',
    type: 'is-info',
    duration: 2000
  })
  
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

/* Remover hover problemático do checkbox */
:deep(.b-checkbox .control-label):hover {
  background-color: transparent !important;
  color: inherit !important;
  opacity: 1 !important;
}

:deep(.b-checkbox):hover {
  background-color: transparent !important;
}

:deep(.b-checkbox .check) {
  background-color: transparent !important;
  border-color: #3273dc !important;
}

:deep(.b-checkbox .check):hover {
  background-color: transparent !important;
  border-color: #3273dc !important;
}

/* Estilos para linha divisória */
hr {
  background-color: #dbdbdb;
  border: none;
  height: 1px;
  margin: 0.5rem 0;
}
</style>
