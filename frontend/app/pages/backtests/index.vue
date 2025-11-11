<template>
  <div>
    <h1 class="title is-2 mb-5">Listagem de Backtests</h1>

    <div class="box">
      <b-table
        :data="backtests"
        :loading="loading"
        paginated
        backend-pagination
        :total="totalItems"
        :per-page="pageSize"
        :current-page="currentPage"
        @page-change="onPageChange"
        hoverable
        striped
      >
        <b-table-column field="id" label="ID" width="40" numeric v-slot="props">
          {{ props.row.id }}
        </b-table-column>

        <b-table-column field="dataInicio" label="Período" v-slot="props">
          {{ formatarData(props.row.dataInicio) }} - {{ formatarData(props.row.dataFim) }}
        </b-table-column>

        <b-table-column field="ativoNome" label="Ativo" v-slot="props">
          {{ props.row.ativoNome }} ({{ props.row.ativoCodigo }})
        </b-table-column>

        <b-table-column field="entrada" label="Entrada" v-slot="props">
          {{ props.row.entrada }} PTS
        </b-table-column>

        <b-table-column field="stop" label="Stop" v-slot="props">
          {{ props.row.stop }} PTS
        </b-table-column>

        <b-table-column field="alvo" label="Alvo" v-slot="props">
          {{ props.row.alvo }} PTS
        </b-table-column>

        <b-table-column field="numeroContratos" label="Contratos" v-slot="props" numeric>
          {{ props.row.numeroContratos }}
        </b-table-column>

        <b-table-column field="proteger" label="Proteger 1:1" v-slot="props" centered>
          <b-icon 
            :icon="props.row.proteger ? 'shield-check' : 'shield-off'" 
            :type="props.row.proteger ? 'is-success' : 'is-grey-light'"
            size="is-small"
          />
        </b-table-column>

        <b-table-column field="status" label="Status" v-slot="props">
          <span 
            class="tag" 
            :class="{
              'is-warning': props.row.status === 'Iniciado',
              'is-success': props.row.status === 'Finalizado',
              'is-danger': props.row.status === 'Erro'
            }"
          >
            {{ props.row.status }}
          </span>
        </b-table-column>

        <b-table-column field="dataCriacao" label="Data de Criação" v-slot="props">
          {{ formatarDataHora(props.row.dataCriacao) }}
        </b-table-column>

        <b-table-column label="Ações" width="120" v-slot="props">
          <b-button
            type="is-info"
            size="is-small"
            icon-left="eye"
            @click="visualizarBacktest(props.row.id)"
            class="mr-2"
            title="Visualizar"
          />
          <b-button
            type="is-danger"
            size="is-small"
            icon-left="delete"
            @click="confirmarDeletar(props.row.id)"
            title="Deletar"
          />
        </b-table-column>

        <template #empty>
          <div class="has-text-centered">Nenhum backtest encontrado.</div>
        </template>
      </b-table>
    </div>

    <!-- Botão Flutuante -->
    <button class="fab-button" @click="criarNovoBacktest" title="Criar novo backtest">
      <b-icon icon="plus" size="is-medium"></b-icon>
    </button>
  </div>
</template>

<script setup lang="ts">
import type { Backtest, PaginacaoResponse } from '~/composables/useBacktests'
import dayjs from 'dayjs'
import utc from 'dayjs/plugin/utc'
import timezone from 'dayjs/plugin/timezone'
import 'dayjs/locale/pt-br'

// Configurar dayjs
dayjs.extend(utc)
dayjs.extend(timezone)
dayjs.locale('pt-br')

definePageMeta({
  layout: 'default',
  ssr: false
})

const router = useRouter()
const { listarBacktests, deletarBacktest } = useBacktests()
const Toast = useBuefyToast()
const Dialog = useBuefyDialog()

const backtests = ref<Backtest[]>([])
const loading = ref(false)
const totalItems = ref(0)
const currentPage = ref(1)
const pageSize = ref(10)
let pollingInterval: NodeJS.Timeout | null = null

const formatarData = (dataString: string) => {
  const data = new Date(dataString)
  return data.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

const fetchBacktests = async () => {
  loading.value = true
  try {
    const response: PaginacaoResponse = await listarBacktests(currentPage.value, pageSize.value)
    backtests.value = response.items
    totalItems.value = response.totalItems
    currentPage.value = response.page
    pageSize.value = response.pageSize
  } catch (error) {
    console.error('Erro ao buscar backtests:', error)
    Toast.open({
      message: 'Erro ao carregar backtests',
      type: 'is-danger',
      duration: 3000
    })
  } finally {
    loading.value = false
  }
}

const criarNovoBacktest = () => {
  router.push('/backtests/criar')
}

const visualizarBacktest = (id: number) => {
  router.push(`/backtests/${id}`)
}

const confirmarDeletar = (id: number) => {
  Dialog.confirm({
    title: 'Confirmar exclusão',
    message: 'Tem certeza que deseja deletar este backtest?',
    confirmText: 'Deletar',
    cancelText: 'Cancelar',
    type: 'is-danger',
    hasIcon: true,
    onConfirm: () => deletar(id)
  })
}

const deletar = async (id: number) => {
  try {
    await deletarBacktest(id)
    
    Toast.open({
      message: 'Backtest deletado com sucesso!',
      type: 'is-success',
      duration: 3000
    })

    fetchBacktests()
  } catch (error: any) {
    Toast.open({
      message: error.message || 'Erro ao deletar backtest',
      type: 'is-danger',
      duration: 3000
    })
  }
}

const formatarDataHora = (data: string) => {
  // Converte UTC para horário de São Paulo (Brasil)
  return dayjs.utc(data).tz('America/Sao_Paulo').format('DD/MM/YYYY, HH:mm')
}

const onPageChange = (page: number) => {
  currentPage.value = page
  fetchBacktests()
}

const temBacktestIniciado = computed(() => {
  return backtests.value.some(b => b.status === 'Iniciado')
})

const iniciarPolling = () => {
  // Só inicia polling se tiver algum backtest com status "Iniciado"
  if (temBacktestIniciado.value) {
    pollingInterval = setInterval(() => {
      if (temBacktestIniciado.value) {
        fetchBacktests()
      } else {
        pararPolling()
      }
    }, 5000) // Atualiza a cada 5 segundos
  }
}

const pararPolling = () => {
  if (pollingInterval) {
    clearInterval(pollingInterval)
    pollingInterval = null
  }
}

onMounted(() => {
  fetchBacktests()
})

onUnmounted(() => {
  pararPolling()
})

watch(temBacktestIniciado, (novo) => {
  if (novo && !pollingInterval) {
    iniciarPolling()
  } else if (!novo && pollingInterval) {
    pararPolling()
  }
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
</style>
