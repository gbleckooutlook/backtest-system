<template>
  <div>
    <h1 class="title is-2 mb-5">Listagem de DayTrades</h1>

    <div class="box">
      <b-table
        :data="daytrades"
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

        <b-table-column field="diaDayTrade" label="Data" v-slot="props">
          {{ formatarData(props.row.diaDayTrade) }}
        </b-table-column>

        <b-table-column field="ativoNome" label="Ativo" v-slot="props">
          {{ props.row.ativoNome }} ({{ props.row.ativoCodigo }})
        </b-table-column>

        <b-table-column field="dataCriacao" label="Data de Criação" v-slot="props">
          {{ new Date(props.row.dataCriacao).toLocaleDateString() }}
        </b-table-column>

        <b-table-column label="Ações" width="120" v-slot="props">
          <b-button
            type="is-info"
            size="is-small"
            icon-left="eye"
            @click="visualizarDayTrade(props.row.id)"
            class="mr-2"
            title="Visualizar/Editar"
          />
          <b-button
            type="is-danger"
            size="is-small"
            icon-left="delete"
            @click="confirmarDeletar(props.row.id, props.row.diaDayTrade)"
            title="Deletar"
          />
        </b-table-column>

      <template #empty>
        <div class="has-text-centered">Nenhum daytrade encontrado.</div>
      </template>
      </b-table>
    </div>

    <!-- Botão Flutuante -->
    <button class="fab-button" @click="criarNovoDayTrade" title="Criar novo daytrade">
      <b-icon icon="plus" size="is-medium"></b-icon>
    </button>
  </div>
</template>

<script setup lang="ts">
import type { DayTrade, PaginacaoResponse } from '~/composables/useDayTrades'

definePageMeta({
  layout: 'default',
  ssr: false
})

const router = useRouter()
const { listarDayTrades, deletarDayTrade } = useDayTrades()
const Toast = useBuefyToast()
const Dialog = useBuefyDialog()

const daytrades = ref<DayTrade[]>([])
const loading = ref(false)
const totalItems = ref(0)
const currentPage = ref(1)
const pageSize = ref(10)

const formatarData = (dataString: string) => {
  const data = new Date(dataString)
  return data.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

const fetchDayTrades = async () => {
  loading.value = true
  try {
    const response: PaginacaoResponse = await listarDayTrades(currentPage.value, pageSize.value)
    daytrades.value = response.items
    totalItems.value = response.totalItems
    currentPage.value = response.page
    pageSize.value = response.pageSize
  } catch (error) {
    console.error('Erro ao buscar daytrades:', error)
    Toast.open({
      message: 'Erro ao carregar daytrades',
      type: 'is-danger',
      duration: 3000
    })
  } finally {
    loading.value = false
  }
}

const criarNovoDayTrade = () => {
  router.push('/daytrades/criar')
}

const visualizarDayTrade = (id: number) => {
  router.push(`/daytrades/editar/${id}`)
}

const confirmarDeletar = (id: number, diaDayTrade: string) => {
  const dataFormatada = formatarData(diaDayTrade)
  Dialog.confirm({
    title: 'Confirmar exclusão',
    message: `Tem certeza que deseja deletar o daytrade do dia <strong>${dataFormatada}</strong>?<br>Todos os trades relacionados também serão deletados.`,
    confirmText: 'Deletar',
    cancelText: 'Cancelar',
    type: 'is-danger',
    hasIcon: true,
    onConfirm: () => deletar(id)
  })
}

const deletar = async (id: number) => {
  try {
    await deletarDayTrade(id)
    
    Toast.open({
      message: 'DayTrade deletado com sucesso!',
      type: 'is-success',
      duration: 3000
    })

    fetchDayTrades()
  } catch (error: any) {
    Toast.open({
      message: error.message || 'Erro ao deletar daytrade',
      type: 'is-danger',
      duration: 3000
    })
  }
}

const onPageChange = (page: number) => {
  currentPage.value = page
  fetchDayTrades()
}

onMounted(() => {
  fetchDayTrades()
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
