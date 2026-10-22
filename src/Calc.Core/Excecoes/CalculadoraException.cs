namespace Calc.Core.Excecoes;

/// <summary>
/// Base para todos os erros de regra de negócio da calculadora.
/// Quem consome (ex.: a CLI) captura só esta classe e mostra a mensagem ao usuário.
/// </summary>
public abstract class CalculadoraException(string mensagem) : Exception(mensagem);

public sealed class DivisaoPorZeroException()
    : CalculadoraException("Não é possível dividir por zero.");

public sealed class ExpressaoInvalidaException(string expressao)
    : CalculadoraException($"Expressão inválida: '{expressao}'. Use o formato <número> <operador> <número>, ex.: 2 + 3.");
