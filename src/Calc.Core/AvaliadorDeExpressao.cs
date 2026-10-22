using System.Globalization;
using System.Text.RegularExpressions;
using Calc.Core.Excecoes;

namespace Calc.Core;

/// <summary>
/// Avalia expressões simples no formato "&lt;número&gt; &lt;operador&gt; &lt;número&gt;".
/// Números usam ponto como separador decimal (ex.: 1.5). Operadores: + - * / %
/// </summary>
public sealed partial class AvaliadorDeExpressao
{
    private readonly IReadOnlyDictionary<char, Func<decimal, decimal, decimal>> _operacoes;

    public AvaliadorDeExpressao(Calculadora calculadora)
    {
        _operacoes = new Dictionary<char, Func<decimal, decimal, decimal>>
        {
            ['+'] = calculadora.Somar,
            ['-'] = calculadora.Subtrair,
            ['*'] = calculadora.Multiplicar,
            ['/'] = calculadora.Dividir,
            ['%'] = calculadora.Modulo,
        };
    }

    public decimal Avaliar(string expressao)
    {
        var match = PadraoDaExpressao().Match(expressao);
        if (!match.Success)
            throw new ExpressaoInvalidaException(expressao);

        var a = LerNumero(match, "a");
        var operador = match.Groups["op"].Value[0];
        var b = LerNumero(match, "b");

        return _operacoes[operador](a, b);
    }

    private static decimal LerNumero(Match match, string grupo) =>
        decimal.Parse(match.Groups[grupo].Value, CultureInfo.InvariantCulture);

    // Gerada em tempo de compilação (source generator): mais rápida e validada no build.
    [GeneratedRegex(@"^\s*(?<a>-?\d+(\.\d+)?)\s*(?<op>[+\-*/%])\s*(?<b>-?\d+(\.\d+)?)\s*$")]
    private static partial Regex PadraoDaExpressao();
}
