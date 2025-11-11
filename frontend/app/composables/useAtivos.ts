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

  const obterAtivo = async (id: number): Promise<Ativo> => {
    try {
      const response = await api<Ativo>(`/api/ativos/${id}`)
      return response
    } catch (error) {
      console.error('Erro ao obter ativo:', error)
      throw error
    }
  }

  const editarAtivo = async (id: number, formData: FormData) => {
    try {
      const response = await fetch(`${baseURL}/api/ativos/${id}`, {
        method: 'PUT',
        body: formData,
        credentials: 'include'
      })

      if (!response.ok) {
        const error = await response.json()
        throw new Error(error.erro || 'Erro ao editar ativo')
      }

      return await response.json()
    } catch (error) {
      console.error('Erro ao editar ativo:', error)
      throw error
    }
  }

  const deletarAtivo = async (id: number) => {
    try {
      const response = await fetch(`${baseURL}/api/ativos/${id}`, {
        method: 'DELETE',
        credentials: 'include'
      })

      if (!response.ok) {
        const error = await response.json()
        throw new Error(error.erro || 'Erro ao deletar ativo')
      }

      return await response.json()
    } catch (error) {
      console.error('Erro ao deletar ativo:', error)
      throw error
    }
  }

  const listarCandles = async (ativoId: number, page: number = 1, pageSize: number = 50) => {
    try {
      const response = await api<any>(`/api/ativos/${ativoId}/candles?page=${page}&pageSize=${pageSize}`)
      return response
    } catch (error) {
      console.error('Erro ao listar candles:', error)
      throw error
    }
  }

  return {
    listarAtivos,
    criarAtivo,
    obterAtivo,
    editarAtivo,
    deletarAtivo,
    listarCandles
  }
}

