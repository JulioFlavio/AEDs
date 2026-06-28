# CLAUDE.md — Contexto do Projeto para o Claude Code

Trabalho prático da disciplina **Algoritmos e Estruturas de Dados (AED)** — implementação do jogo de cartas **Buraco para 2 jogadores** em C# console, com ênfase em estruturas de dados implementadas manualmente.

---

## Comandos essenciais

```bash
# Compilar
dotnet build

# Rodar
dotnet run

# Compilar + rodar direto
dotnet run --project Buraco.csproj

# Semente fixa para demonstração (sempre termina em BATIDA)
# Escolha a opção 2 no menu e digite: 100
```

**Saídas geradas em tempo de execução** (ignoradas pelo `.gitignore`):
- `bin/` e `obj/` — artefatos de build
- `log_partida.txt` — log gerado ao rodar

---

## Arquitetura e pastas

```
Trabalho Prático/
├── Models/       — Classes de DADOS (o "o quê" do jogo)
├── Estruturas/   — Estruturas de dados implementadas MANUALMENTE
├── Services/     — REGRAS e comportamento (o "como" do jogo)
├── Documentacao/ — Documentação de estudo e guia de apresentação
├── Program.cs    — Menu e ponto de entrada
└── Buraco.csproj — Projeto .NET (net10.0, sem dependências externas)
```

---

## Estruturas de dados (restrição do trabalho)

**Regra inviolável: não usar `Stack<T>`, `Queue<T>`, `List<T>` ou qualquer coleção pronta do .NET.**

Todas as estruturas são construídas sobre o nó genérico `Estruturas/No.cs`:

| Classe | Tipo | Onde é usada |
|---|---|---|
| `PilhaCartas` | Pilha / LIFO | Monte (compra) e Lixo (descarte) |
| `FilaLog` | Fila / FIFO | Log de eventos da partida |
| `ListaCartas` | Lista encadeada simples | Mão do jogador, Morto, cartas de cada jogo na mesa |
| `ListaJogos` | Lista encadeada simples | Mesa do jogador (conjunto de jogos baixados) |

Se adicionar uma nova funcionalidade que precise de armazenamento, **use as estruturas acima**. Nunca introduza `List<T>`, arrays dinâmicos ou coleções do `System.Collections.Generic`.

---

## Convenções do código

- **Namespace raiz:** `Buraco`. Sub-namespaces: `Buraco.Models`, `Buraco.Estruturas`, `Buraco.Services`.
- **`Nullable = disable`** e **`ImplicitUsings = disable`** — todos os `using` são declarados explicitamente no topo de cada arquivo.
- **Estilo:** acadêmico. Comentários em **português** em todos os métodos públicos e blocos importantes. Nomes de variáveis e métodos em português.
- **Comparação de cartas** usa `object.ReferenceEquals` (não `.Equals`), porque o baralho é duplo — duas cartas com o mesmo valor são objetos distintos.
- **`No<T>`** é o único tipo genérico do projeto — não adicionar outros parâmetros de tipo.

---

## Classes principais e responsabilidades

### Models/
- **`Carta`** — `Numero` (1–13), `Naipe`, `EhCuringa` (Numero==2), `RankSequencia` (Ás→14), `Valor` (A/2=15, 3-7=5, 8-K=10).
- **`JogoMesa`** — um jogo (sequência) na mesa; método chave: `Classificar()` → `TipoCanastra`.
- **`TipoCanastra`** — enum: `NaoEhCanastra`, `Suja` (+100), `Limpa` (+200), `MeiaReal` (+250), `Real` (+500).
- **`Partida`** — estado completo: `Jogadores[2]`, `Monte` (Pilha), `Lixo` (Pilha), `MortoA`/`MortoB` (Lista), flags de disponibilidade, `JogadorDaVez`, `Encerrada`, `Batedor`.
- **`Jogador`** — `Mao` (ListaCartas), `Jogos` (ListaJogos), `Pontuacao`, `ComprouMorto`.

