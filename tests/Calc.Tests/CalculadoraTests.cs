using Calc.Core;
using Calc.Core.Excecoes;

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

    public static TheoryData<decimal, decimal, decimal> CasosDeSubtracao => new()
    {
        { 5m, 3m, 2m },
        { 3m, 5m, -2m },
        { -2m, -3m, 1m },
        { 1.5m, 0.5m, 1m },
    };

    [Theory]
    [MemberData(nameof(CasosDeSubtracao))]
    public void Subtrair_DoisNumeros_RetornaADiferenca(decimal a, decimal b, decimal esperado)
    {
        var resultado = _calculadora.Subtrair(a, b);

        Assert.Equal(esperado, resultado);
    }

    public static TheoryData<decimal, decimal, decimal> CasosDeMultiplicacao => new()
    {
        { 6m, 7m, 42m },
        { -2m, 3m, -6m },
        { -2m, -3m, 6m },
        { 123m, 0m, 0m },
        { 2.5m, 4m, 10m },
    };

    [Theory]
    [MemberData(nameof(CasosDeMultiplicacao))]
    public void Multiplicar_DoisNumeros_RetornaOProduto(decimal a, decimal b, decimal esperado)
    {
        var resultado = _calculadora.Multiplicar(a, b);

        Assert.Equal(esperado, resultado);
    }

    public static TheoryData<decimal, decimal, decimal> CasosDeDivisao => new()
    {
        { 10m, 2m, 5m },
        { 9m, 2m, 4.5m },
        { -10m, 4m, -2.5m },
        { 0m, 5m, 0m },
    };

    [Theory]
    [MemberData(nameof(CasosDeDivisao))]
    public void Dividir_DivisorDiferenteDeZero_RetornaOQuociente(decimal dividendo, decimal divisor, decimal esperado)
    {
        var resultado = _calculadora.Dividir(dividendo, divisor);

        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void Dividir_PorZero_LancaDivisaoPorZeroExceptionComMensagemAmigavel()
    {
        var excecao = Assert.Throws<DivisaoPorZeroException>(() => _calculadora.Dividir(10m, 0m));

        Assert.Equal("Não é possível dividir por zero.", excecao.Message);
    }
}
