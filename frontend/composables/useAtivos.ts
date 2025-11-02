export interface Ativo {
  id: number
  nome: string
  mercado: string
  codigo: string
  timeframe: string
  dataCriacao: string
}

export interface PaginacaoResponse {
  items: Ativo[]
  totalItems: number
  page: number
  pageSize: number
  totalPages: number
}

export const useAtivos = () => {
  const { api, baseURL } = useApi()

  const listarAtivos = async (page: number = 1, pageSize: number = 10): Promise<PaginacaoResponse> => {
    try {
      const response = await api<PaginacaoResponse>(`/api/ativos?page=${page}&pageSize=${pageSize}`)
      return response
    } catch (error) {
      console.error('Erro ao listar ativos:', error)
      throw error
    }
  }

  const criarAtivo = async (formData: FormData) => {
    try {
      // Usar fetch nativo para upload de arquivo
      const response = await fetch(`${baseURL}/api/ativos`, {
        method: 'POST',
        body: formData,
        credentials: 'include'
      })

      if (!response.ok) {
        const error = await response.json()
        throw new Error(error.erro || 'Erro ao criar ativo')
      }

      return await response.json()
    } catch (error) {
      console.error('Erro ao criar ativo:', error)
      throw error
    }
  }

  return {
    listarAtivos,
    criarAtivo
  }
}


