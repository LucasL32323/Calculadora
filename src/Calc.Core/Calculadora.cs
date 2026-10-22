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
}
