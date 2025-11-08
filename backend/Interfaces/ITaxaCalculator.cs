namespace Backend.Interfaces;

/// <summary>
/// Interface para cálculo de taxas em operações de trading.
/// Preparada para implementações futuras (corretagem, emolumentos, etc).
/// </summary>
public interface ITaxaCalculator
{
    /// <summary>
    /// Calcula o valor total de taxas para uma operação.
    /// </summary>
    /// <param name="valorOperacao">Valor da operação em reais</param>
    /// <param name="numeroContratos">Número de contratos negociados</param>
    /// <returns>Valor total de taxas em reais</returns>
    decimal CalcularTaxa(decimal valorOperacao, int numeroContratos);
}





