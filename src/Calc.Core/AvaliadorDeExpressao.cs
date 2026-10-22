using System.Globalization;
using System.Text.RegularExpressions;
using Calc.Core.Excecoes;

namespace Calc.Core;

public class AvaliadorDeExpressao(Calculadora calculadora)
{
    private readonly Calculadora _calculadora = calculadora;

    public decimal Avaliar(string expressao)
    {
        var match = Regex.Match(expressao, @"^\s*(-?\d+(\.\d+)?)\s*([+\-*/])\s*(-?\d+(\.\d+)?)\s*$");
        if (!match.Success)
            throw new ExpressaoInvalidaException(expressao);

        var a = decimal.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
        var b = decimal.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);

        switch (match.Groups[3].Value)
        {
            case "+": return _calculadora.Somar(a, b);
            case "-": return _calculadora.Subtrair(a, b);
            case "*": return _calculadora.Multiplicar(a, b);
            case "/": return _calculadora.Dividir(a, b);
            default: throw new ExpressaoInvalidaException(expressao);
        }
    }
}
