using Backend.Interfaces;

namespace Backend.Services;

/// <summary>
/// Implementação inicial do ITaxaCalculator que retorna taxa zero.
/// Posteriormente será substituída por cálculo real de taxas.
/// </summary>
public class ZeroTaxaCalculator : ITaxaCalculator
{
    public decimal CalcularTaxa(decimal valorOperacao, int numeroContratos)
    {
        // Por enquanto, taxa zero
        return 0;
    }
}