### Estruturas/
- **`No<T>`** — nó genérico: `Valor`, `Proximo`. Base de tudo.
- **`PilhaCartas`** — `topo` (No<Carta>), `Empilhar`/`Desempilhar`/`VerTopo`/`EstaVazia`/`Quantidade`.
- **`FilaLog`** — `inicio`/`fim` (No<string>), `Enfileirar`/`Desenfileirar`/`MontarTexto` (não-destrutivo)/`EstaVazia`.
- **`ListaCartas`** — `Adicionar` (fim), `AdicionarOrdenado` (por RankSequencia; curingas→MaxValue, ficam no fim), `Remover` (por ReferenceEquals), `ParaVetor`, `Quantidade`.
- **`ListaJogos`** — `Adicionar`, `ParaVetor`.

### Services/
- **`BaralhoService`** (static) — `CriarBaralhoDuplo()` (104 cartas), `Embaralhar()` Fisher-Yates O(n).
- **`PartidaService`** — motor principal; fluxo: `Iniciar()` → `Jogar()` → `ExecutarTurno()` → `FaseDeJogos()` → `Apurar()`.
- **`PontuacaoService`** (static) — `Calcular()` e `Detalhar()`; fórmula: `bonuses de canastras + cartas na mesa − cartas na mão − 100 (sem morto) + 100 (batedor)`.
- **`LogService`** — envolve `FilaLog`; `Registrar()`, `SalvarEmArquivo()` (não-destrutivo), `Imprimir()` (destrutivo/FIFO).

---

## Fluxo de um turno (para entender o `PartidaService`)

```
ExecutarTurno()
  ├─ Comprar()          — 1 carta do Monte (Pilha) ou Lixo inteiro (Pilha)
  ├─ FaseDeJogos()      — while(houveMudanca): TentarEncaixar + CriarNovosJogos + FecharCanastra
  ├─ Checar batida      — PodeReduzirPara(0): exige ComprouMorto + TemCanastra()
  ├─ EscolherDescarte() — prefere carta isolada de maior valor; nunca descarta curinga
  ├─ Pegar morto        — se mão ficou vazia pela 1ª vez
  └─ Checar batida      — novamente após pegar o morto
```

---

## Regras do Buraco adotadas

- Baralho duplo: 104 cartas (2×52). Número 2 = **curinga** (universal).
- Distribuição: 11 cartas por jogador + 2 mortos de 11 + resto no monte.
- Compra: 1 carta do monte **ou** o lixo inteiro.
- Jogos: sequência do mesmo naipe, 3+ cartas. Ás é **alto** (...Q, K, A). Máximo 1 curinga por jogo.
- Canastra = 7+ cartas. Tipos: Suja / Limpa / Meia Real (com Ás+curinga) / Real (com Ás, sem curinga).
- Morto: pegado ao esvaziar a mão pela 1ª vez. Quem não pega: −100 pontos.
- Batida: só com morto já em mãos **e** pelo menos 1 canastra. Bônus: +100.
- `MAX_TURNOS = 4000` — limite de segurança contra loop infinito.

---

## O que NÃO fazer

- Não importar `System.Collections.Generic` em nenhum arquivo.
- Não refatorar os comentários em inglês — o professor avalia os comentários em português.
- Não remover o `object.ReferenceEquals` na `ListaCartas.Remover()` — quebra a remoção de cartas com baralho duplo.
- Não trocar a lógica de `AdicionarOrdenado` para `.Sort()` de array — a lista deve permanecer encadeada manual.
- Não adicionar dependências externas ao `.csproj`.
- Não alterar `Nullable` ou `ImplicitUsings` sem necessidade — o estilo acadêmico é intencional.

---

## Documentação

- **[README.md](README.md)** — visão geral, guia de compilação e execução.
- **[Documentacao/DOCUMENTACAO.md](Documentacao/DOCUMENTACAO.md)** — 7 seções + 33 perguntas e respostas do professor.
- **[Documentacao/GUIA_APRESENTACAO.md](Documentacao/GUIA_APRESENTACAO.md)** — roteiro de apresentação, frases-chave, checklist.

**Semente de demonstração recomendada:** `100` — termina em BATIDA e exibe os 4 tipos de canastra.
