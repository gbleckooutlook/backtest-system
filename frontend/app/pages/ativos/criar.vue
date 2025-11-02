<template>
  <div>
    <div style="display: flex; align-items: center; margin-bottom: 2rem;">
      <Button 
        icon="pi pi-arrow-left" 
        text 
        @click="voltar" 
        style="margin-right: 1rem;"
      />
      <h1 style="font-size: 2rem; font-weight: bold; color: #e6e8eb; margin: 0;">Criar Ativo</h1>
    </div>

    <Card>
      <template #content>
        <form @submit.prevent="salvarAtivo">
          <div style="margin-bottom: 1.5rem;">
            <label for="nome" style="display: block; font-weight: 600; margin-bottom: 0.5rem; color: #e6e8eb;">Nome do Ativo *</label>
            <InputText 
              id="nome" 
              v-model="form.nome" 
              style="width: 100%;"
              placeholder="Ex: Mini-Dólar"
              :class="{'p-invalid': submitted && !form.nome}"
            />
            <small v-if="submitted && !form.nome" style="color: #ef4444; font-size: 0.875rem;">Nome é obrigatório</small>
          </div>

          <div style="margin-bottom: 1.5rem;">
            <label for="mercado" style="display: block; font-weight: 600; margin-bottom: 0.5rem; color: #e6e8eb;">Mercado *</label>
            <Select 
              id="mercado" 
              v-model="form.mercado" 
              :options="mercados" 
              placeholder="Selecione o mercado"
              style="width: 100%;"
              :class="{'p-invalid': submitted && !form.mercado}"
            />
            <small v-if="submitted && !form.mercado" style="color: #ef4444; font-size: 0.875rem;">Mercado é obrigatório</small>
          </div>

          <div style="margin-bottom: 1.5rem;">
            <label for="codigo" style="display: block; font-weight: 600; margin-bottom: 0.5rem; color: #e6e8eb;">Código *</label>
            <InputText 
              id="codigo" 
              v-model="form.codigo" 
              style="width: 100%;"
              placeholder="Ex: WDO"
              :class="{'p-invalid': submitted && !form.codigo}"
            />
            <small v-if="submitted && !form.codigo" style="color: #ef4444; font-size: 0.875rem;">Código é obrigatório</small>
          </div>

          <div style="margin-bottom: 1.5rem;">
            <label for="timeframe" style="display: block; font-weight: 600; margin-bottom: 0.5rem; color: #e6e8eb;">Timeframe *</label>
            <Select 
              id="timeframe" 
              v-model="form.timeframe" 
              :options="timeframes" 
              placeholder="Selecione o timeframe"
              style="width: 100%;"
              :class="{'p-invalid': submitted && !form.timeframe}"
            />
            <small v-if="submitted && !form.timeframe" style="color: #ef4444; font-size: 0.875rem;">Timeframe é obrigatório</small>
          </div>

          <div style="margin-bottom: 1.5rem;">
            <label for="csv" style="display: block; font-weight: 600; margin-bottom: 0.5rem; color: #e6e8eb;">Arquivo CSV</label>
            <input
              type="file"
              id="csv"
              accept=".csv"
              @change="onFileChange"
              style="display: block; width: 100%; padding: 0.5rem; border: 1px solid #ced4da; border-radius: 6px;"
            />
            <small style="color: #94a3b8; font-size: 0.875rem;">
              Formato esperado: Data, Abertura, Máxima, Mínima, Fechamento, Contador de Candles
            </small>
          </div>

          <div style="display: flex; gap: 0.5rem; margin-top: 2rem;">
            <Button 
              type="submit" 
              label="Salvar" 
              icon="pi pi-check" 
              :loading="loading"
            />
            <Button 
              type="button" 
              label="Cancelar" 
              icon="pi pi-times" 
              severity="secondary"
              @click="voltar"
              :disabled="loading"
            />
          </div>
        </form>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { useToast } from 'primevue/usetoast'

definePageMeta({
  layout: 'default'
})

const toast = useToast()
const router = useRouter()
const { criarAtivo } = useAtivos()

const form = ref({
  nome: '',
  mercado: '',
  codigo: '',
  timeframe: '',
  arquivo: null as File | null
})

const mercados = ref(['B3', 'Forex'])
const timeframes = ref([
  '1 minuto',
  '2 minutos',
  '3 minutos',
  '5 minutos',
  '10 minutos',
  '15 minutos',
  '30 minutos',
  '60 minutos',
  '240 minutos'
])
const submitted = ref(false)
const loading = ref(false)

const onFileChange = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files && target.files[0]) {
    form.value.arquivo = target.files[0]
  }
}

const salvarAtivo = async () => {
  submitted.value = true

  // Validar campos obrigatórios
  if (!form.value.nome || !form.value.mercado || !form.value.codigo || !form.value.timeframe) {
    toast.add({
      severity: 'warn',
      summary: 'Atenção',
      detail: 'Preencha todos os campos obrigatórios',
      life: 3000
    })
    return
  }

  loading.value = true

  try {
    const formData = new FormData()
    formData.append('nome', form.value.nome)
    formData.append('mercado', form.value.mercado)
    formData.append('codigo', form.value.codigo)
    formData.append('timeframe', form.value.timeframe)
    
    if (form.value.arquivo) {
      formData.append('arquivoCsv', form.value.arquivo)
    }

    await criarAtivo(formData)

    toast.add({
      severity: 'success',
      summary: 'Sucesso',
      detail: 'Ativo criado com sucesso',
      life: 3000
    })

    setTimeout(() => {
      router.push('/ativos')
    }, 1000)
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: 'Erro',
      detail: error.message || 'Erro ao criar ativo',
      life: 5000
    })
  } finally {
    loading.value = false
  }
}

const voltar = () => {
  router.push('/ativos')
}
</script>


