/**
 * Lista centralizada de estratégias disponíveis no sistema.
 * Use esta constante em todos os lugares onde precisar listar estratégias.
 * 
 * ORDEM DE PRIORIDADE:
 * 1. Quebra macro (mais usada)
 * 2. Consolidação
 * 3. Pullback no 50% micro
 * 4. Outros pullbacks
 * 5. Quebra micro (menos usada)
 */
export const ESTRATEGIAS = [
  'Quebra macro',
  'Consolidação',
  'Pullback no 50% micro',
  'Pullback de lado',
  'Pullback na inversão',
  'Pullback no 50% macro',
  'Quebra micro'
] as const

/**
 * Tipo TypeScript derivado da lista de estratégias.
 * Garante type-safety ao usar estratégias.
 */
export type Estrategia = typeof ESTRATEGIAS[number]

/**
 * Verifica se uma string é uma estratégia válida.
 */
export function isEstrategia(value: string): value is Estrategia {
  return ESTRATEGIAS.includes(value as Estrategia)
}

