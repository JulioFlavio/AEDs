# Guia de Apresentação ao Professor — Buraco (AED)

Este guia te ajuda a **apresentar o trabalho com segurança**, mesmo que esteja nervoso. Siga o roteiro, fale com calma e mostre que você **entende o que fez**.

---

## ⏱️ Roteiro de 5–8 minutos

### 1. Abertura (30s)
> "Implementei o jogo **Buraco** para 2 jogadores em **C#**, console, usando **POO**. O foco do trabalho foram as **estruturas de dados implementadas manualmente**: **Pilha**, **Fila** e **Lista encadeada**. A partida roda em **modo automático** e gera um **log completo**, o que facilita demonstrar tudo de ponta a ponta."

### 2. Mostrar a organização do projeto (1 min)
Abra as pastas e explique em uma frase cada uma:
- **`Models/`** — as coisas do jogo (Carta, Jogador, JogoMesa, Partida).
- **`Estruturas/`** — as estruturas feitas à mão (No, PilhaCartas, FilaLog, ListaCartas, ListaJogos).
- **`Services/`** — as regras e processos (Baralho, Partida, Pontuacao, Log).

> Frase-chave: *"Separei **dados** (Models), **estruturas** (Estruturas) e **comportamento** (Services) — isso é separação de responsabilidades."*

### 3. Rodar a demonstração (2 min)
No terminal, dentro da pasta do projeto:

```bash
dotnet run
```

Escolha a **opção 2** e digite a semente **`100`**.

Por que a semente 100? Porque essa partida termina em **BATIDA** e mostra **todos os tipos de canastra** (limpa, suja, real e meia real) — ou seja, demonstra os requisitos mais importantes de uma vez só.

Enquanto roda, aponte no **log**:
- "Aqui o jogador **comprou do MONTE**" (Pilha).
- "Aqui ele **comprou o LIXO inteiro**" (Pilha).
- "Aqui ele **baixou um jogo** / **encaixou** uma carta" (Listas/Mesa).
- "Aqui ele **pegou o morto**".
- "Aqui alguém **BATEU**".
- No fim: **detalhamento da pontuação** e **vencedor**.

> Mostre também que foi gerado o arquivo **`log_partida.txt`** com o log completo.

### 4. Explicar UMA estrutura por dentro (2 min)
Abra `Estruturas/PilhaCartas.cs` e explique `Empilhar`/`Desempilhar`:

> "A pilha guarda só o **topo**. Empilhar cria um nó que aponta para o topo antigo e vira o novo topo — **O(1)**. Desempilhar devolve o topo e desce para o nó de baixo — também **O(1)**. Usei pilha no monte e no lixo porque sempre mexemos **na carta de cima** (LIFO)."

Se sobrar tempo, mostre `FilaLog` (FIFO, com `inicio` e `fim`) e diga por que o log é uma fila.

### 5. Fechamento (30s)
> "Resumindo: **Pilha** no monte e no lixo, **Fila** no log e **Lista encadeada** na mão, no morto e na mesa. Todas implementadas manualmente, sem usar as coleções prontas do .NET. O jogo cobre baralho duplo, distribuição, compra, formação de jogos, as quatro canastras, batida, contagem de pontos e vencedor."

---

## 🎯 Frases-chave (decore estas)

- **Pilha = LIFO** (último a entrar, primeiro a sair) → monte e lixo.
- **Fila = FIFO** (primeiro a entrar, primeiro a sair) → log.
- **Lista encadeada** → cresce/diminui e remove do meio sem deslocar tudo → mão, morto, mesa.
- **`No<T>` genérico** é a base de todas as estruturas encadeadas.
- **Empilhar/Desempilhar/Enfileirar/Desenfileirar = O(1)**; buscar/remover na lista = O(n).
- **Comparo cartas por referência** porque o baralho é duplo (cartas iguais, objetos diferentes).

---

## 🧠 As 6 perguntas mais prováveis (respostas curtas)

1. **Por que pilha no monte?** Porque sempre compramos a carta **de cima** — LIFO. O(1).
2. **Por que fila no log?** Porque os eventos devem aparecer **na ordem em que aconteceram** — FIFO.
3. **Por que lista encadeada?** Porque a mão/mesa **mudam de tamanho** e precisam inserir/remover/percorrer; a lista faz isso sem tamanho fixo e sem deslocar elementos.
4. **Complexidade?** Pilha e fila: O(1). Lista: O(n) para buscar/remover. Embaralhar: O(n).
5. **Por que não usou `Stack`/`Queue`/`List` do .NET?** Porque o trabalho pede **implementação manual**, para mostrar que entendo o funcionamento interno.
6. **Como diferencia as canastras?** No `Classificar`: testo **tem curinga?** e **chega ao Ás?** → Real/Meia Real/Limpa/Suja.

> A lista completa com **30+ perguntas e respostas** está em **[DOCUMENTACAO.md, seção 7](DOCUMENTACAO.md#7-possíveis-perguntas-do-professor)**.

---

## ✅ Checklist antes de apresentar

- [ ] `dotnet build` compila com **0 erros**.
- [ ] `dotnet run` abre o menu.
- [ ] Opção **2** + semente **100** roda até o fim e mostra a **batida** e o **vencedor**.
- [ ] O arquivo **`log_partida.txt`** é gerado.
- [ ] Sei explicar **Pilha**, **Fila** e **Lista** e **por que** usei cada uma.
- [ ] Sei abrir `PilhaCartas.cs` e explicar `Empilhar`/`Desempilhar`.
- [ ] Sei dizer onde estão Monte (Pilha), Lixo (Pilha), Log (Fila) e Mesa/Mão (Lista).
- [ ] Li a seção 7 da documentação (perguntas do professor).

---

## 💬 Se algo der errado na hora

- **"Não compila na minha versão do .NET":** abra `Buraco.csproj` e troque `net10.0` pela versão instalada (`net6.0` ou `net8.0`). O código funciona em todas.
- **Os símbolos dos naipes aparecem estranhos:** é só questão de fonte do terminal; o jogo funciona igual. Já forçamos `UTF-8` na saída.
- **A partida terminou sem batida:** normal — às vezes o **monte acaba** antes. Rode de novo com a semente **100**, que sempre bate.
