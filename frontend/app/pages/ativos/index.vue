<template>
  <div>
    <h1 class="title is-2 mb-5">Listagem de Ativos</h1>

    <div class="box">
      <b-table
        :data="ativos"
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
        <a @click="editarAtivo(props.row.id)" style="cursor: pointer; color: #3b82f6;">
          {{ props.row.id }}
        </a>
      </b-table-column>

      <b-table-column field="nome" label="Nome" v-slot="props">
        <a @click="editarAtivo(props.row.id)" style="cursor: pointer; color: #3b82f6;">
          {{ props.row.nome }}
        </a>
      </b-table-column>

      <b-table-column field="mercado" label="Mercado" v-slot="props">
        {{ props.row.mercado }}
      </b-table-column>

      <b-table-column field="codigo" label="Código" v-slot="props">
        {{ props.row.codigo }}
      </b-table-column>

      <b-table-column field="timeframe" label="Timeframe" v-slot="props">
        {{ props.row.timeframe }}
      </b-table-column>

      <b-table-column field="dataCriacao" label="Data de Criação" v-slot="props">
        {{ new Date(props.row.dataCriacao).toLocaleDateString() }}
      </b-table-column>

      <b-table-column label="Ações" width="150" v-slot="props">
        <b-button
          type="is-info"
          size="is-small"
          icon-left="eye"
          @click="visualizarCandles(props.row.id)"
          title="Visualizar candles"
          class="mr-2"
        />
        <b-button
          type="is-danger"
          size="is-small"
          icon-left="delete"
          @click="confirmarDeletar(props.row.id, props.row.nome)"
          title="Deletar ativo"
        />
      </b-table-column>

      <template #empty>
        <div class="has-text-centered">Nenhum ativo encontrado.</div>
      </template>
      </b-table>
    </div>

    <!-- Botão Flutuante -->
    <button class="fab-button" @click="criarNovoAtivo" title="Criar novo ativo">
      <b-icon icon="plus" size="is-medium"></b-icon>
    </button>
  </div>
</template>

<script setup lang="ts">
import type { Ativo, PaginacaoResponse } from '~/composables/useAtivos'

definePageMeta({
  layout: 'default',
  ssr: false
})

const router = useRouter()
const { listarAtivos, deletarAtivo } = useAtivos()
const Toast = useBuefyToast()
const Dialog = useBuefyDialog()

const ativos = ref<Ativo[]>([])
const loading = ref(false)
const totalItems = ref(0)
const currentPage = ref(1)
const pageSize = ref(10)

const fetchAtivos = async () => {
  loading.value = true
  try {
    const response: PaginacaoResponse = await listarAtivos(currentPage.value, pageSize.value)
    ativos.value = response.items
    totalItems.value = response.totalItems
    currentPage.value = response.page
    pageSize.value = response.pageSize
  } catch (error) {
    console.error('Erro ao buscar ativos:', error)
    Toast.open({
      message: 'Erro ao carregar ativos',
      type: 'is-danger',
      duration: 3000
    })
  } finally {
    loading.value = false
  }
}

const criarNovoAtivo = () => {
  router.push('/ativos/criar')
}

const editarAtivo = (id: number) => {
  router.push(`/ativos/editar/${id}`)
}

const visualizarCandles = (id: number) => {
  router.push(`/ativos/${id}`)
}

const confirmarDeletar = (id: number, nome: string) => {
  Dialog.confirm({
    title: 'Confirmar exclusão',
    message: `Tem certeza que deseja deletar o ativo <strong>${nome}</strong>?<br>Todos os candles relacionados também serão deletados.`,
    confirmText: 'Deletar',
    cancelText: 'Cancelar',
    type: 'is-danger',
    hasIcon: true,
    onConfirm: () => deletar(id)
  })
}

const deletar = async (id: number) => {
  try {
    await deletarAtivo(id)
    
    Toast.open({
      message: 'Ativo deletado com sucesso!',
      type: 'is-success',
      duration: 3000
    })

    fetchAtivos()
  } catch (error: any) {
    Toast.open({
      message: error.message || 'Erro ao deletar ativo',
      type: 'is-danger',
      duration: 3000
    })
  }
}

const onPageChange = (page: number) => {
  currentPage.value = page
  fetchAtivos()
}

onMounted(() => {
  fetchAtivos()
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
