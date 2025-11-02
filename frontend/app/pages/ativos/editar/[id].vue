<template>
  <div>
    <div class="level mb-5">
      <div class="level-left">
        <div class="level-item">
          <b-button tag="router-link" to="/ativos" icon-left="arrow-left" type="is-text">
            Voltar
          </b-button>
        </div>
        <div class="level-item">
          <h1 class="title">Editar Ativo</h1>
        </div>
      </div>
    </div>

    <div class="box" v-if="!loadingAtivo">
      <form @submit.prevent="salvarAtivo">
        <b-field
          label="Nome do Ativo"
          :type="submitted && !form.nome ? 'is-danger' : ''"
          :message="submitted && !form.nome ? 'Nome é obrigatório' : ''"
        >
          <b-input
            v-model="form.nome"
            placeholder="Ex: Mini-Dólar"
          />
        </b-field>

        <b-field
          label="Mercado"
          :type="submitted && !form.mercado ? 'is-danger' : ''"
          :message="submitted && !form.mercado ? 'Mercado é obrigatório' : ''"
        >
          <b-select
            v-model="form.mercado"
            placeholder="Selecione o mercado"
            expanded
          >
            <option value="B3">B3</option>
            <option value="Forex">Forex</option>
          </b-select>
        </b-field>

        <b-field
          label="Código"
          :type="submitted && !form.codigo ? 'is-danger' : ''"
          :message="submitted && !form.codigo ? 'Código é obrigatório' : ''"
        >
          <b-input
            v-model="form.codigo"
            placeholder="Ex: WDO"
          />
        </b-field>

        <b-field
          label="Timeframe"
          :type="submitted && !form.timeframe ? 'is-danger' : ''"
          :message="submitted && !form.timeframe ? 'Timeframe é obrigatório' : ''"
        >
          <b-select
            v-model="form.timeframe"
            placeholder="Selecione o timeframe"
            expanded
          >
            <option>1 minuto</option>
            <option>2 minutos</option>
            <option>3 minutos</option>
            <option>5 minutos</option>
            <option>10 minutos</option>
            <option>15 minutos</option>
            <option>30 minutos</option>
            <option>60 minutos</option>
            <option>240 minutos</option>
          </b-select>
        </b-field>

        <b-field label="Arquivo CSV (opcional - deixe em branco para manter o atual)">
          <b-field class="file mb-5">
            <b-upload
              v-model="form.arquivo"
              accept=".csv"
            >
              <span class="file-cta">
                <b-icon icon="upload"></b-icon>
                <span class="file-label">{{ form.arquivo ? form.arquivo.name : 'Escolher novo arquivo' }}</span>
              </span>
            </b-upload>
          </b-field>
          <p class="help">Se enviar um novo CSV, todos os candles antigos serão substituídos</p>
        </b-field>

        <b-field grouped class="mt-5">
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

    <div class="box" v-else>
      <b-loading :is-full-page="false" v-model="loadingAtivo" :can-cancel="false"></b-loading>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'default'
})

const route = useRoute()
const router = useRouter()
const { obterAtivo, editarAtivo } = useAtivos()
const Toast = useBuefyToast()

const ativoId = computed(() => parseInt(route.params.id as string))

const form = ref({
  nome: '',
  mercado: '',
  codigo: '',
  timeframe: '',
  arquivo: null as File | null
})

const submitted = ref(false)
const loading = ref(false)
const loadingAtivo = ref(true)

const carregarAtivo = async () => {
  try {
    const ativo = await obterAtivo(ativoId.value)
    form.value.nome = ativo.nome
    form.value.mercado = ativo.mercado
    form.value.codigo = ativo.codigo
    form.value.timeframe = ativo.timeframe
  } catch (error) {
    Toast.open({
      message: 'Erro ao carregar ativo',
      type: 'is-danger',
      duration: 3000
    })
    router.push('/ativos')
  } finally {
    loadingAtivo.value = false
  }
}

const salvarAtivo = async () => {
  submitted.value = true

  if (!form.value.nome || !form.value.mercado || !form.value.codigo || !form.value.timeframe) {
    Toast.open({
      message: 'Preencha todos os campos obrigatórios',
      type: 'is-warning',
      duration: 3000
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

    await editarAtivo(ativoId.value, formData)

    Toast.open({
      message: 'Ativo atualizado com sucesso!',
      type: 'is-success',
      duration: 3000
    })

    setTimeout(() => {
      router.push('/ativos')
    }, 1000)
  } catch (error: any) {
    Toast.open({
      message: error.message || 'Erro ao atualizar ativo',
      type: 'is-danger',
      duration: 5000
    })
  } finally {
    loading.value = false
  }
}

const voltar = () => {
  router.push('/ativos')
}

onMounted(() => {
  carregarAtivo()
})
</script>

