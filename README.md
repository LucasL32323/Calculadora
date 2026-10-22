# TDD Dojo · Dia 01 — Calculadora

[![CI](https://github.com/LucasL32323/Calculadora/actions/workflows/ci.yml/badge.svg)](https://github.com/LucasL32323/Calculadora/actions/workflows/ci.yml)

> Série **TDD Dojo**: um projeto C# por dia útil, do simples ao complexo, sempre guiado por testes.
> Hoje: **Dia 01 de 25** · Fase 1 — Fundamentos · Stack: **.NET 10 (LTS)**, **C# 14**, **xUnit**.

## 🎯 Objetivo do dia

Construir uma calculadora com as 4 operações e um avaliador de expressões em texto (`"2 + 3"`),
**escrevendo cada teste antes do código**. O foco não é a calculadora — é internalizar o ritmo
Red → Green → Refactor e aprender a ler o histórico de commits como uma narrativa de design.

## 🔁 O ciclo Red-Green-Refactor

| Fase | O que você faz | Regra de ouro |
|---|---|---|
| 🔴 **Red** | Escreve **um** teste que descreve o próximo comportamento e vê ele falhar. | O teste tem que falhar **pelo motivo certo** (asserção/exceção esperada), não por erro bobo. |
| 🟢 **Green** | Escreve o **mínimo** de código para o teste passar. Pode ficar feio. | Nada de código "para o futuro". Se nenhum teste pede, não existe. |
| 🔵 **Refactor** | Melhora nomes, remove duplicação, organiza — **com tudo verde**. | Testes não mudam de comportamento. Se ficou vermelho, você mudou regra, não estrutura. |

**Por que o stub com `NotImplementedException` nos commits red?** Em C#, um teste chamando um
método inexistente nem compila. Compilar já é um "red" válido, mas o stub deixa o red mais
honesto: o projeto compila e o teste falha na execução, mostrando exatamente qual comportamento falta.

### O histórico deste repo é a aula

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
linha de regra de negócio — e os testes continuariam valendo. Essa separação é a semente da
Clean Architecture que vamos usar nos dias finais (pizzaria).

## 📏 Regras de negócio

1. Somar, subtrair, multiplicar e dividir dois números `decimal`.
2. Dividir por zero lança `DivisaoPorZeroException` ("Não é possível dividir por zero.").
3. Expressões no formato `<número> <operador> <número>`; espaços opcionais; ponto como decimal; operandos podem ser negativos.
4. Expressão fora do formato lança `ExpressaoInvalidaException`.
5. Todos os erros de regra herdam de `CalculadoraException`, então a CLI captura um tipo só.

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

**Pra praticar de verdade:** volte ao commit `d5e7caa` (`git checkout d5e7caa`), apague a
implementação e refaça o caminho sozinho, um teste por vez.

## 🏋️ Desafios extras (faça com TDD!)

1. Aceitar vírgula como separador decimal (`"1,5 + 2"`).
2. Operação de potência `^` e de resto `%`.
3. Tratar `OverflowException` (ex.: `79228162514264337593543950335 + 1`) como erro de domínio.
4. Histórico das últimas 5 operações (`Historico` na Calculadora).
5. **Boss fight:** expressões com várias operações e precedência (`2 + 3 * 4 = 14`).

## 🗺️ Roadmap do TDD Dojo

| Fase | Dias | Projetos |
|---|---|---|
| 1 · Fundamentos | 01–05 | Calculadora · Conversor de unidades · Validador de CPF/CNPJ · Números romanos · String Calculator |
| 2 · Domínio e OO | 06–10 | Conta bancária · Carrinho com cupons · Biblioteca com multas (`TimeProvider`) · Estacionamento · Jogo da velha |
| 3 · Dublês e arquitetura | 11–15 | Agenda com repositório · Notificações com mocks · Reserva de assentos · To-do em Clean Architecture · Result pattern + validação |
| 4 · Web API e dados | 16–20 | Minimal API + testes de integração · EF Core · Autenticação JWT · Paginação e ProblemDetails · Sistema de pedidos |
| 5 · Projeto final: Pizzaria | 21–25 | Cardápio e preços · Estoque de ingredientes · Pedidos e status · API + banco · Relatórios de vendas e alertas de estoque |
