using Calc.Core;
using Calc.Core.Excecoes;

var avaliador = new AvaliadorDeExpressao(new Calculadora());

Console.WriteLine("Calculadora TDD — digite uma expressão (ex.: 2 + 3) ou 'sair'.");

while (true)
{
    Console.Write("> ");
    var entrada = Console.ReadLine();

    if (entrada is null || entrada.Trim().Equals("sair", StringComparison.OrdinalIgnoreCase))
        break;

    try
    {
        Console.WriteLine($"= {avaliador.Avaliar(entrada)}");
    }
    catch (CalculadoraException ex)
    {
        Console.WriteLine($"Erro: {ex.Message}");
    }
}
