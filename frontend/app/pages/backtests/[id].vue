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
            <b-field label="Folga PTS">
              <b-input :value="backtest.folga" disabled />
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
      <div v-if="backtest.status === 'Finalizado' && backtest.resultado" class="box mt-5">
        <h2 class="title is-4 mb-4">Resultados</h2>
        <div class="content">
          <pre>{{ backtest.resultado }}</pre>
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

