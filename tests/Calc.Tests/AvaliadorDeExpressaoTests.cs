using Calc.Core;
using Calc.Core.Excecoes;

namespace Calc.Tests;

public class AvaliadorDeExpressaoTests
{
    private readonly AvaliadorDeExpressao _avaliador = new(new Calculadora());

    public static TheoryData<string, decimal> ExpressoesValidas => new()
    {
        { "2 + 3", 5m },
        { "10 - 4", 6m },
        { "6 * 7", 42m },
        { "9 / 2", 4.5m },
        { "2+3", 5m },          // sem espaços
        { "  1.5 * 2  ", 3m },  // espaços nas pontas e decimal com ponto
        { "-5 + 2", -3m },      // primeiro operando negativo
        { "5 - -2", 7m },       // segundo operando negativo
    };

    [Theory]
    [MemberData(nameof(ExpressoesValidas))]
    public void Avaliar_ExpressaoValida_RetornaOResultado(string expressao, decimal esperado)
    {
        var resultado = _avaliador.Avaliar(expressao);

        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("2 +")]
    [InlineData("2 ^ 3")]
    [InlineData("2 + 3 + 4")]
    public void Avaliar_ExpressaoMalFormada_LancaExpressaoInvalidaException(string expressao)
    {
        var excecao = Assert.Throws<ExpressaoInvalidaException>(() => _avaliador.Avaliar(expressao));

        Assert.Contains("Expressão inválida", excecao.Message);
    }

    [Fact]
    public void Avaliar_DivisaoPorZero_PropagaDivisaoPorZeroException()
    {
        Assert.Throws<DivisaoPorZeroException>(() => _avaliador.Avaliar("10 / 0"));
    }
}
