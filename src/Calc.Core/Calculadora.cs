using Calc.Core.Excecoes;

namespace Calc.Core;

public class Calculadora
{
    public decimal Somar(decimal a, decimal b) => a + b;

    public decimal Subtrair(decimal a, decimal b) => a - b;

    public decimal Multiplicar(decimal a, decimal b) => a * b;

    public decimal Dividir(decimal dividendo, decimal divisor)
    {
        if (divisor == 0)
            throw new DivisaoPorZeroException();

        return dividendo / divisor;
    }

    /// <summary>
    /// Módulo matemático: o resultado tem o mesmo sinal do divisor
    /// (ex.: -7 mod 3 = 2). O operador % do C# devolve o resto, que daria -1.
    /// </summary>
    public decimal Modulo(decimal a, decimal b)
    {
        if (b == 0)
            throw new DivisaoPorZeroException();

        return ((a % b) + b) % b;
    }
}
