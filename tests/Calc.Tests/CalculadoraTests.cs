using Calc.Core;

namespace Calc.Tests;

public class CalculadoraTests
{
    private readonly Calculadora _calculadora = new();

    public static TheoryData<decimal, decimal, decimal> CasosDeSoma => new()
    {
        { 2m, 3m, 5m },
        { -2m, -3m, -5m },
        { 0m, 0m, 0m },
        { 0.1m, 0.2m, 0.3m }, // o clássico que double erra e decimal acerta
    };

    [Theory]
    [MemberData(nameof(CasosDeSoma))]
    public void Somar_DoisNumeros_RetornaASoma(decimal a, decimal b, decimal esperado)
    {
        var resultado = _calculadora.Somar(a, b);

        Assert.Equal(esperado, resultado);
    }
}
