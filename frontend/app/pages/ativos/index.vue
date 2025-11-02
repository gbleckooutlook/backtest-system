<template>
  <div>
    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem;">
      <h1 style="font-size: 2rem; font-weight: bold; color: #2c3e50; margin: 0;">Ativos</h1>
      <Button label="Novo Ativo" icon="pi pi-plus" @click="navegarParaCriar" /></div>

    <Card>
      <template #content>
        <DataTable 
          :value="ativos" 
          :loading="loading"
          stripedRows
          class="p-datatable-sm"
        >
          <Column field="nome" header="Nome" sortable></Column>
          <Column field="mercado" header="Mercado" sortable></Column>
          <Column field="codigo" header="Código" sortable></Column>
          <Column field="timeframe" header="Timeframe" sortable></Column>
          <Column field="dataCriacao" header="Data de Criação" sortable>
            <template #body="slotProps">
              {{ formatarData(slotProps.data.dataCriacao) }}
            </template>
          </Column>
        </DataTable>

        <Paginator
          v-if="totalItems > 0"
          :rows="pageSize"
          :totalRecords="totalItems"
          :rowsPerPageOptions="[10, 20, 50]"
          @page="onPageChange"
          style="margin-top: 1rem;"
        ></Paginator>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { useToast } from 'primevue/usetoast'
import type { Ativo } from '~/composables/useAtivos'

definePageMeta({
  layout: 'default'
})

const toast = useToast()
const { listarAtivos } = useAtivos()
const router = useRouter()

const ativos = ref<Ativo[]>([])
const loading = ref(false)
const page = ref(1)
const pageSize = ref(10)
const totalItems = ref(0)

const carregarAtivos = async () => {
  loading.value = true
  try {
    const response = await listarAtivos(page.value, pageSize.value)
    ativos.value = response.items
    totalItems.value = response.totalItems
    pageSize.value = response.pageSize
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: 'Erro',
      detail: 'Erro ao carregar ativos',
      life: 3000
    })
  } finally {
    loading.value = false
  }
}

const onPageChange = (event: any) => {
  page.value = event.page + 1
  pageSize.value = event.rows
  carregarAtivos()
}

const formatarData = (data: string) => {
  return new Date(data).toLocaleString('pt-BR')
}

const navegarParaCriar = () => {
  router.push('/ativos/criar')
}

onMounted(() => {
  carregarAtivos()
})
</script>


