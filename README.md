# Calculadora

[![CI](https://github.com/LucasL32323/Calculadora/actions/workflows/ci.yml/badge.svg)](https://github.com/LucasL32323/Calculadora/actions/workflows/ci.yml)

Calculadora em **C# / .NET 10** com as operações `+ - * / %` e um avaliador de expressões em texto (`"2 + 3"`),
desenvolvida com **TDD** (testes escritos antes do código) e testes em **xUnit**.

## 🔁 O ciclo Red-Green-Refactor

| Fase | O que você faz | Regra de ouro |
|---|---|---|
| 🔴 **Red** | Escreve **um** teste que descreve o próximo comportamento e vê ele falhar. | O teste tem que falhar **pelo motivo certo** (asserção/exceção esperada), não por erro bobo. |
| 🟢 **Green** | Escreve o **mínimo** de código para o teste passar. Pode ficar feio. | Nada de código "para o futuro". Se nenhum teste pede, não existe. |
| 🔵 **Refactor** | Melhora nomes, remove duplicação, organiza — **com tudo verde**. | Testes não mudam de comportamento. Se ficou vermelho, você mudou regra, não estrutura. |

**Por que o stub com `NotImplementedException` nos commits red?** Em C#, um teste chamando um
método inexistente nem compila. Compilar já é um "red" válido, mas o stub deixa o red mais
honesto: o projeto compila e o teste falha na execução, mostrando exatamente qual comportamento falta.

### O histórico de commits

Rode `git log --reverse --oneline` e acompanhe:

| Commit | Fase | O que aconteceu |
|---|---|---|
| `b4d1466` | ⚙️ setup | Solução, 3 projetos, CI e configs compartilhadas. |
| `d5e7caa` | 🔴 red | Testes de soma (inclui `0.1 + 0.2 = 0.3`). Stub lança `NotImplementedException`. |
| `dfed22a` | 🟢 green | `a + b`. Só isso. |
| `0802c84` | 🔴 red | Testes de subtração, multiplicação e divisão. |
| `4a05c84` | 🟢 green | Implementação direta das três. |
| `8741c82` | 🔴 red | Divisão por zero deve lançar **`DivisaoPorZeroException`** com mensagem amigável. |
| `2678826` | 🟢 green | Guard clause `if (divisor == 0)`. |
| `f6b69b8` | 🔴 red | Avaliador de expressões: 8 casos válidos, 6 inválidos, divisão por zero propagada. |
| `a94214d` | 🟢 green | Regex com índices de grupo (`Groups[1]`, `Groups[4]`…) + `switch`. Funciona, mas é frágil. |
| `99c73b7` | 🔵 refactor | Grupos nomeados, `[GeneratedRegex]` e dicionário de operadores. **Nenhum teste mudou.** |
| `ee35426` | ✨ feat | CLI interativa consumindo o Core. |

> 💡 Repare no red de `8741c82`: `decimal` **já** lança `DivideByZeroException` nativamente.
> Se o teste esperasse essa exceção, passaria de primeira — e teste que nunca falhou não prova nada.
> A regra de negócio (exceção do domínio + mensagem pro usuário) é o que tornou o red real.

## 🏗️ Arquitetura

```
Calculadora/
├── Calc.slnx                     # solução no formato novo (XML) do .NET 10
├── Directory.Build.props         # configs comuns: nullable, warnings como erro
├── global.json                   # fixa o SDK .NET 10
├── .github/workflows/ci.yml      # build + testes a cada push
├── src/
│   ├── Calc.Core/                # 🧠 regras de negócio (zero dependências)
│   │   ├── Calculadora.cs
│   │   ├── AvaliadorDeExpressao.cs
│   │   └── Excecoes/CalculadoraException.cs
│   └── Calc.Cli/                 # 🖥️ interface: só lê/escreve no console
│       └── Program.cs
└── tests/
    └── Calc.Tests/               # ✅ xUnit
        ├── CalculadoraTests.cs
        └── AvaliadorDeExpressaoTests.cs
```

```
Calc.Cli ──► Calc.Core ◄── Calc.Tests
```

O **Core não conhece a interface**. A CLI poderia virar API, app desktop ou bot sem tocar numa
linha de regra de negócio — e os testes continuariam valendo. Essa separação é a base da
Clean Architecture.

## 📏 Regras de negócio

1. Somar, subtrair, multiplicar, dividir e calcular o módulo (`%`) de dois números `decimal`.
2. Dividir ou calcular módulo por zero lança `DivisaoPorZeroException` ("Não é possível dividir por zero.").
3. O módulo é matemático: o resultado tem o sinal do divisor (`-7 % 3 = 2`, enquanto o `%` puro do C# daria `-1`).
4. Expressões no formato `<número> <operador> <número>`; espaços opcionais; ponto como decimal; operandos podem ser negativos. Operadores: `+ - * / %`.
5. Expressão fora do formato lança `ExpressaoInvalidaException`.
6. Todos os erros de regra herdam de `CalculadoraException`, então a CLI captura um tipo só.

## 🧠 Decisões técnicas (nível sênior)

- **`decimal` em vez de `double`**: `0.1 + 0.2` em `double` dá `0.30000000000000004`. Para dinheiro e contas exatas, `decimal`. Existe um teste só para isso.
- **`TheoryData<T...>` + `[MemberData]`**: `[InlineData]` não aceita `decimal` (atributos só aceitam constantes primitivas). `TheoryData` é tipado e lê bem.
- **Nome do teste = especificação**: `Metodo_Cenario_ResultadoEsperado`. Quando falha, o nome já diz o que quebrou.
- **Arrange / Act / Assert**: blocos separados por linha em branco em todo teste.
- **Exceções de domínio** em vez de exceções genéricas: a camada de cima decide como exibir; o Core só diz *o que* deu errado.
- **`TreatWarningsAsErrors`**: pegou, durante o desenvolvimento, um parâmetro de construtor não utilizado no stub. Warning ignorado vira bug amanhã.
- **`[GeneratedRegex]`**: a regex é gerada em compilação — mais rápida e com erro de sintaxe detectado no build, não em produção.
- **Por que `Calc.*` e não `Calculadora.*`?** Uma classe `Calculadora` dentro do namespace `Calculadora.Core` gera o erro CS0118 nos testes (o compilador acha o *namespace* `Calculadora` antes da *classe*). Pelo mesmo motivo a interface se chama `Calc.Cli` e não `Calc.Console` (colidiria com `System.Console`).

## ▶️ Como rodar

Pré-requisito: [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/LucasL32323/Calculadora.git
cd Calculadora

dotnet test                          # roda os testes
dotnet run --project src/Calc.Cli    # abre a calculadora
```

```
> 9 / 2
= 4,5
> 10 / 0
Erro: Não é possível dividir por zero.
```
