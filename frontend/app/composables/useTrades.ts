export interface Trade {
  id: number
  dayTradeId: number
  gatilho1: number
  gatilho2: number
  regiao?: number
  operacao: string
  estrategia?: string
  dataCriacao: string
}

export const useTrades = () => {
  const { api, baseURL } = useApi()

  const listarTradesPorDayTrade = async (dayTradeId: number): Promise<Trade[]> => {
    try {
      const response = await api<Trade[]>(`/api/trades/daytrade/${dayTradeId}`)
      return response
    } catch (error) {
      console.error('Erro ao listar trades:', error)
      throw error
    }
  }

  const criarTrade = async (dayTradeId: number, gatilho1: number, gatilho2: number, regiao: number | null, operacao: string, estrategia?: string) => {
    try {
      const response = await fetch(`${baseURL}/api/trades`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          dayTradeId,
          gatilho1,
          gatilho2,
          regiao,
          operacao,
          estrategia
        }),
        credentials: 'include'
      })

      if (!response.ok) {
        const error = await response.json()
        throw new Error(error.erro || 'Erro ao criar trade')
      }

      return await response.json()
    } catch (error) {
      console.error('Erro ao criar trade:', error)
      throw error
    }
  }

  const deletarTrade = async (id: number) => {
    try {
      const response = await fetch(`${baseURL}/api/trades/${id}`, {
        method: 'DELETE',
        credentials: 'include'
      })

      if (!response.ok) {
        const error = await response.json()
        throw new Error(error.erro || 'Erro ao deletar trade')
      }

      return await response.json()
    } catch (error) {
      console.error('Erro ao deletar trade:', error)
      throw error
    }
  }

  return {
    listarTradesPorDayTrade,
    criarTrade,
    deletarTrade
  }
}

