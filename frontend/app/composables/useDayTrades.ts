export interface DayTrade {
  id: number
  diaDayTrade: string
  ativoId: number
  ativoNome: string
  ativoCodigo: string
  dataCriacao: string
}

export interface PaginacaoResponse {
  items: DayTrade[]
  totalItems: number
  page: number
  pageSize: number
  totalPages: number
}

export const useDayTrades = () => {
  const { api, baseURL } = useApi()

  const listarDayTrades = async (page: number = 1, pageSize: number = 10): Promise<PaginacaoResponse> => {
    try {
      const response = await api<PaginacaoResponse>(`/api/daytrades?page=${page}&pageSize=${pageSize}`)
      return response
    } catch (error) {
      console.error('Erro ao listar daytrades:', error)
      throw error
    }
  }

  const criarDayTrade = async (diaDayTrade: Date, ativoId: number) => {
    try {
      const response = await fetch(`${baseURL}/api/daytrades`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          diaDayTrade: diaDayTrade.toISOString(),
          ativoId
        }),
        credentials: 'include'
      })

      if (!response.ok) {
        const error = await response.json()
        throw new Error(error.erro || 'Erro ao criar daytrade')
      }

      return await response.json()
    } catch (error) {
      console.error('Erro ao criar daytrade:', error)
      throw error
    }
  }

  const obterDayTrade = async (id: number): Promise<DayTrade> => {
    try {
      const response = await api<DayTrade>(`/api/daytrades/${id}`)
      return response
    } catch (error) {
      console.error('Erro ao obter daytrade:', error)
      throw error
    }
  }

  const deletarDayTrade = async (id: number) => {
    try {
      const response = await fetch(`${baseURL}/api/daytrades/${id}`, {
        method: 'DELETE',
        credentials: 'include'
      })

      if (!response.ok) {
        const error = await response.json()
        throw new Error(error.erro || 'Erro ao deletar daytrade')
      }

      return await response.json()
    } catch (error) {
      console.error('Erro ao deletar daytrade:', error)
      throw error
    }
  }

  return {
    listarDayTrades,
    criarDayTrade,
    obterDayTrade,
    deletarDayTrade
  }
}

