# Buraco — Trabalho Prático de Algoritmos e Estruturas de Dados

Implementação do jogo de cartas **Buraco** para **2 jogadores**, em **C# (aplicação console)**, usando **Programação Orientada a Objetos** e **estruturas de dados implementadas manualmente** (Pilha, Fila e Lista encadeada).

> O programa roda em **modo automático**: o próprio sistema joga pelos dois jogadores usando regras simples de decisão, registra **todos** os acontecimentos em um **log** (uma Fila) e, ao final, apura a pontuação e mostra o vencedor. Isso garante uma demonstração completa e repetível.

---

## ✅ O que o trabalho atende

| Requisito | Onde está |
|---|---|
| Baralho duplo (104 cartas) | `Services/BaralhoService.cs` |
| Distribuição inicial | `Services/PartidaService.cs` → `Iniciar()` |
| Monte (compra) | `Estruturas/PilhaCartas.cs` (Pilha) |
| Lixo (descarte/compra) | `Estruturas/PilhaCartas.cs` (Pilha) |
| Morto | `Estruturas/ListaCartas.cs` (Lista) |
| Mesa / formação de jogos | `Models/JogoMesa.cs` + `Estruturas/ListaJogos.cs` (Lista) |
| Canastra limpa / suja / real / meia real | `Models/JogoMesa.cs` → `Classificar()` |
| Batida | `Services/PartidaService.cs` → `RegistrarBatida()` |
| Contagem de pontos | `Services/PontuacaoService.cs` |
| Determinação do vencedor | `Services/PartidaService.cs` → `Apurar()` |
| Log completo da partida | `Estruturas/FilaLog.cs` (Fila) + `Services/LogService.cs` |
| Pilha, Fila e Lista (manuais) | pasta `Estruturas/` |

**Estruturas implementadas manualmente (sem `Stack<T>`, `Queue<T>` ou `List<T>` do .NET):**
- **Pilha** → `PilhaCartas` (Monte e Lixo)
- **Fila** → `FilaLog` (Log)
- **Lista encadeada** → `ListaCartas` (mão, morto, cartas de cada jogo) e `ListaJogos` (mesa)

---

## 📁 Estrutura do projeto

```
Trabalho Prático/
├── Buraco.csproj            # arquivo de projeto (.NET)
├── Program.cs               # menu e ponto de entrada
├── README.md               # este arquivo
│
├── Models/                  # classes de DADOS (o "o quê" do jogo)
│   ├── Carta.cs
│   ├── Naipe.cs
│   ├── TipoCanastra.cs
│   ├── JogoMesa.cs
│   ├── Jogador.cs
│   └── Partida.cs
│
├── Estruturas/              # estruturas de dados MANUAIS
│   ├── No.cs                # nó genérico (base de tudo)
│   ├── PilhaCartas.cs       # PILHA  -> Monte e Lixo
│   ├── FilaLog.cs           # FILA   -> Log
│   ├── ListaCartas.cs       # LISTA  -> mão / morto / cartas de um jogo
│   └── ListaJogos.cs        # LISTA  -> mesa (jogos baixados)
│
├── Services/                # classes de COMPORTAMENTO (as "regras")
│   ├── BaralhoService.cs    # cria e embaralha o baralho
│   ├── PartidaService.cs    # controla a partida (o "cérebro")
│   ├── PontuacaoService.cs  # calcula os pontos
│   └── LogService.cs        # registra o log
│
└── Documentacao/
    ├── DOCUMENTACAO.md      # documentação de estudo (detalhada)
    └── GUIA_APRESENTACAO.md # guia para apresentar ao professor
```

---

## 🛠️ Guia de compilação

**Pré-requisito:** ter o **.NET SDK** instalado (versão 6, 8 ou 10 — qualquer uma serve).
Para conferir, abra o terminal e rode:

```bash
dotnet --version
```

> O projeto está configurado para **`net10.0`** no arquivo `Buraco.csproj`.
> Se você tiver **outra** versão instalada (por exemplo `net6.0` ou `net8.0`), basta abrir o `Buraco.csproj` e trocar a linha:
> ```xml
> <TargetFramework>net10.0</TargetFramework>
> ```
> pela sua versão. O código **não usa nada específico de versão**, então compila em todas.

**Compilar:**

```bash
# entre na pasta do projeto (onde está o Buraco.csproj)
cd "Trabalho Prático"

# compila
dotnet build
```

Se aparecer **"Compilação com êxito. 0 Erro(s)"**, está tudo certo.

---

## ▶️ Guia de execução

```bash
dotnet run
```

Vai aparecer o menu:

```
============ BURACO - Trabalho de AED ============
 1 - Jogar uma partida (embaralhamento aleatorio)
 2 - Jogar uma partida (semente fixa - repetivel)
 3 - Mostrar as regras adotadas no trabalho
 0 - Sair
==================================================
```

- **Opção 1** — joga uma partida com embaralhamento aleatório (diferente a cada vez).
- **Opção 2** — pede uma **semente** (um número). A mesma semente gera **sempre a mesma partida** (ótimo para demonstrar e repetir o resultado).
- **Opção 3** — mostra as regras adotadas.

> 💡 **Dica para a demonstração:** use a **opção 2** com a semente **`100`**. Ela gera uma partida que termina em **BATIDA** e mostra **todos os tipos de canastra** (limpa, suja, real e meia real).

Ao final, o programa:
1. salva o **log completo** no arquivo `log_partida.txt`;
2. imprime o log na tela (na ordem em que tudo aconteceu — comportamento FIFO da Fila);
3. mostra o **resumo final** com os jogos de cada jogador, o detalhamento da pontuação e o **vencedor**.

---

## 📚 Documentação

Para o estudo aprofundado (explicação das classes, das estruturas de dados, do fluxo do jogo, explicação do código e **mais de 30 perguntas e respostas** que o professor pode fazer), veja:

➡️ **[Documentacao/DOCUMENTACAO.md](Documentacao/DOCUMENTACAO.md)**

Para apresentar o trabalho com segurança:

➡️ **[Documentacao/GUIA_APRESENTACAO.md](Documentacao/GUIA_APRESENTACAO.md)**
