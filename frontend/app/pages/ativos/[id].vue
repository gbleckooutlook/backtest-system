<template>
  <div class="container-page">
    <div class="level mb-5">
      <div class="level-left">
        <div class="level-item">
          <div>
            <h1 class="title is-2 mb-2">Candles do Ativo</h1>
            <p class="subtitle is-5 mt-0" v-if="ativo">
              {{ ativo.nome }} - {{ ativo.codigo }} ({{ ativo.timeframe }})
            </p>
          </div>
        </div>
      </div>
      <div class="level-right">
        <div class="level-item">
          <b-button
            type="is-light"
            icon-left="arrow-left"
            @click="voltar"
          >
            Voltar
          </b-button>
        </div>
      </div>
    </div>

    <div class="box">
      <b-table
        :data="candles"
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
        <b-table-column field="contadorCandles" label="Nº Candle" width="100" numeric v-slot="props">
          <strong class="has-text-info">{{ props.row.contadorCandles }}</strong>
        </b-table-column>

        <b-table-column field="data" label="Data/Hora" v-slot="props">
          {{ formatarDataHora(props.row.data) }}
        </b-table-column>

        <b-table-column field="abertura" label="Abertura" numeric v-slot="props">
          <span class="has-text-white">{{ formatarPreco(props.row.abertura) }}</span>
        </b-table-column>

        <b-table-column field="maxima" label="Máxima" numeric v-slot="props">
          <span class="has-text-success">{{ formatarPreco(props.row.maxima) }}</span>
        </b-table-column>

        <b-table-column field="minima" label="Mínima" numeric v-slot="props">
          <span class="has-text-danger">{{ formatarPreco(props.row.minima) }}</span>
        </b-table-column>

        <b-table-column field="fechamento" label="Fechamento" numeric v-slot="props">
          <span class="has-text-white">{{ formatarPreco(props.row.fechamento) }}</span>
        </b-table-column>

        <b-table-column label="Variação" numeric v-slot="props">
          <b-tag 
            :type="calcularVariacao(props.row.abertura, props.row.fechamento) >= 0 ? 'is-success' : 'is-danger'"
          >
            {{ calcularVariacao(props.row.abertura, props.row.fechamento) >= 0 ? '+' : '' }}{{ calcularVariacao(props.row.abertura, props.row.fechamento).toFixed(2) }}%
          </b-tag>
        </b-table-column>

        <template #empty>
          <div class="has-text-centered">Nenhum candle encontrado para este ativo.</div>
        </template>
      </b-table>
    </div>

    <div class="box mt-4" v-if="totalItems > 0">
      <h3 class="subtitle is-5 mb-3">📊 Informações Gerais</h3>
      <div class="columns">
        <div class="column">
          <p><strong>Total de Candles:</strong> {{ totalItems.toLocaleString('pt-BR') }}</p>
        </div>
        <div class="column">
          <p><strong>Total de Páginas:</strong> {{ totalPages }}</p>
        </div>
        <div class="column">
          <p><strong>Página Atual:</strong> {{ currentPage }} de {{ totalPages }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
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

const route = useRoute()
const router = useRouter()
const { obterAtivo, listarCandles } = useAtivos()
const Toast = useBuefyToast()

const ativoId = computed(() => parseInt(route.params.id as string))

const ativo = ref<any>(null)
const candles = ref<any[]>([])
const loading = ref(false)
const totalItems = ref(0)
const currentPage = ref(1)
const pageSize = ref(50)
const totalPages = ref(0)

const fetchAtivo = async () => {
  try {
    ativo.value = await obterAtivo(ativoId.value)
  } catch (error) {
    Toast.open({
      message: 'Erro ao carregar ativo',
      type: 'is-danger',
      duration: 3000
    })
    router.push('/ativos')
  }
}

const fetchCandles = async () => {
  loading.value = true
  try {
    const response = await listarCandles(ativoId.value, currentPage.value, pageSize.value)
    candles.value = response.items
    totalItems.value = response.totalItems
    currentPage.value = response.page
    pageSize.value = response.pageSize
    totalPages.value = response.totalPages
  } catch (error) {
    console.error('Erro ao buscar candles:', error)
    Toast.open({
      message: 'Erro ao carregar candles',
      type: 'is-danger',
      duration: 3000
    })
  } finally {
    loading.value = false
  }
}

const onPageChange = (page: number) => {
  currentPage.value = page
  fetchCandles()
}

const voltar = () => {
  router.push('/ativos')
}

const formatarDataHora = (data: string) => {
  // Converte UTC para horário de São Paulo (Brasil)
  return dayjs.utc(data).tz('America/Sao_Paulo').format('DD/MM/YYYY, HH:mm:ss')
}

const formatarPreco = (preco: number) => {
  return preco.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

const calcularVariacao = (abertura: number, fechamento: number) => {
  if (!abertura || abertura === 0) return 0
  return ((fechamento - abertura) / abertura) * 100
}

onMounted(() => {
  fetchAtivo()
  fetchCandles()
})
</script>

<style scoped>
.container-page {
  padding-top: 1rem;
}

.has-text-white {
  color: #f5f5f5;
}
</style>

