export interface Backtest {
  id: number
  dataInicio: string
  dataFim: string
  entrada: number
  alvo: number
  numeroContratos: number
  ativoId: number
  stop: number
  estrategias: string
  proteger: boolean
  ativoNome?: string
  ativoCodigo?: string
  status: string // 'Iniciado' | 'Finalizado' | 'Erro'
  dataCriacao: string
  dataFinalizacao?: string
  resultado?: string
}

export interface PaginacaoResponse {
  items: Backtest[]
  totalItems: number
  page: number
  pageSize: number
  totalPages: number
}

export const useBacktests = () => {
  const { api, baseURL } = useApi()

  const listarBacktests = async (page: number = 1, pageSize: number = 10): Promise<PaginacaoResponse> => {
    try {
      const response = await api<PaginacaoResponse>(`/api/backtests?page=${page}&pageSize=${pageSize}`)
      return response
    } catch (error) {
      console.error('Erro ao listar backtests:', error)
      throw error
    }
  }

  const criarBacktest = async (
    dataInicio: Date,
    dataFim: Date,
    entrada: number,
    alvo: number,
    numeroContratos: number,
    ativoId: number,
    stop: number,
    estrategias: string[],
    proteger: boolean
  ) => {
    try {
      const response = await fetch(`${baseURL}/api/backtests`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          dataInicio: dataInicio.toISOString(),
          dataFim: dataFim.toISOString(),
          entrada,
          alvo,
          numeroContratos,
          ativoId,
          stop,
          estrategias,
          proteger
        }),
        credentials: 'include'
      })

      if (!response.ok) {
        const error = await response.json()
        throw new Error(error.erro || 'Erro ao criar backtest')
      }

      return await response.json()
    } catch (error) {
      console.error('Erro ao criar backtest:', error)
      throw error
    }
  }

  const obterBacktest = async (id: number): Promise<Backtest> => {
    try {
      const response = await api<Backtest>(`/api/backtests/${id}`)
      return response
    } catch (error) {
      console.error('Erro ao obter backtest:', error)
      throw error
    }
  }

  const deletarBacktest = async (id: number) => {
    try {
      const response = await fetch(`${baseURL}/api/backtests/${id}`, {
        method: 'DELETE',
        credentials: 'include'
      })

      if (!response.ok) {
        const error = await response.json()
        throw new Error(error.erro || 'Erro ao deletar backtest')
      }

      return await response.json()
    } catch (error) {
      console.error('Erro ao deletar backtest:', error)
      throw error
    }
  }

  return {
    listarBacktests,
    criarBacktest,
    obterBacktest,
    deletarBacktest
  }
}

