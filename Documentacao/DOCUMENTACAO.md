# Documentação de Estudo — Buraco (Trabalho de Algoritmos e Estruturas de Dados)

Este documento explica **todo** o projeto em profundidade. Ele foi escrito para que você consiga **entender** e **defender** o trabalho diante do professor, mesmo as partes que parecem complexas.

Sumário:

1. [Visão Geral do Projeto](#1-visão-geral-do-projeto)
2. [Estrutura de Pastas](#2-estrutura-de-pastas)
3. [Explicação das Classes](#3-explicação-das-classes)
4. [Estruturas de Dados](#4-estruturas-de-dados)
5. [Fluxo do Jogo](#5-fluxo-do-jogo)
6. [Explicação do Código (arquivo por arquivo)](#6-explicação-do-código-arquivo-por-arquivo)
7. [Possíveis Perguntas do Professor (30+)](#7-possíveis-perguntas-do-professor)

---

## 1. Visão Geral do Projeto

O projeto é uma implementação do jogo de cartas **Buraco** para **2 jogadores**, feita em **C#** como **aplicação de console**.

O objetivo principal, do ponto de vista da disciplina, **não é** criar uma inteligência artificial perfeita de Buraco, e sim **exercitar estruturas de dados clássicas** (Pilha, Fila e Lista) implementadas **manualmente**, dentro de um problema real e organizado com **Programação Orientada a Objetos (POO)**.

Como o trabalho funciona:

- O programa monta um **baralho duplo** (104 cartas), embaralha e **distribui**.
- A partida roda em **modo automático**: o sistema joga pelos dois jogadores usando **regras simples de decisão (heurísticas)**. Assim, a partida sempre vai do início ao fim, sem depender de digitação a cada jogada, o que facilita a demonstração.
- Cada acontecimento (compra, baixar jogo, encaixe, descarte, pegar o morto, batida) é **registrado em um log** (uma **Fila**), que é exibido no final na ordem exata em que ocorreu.
- Ao final, o sistema **conta os pontos**, aplica os bônus e penalidades e **determina o vencedor**.

Conceitos da disciplina presentes:

- **Pilha (LIFO)** → Monte e Lixo.
- **Fila (FIFO)** → Log da partida.
- **Lista encadeada** → mão do jogador, morto, mesa (jogos) e cartas de cada jogo.
- **Nó genérico** (`No<T>`) → a peça que forma todas as estruturas encadeadas.
- **Vetor (array)** e **algoritmo de embaralhamento (Fisher-Yates)**.
- **POO**: classes, atributos, métodos, encapsulamento, enumerações, generics.

---

## 2. Estrutura de Pastas

A organização separa **dados**, **estruturas** e **comportamento**. Isso é uma boa prática de POO chamada **separação de responsabilidades**.

```
Trabalho Prático/
├── Buraco.csproj      → arquivo de projeto do .NET (como compilar)
├── Program.cs         → menu e início do programa (a "casca")
│
├── Models/            → classes de DADOS (representam o "o quê")
├── Estruturas/        → estruturas de dados feitas à mão (Pilha, Fila, Listas)
├── Services/          → classes de COMPORTAMENTO (as regras e os processos)
└── Documentacao/      → documentação e guia de apresentação
```

### `Models/` — o que existe no jogo
Contém as classes que **representam as coisas do jogo**, sem regras complicadas:
- `Carta.cs`, `Naipe.cs`, `TipoCanastra.cs` — a carta e seus tipos.
- `JogoMesa.cs` — uma sequência/canastra baixada na mesa.
- `Jogador.cs` — um jogador (nome, mão, jogos, pontuação, se pegou o morto).
- `Partida.cs` — o **estado** completo da partida (jogadores, monte, lixo, mortos).

### `Estruturas/` — como os dados são guardados
Contém as **estruturas de dados implementadas manualmente**:
- `No.cs` — o nó genérico, base de todas as estruturas encadeadas.
- `PilhaCartas.cs` — a **Pilha** (Monte e Lixo).
- `FilaLog.cs` — a **Fila** (Log).
- `ListaCartas.cs` — a **Lista encadeada** de cartas (mão, morto, cartas de um jogo).
- `ListaJogos.cs` — a **Lista encadeada** de jogos (a mesa).

### `Services/` — o que acontece no jogo
Contém as classes que **executam as regras e os processos**:
- `BaralhoService.cs` — cria e embaralha o baralho.
- `PartidaService.cs` — o **cérebro**: conduz os turnos do início ao fim.
- `PontuacaoService.cs` — calcula os pontos de cada jogador.
- `LogService.cs` — registra e exibe o log.

> **Por que separar Models de Services?**
> Porque `Carta` ou `Jogador` apenas **guardam informação**; já `PartidaService` **faz coisas** (movimenta cartas, aplica regras). Separar deixa o código mais organizado, mais fácil de entender e de manter.

---

## 3. Explicação das Classes

### 3.1 `Carta` (Models/Carta.cs)
**Responsabilidade:** representar uma carta do baralho.

**Atributos:**
- `Numero` (int): 1 = Ás, 2 = curinga, 3..10 numéricas, 11 = J, 12 = Q, 13 = K.
- `Naipe` (enum): Copas, Ouros, Paus ou Espadas.

**Métodos/propriedades importantes:**
- `EhCuringa` — diz se a carta é um 2 (curinga).
- `RankSequencia` — valor de ordem da carta na sequência; o Ás vira 14 (Ás alto).
- `Valor` — pontos da carta (A e 2 = 15; 3..7 = 5; 8..K = 10).
- `NomeNumero`, `SimboloNaipe`, `ToString()` — para exibição (ex.: `K♠`, `2♦(C)`).

### 3.2 `Naipe` (enum) e `TipoCanastra` (enum)
- `Naipe` — lista fixa dos quatro naipes.
- `TipoCanastra` — classifica um jogo: `NaoEhCanastra`, `Suja`, `Limpa`, `MeiaReal`, `Real`.

### 3.3 `JogoMesa` (Models/JogoMesa.cs)
**Responsabilidade:** representar **um** jogo baixado (uma sequência do mesmo naipe). Quando tem 7+ cartas, é uma **canastra**.

**Atributos:**
- `Naipe` — o naipe do jogo.
- `cartas` (ListaCartas) — as cartas do jogo, guardadas numa **lista encadeada manual**, em ordem.

**Métodos importantes:**
- `AdicionarCarta`, `Cartas`, `Quantidade`.
- `ContemCuringa`, `QuantidadeCuringas`, `ContemRank`, `MinNatural`, `MaxNatural`, `ContemAsNatural`.
- `EhCanastra` (7+ cartas), `Classificar` (define o tipo), `PontosBonus`, `PontosDasCartas`.
- `ToString` — mostra o jogo e sua classificação.

### 3.4 `Jogador` (Models/Jogador.cs)
**Responsabilidade:** guardar tudo que pertence a um jogador.

**Atributos (os obrigatórios + extensões):**
- `Nome` (string).
- `Mao` (ListaCartas) — cartas na mão.
- `Jogos` (ListaJogos) — jogos baixados na mesa (extensão para organizar a mesa por jogador).
- `Pontuacao` (int).
- `ComprouMorto` (bool) — se já pegou o morto.

### 3.5 `Partida` (Models/Partida.cs)
**Responsabilidade:** guardar o **estado** completo da partida (é uma classe de dados; quem aplica as regras é o `PartidaService`).

**Atributos:**
- `Jogadores[]`, `Monte` (Pilha), `Lixo` (Pilha), `MortoA`/`MortoB` (Listas) e seus *disponíveis*.
- `JogadorDaVez`, `NumeroRodada`, `Encerrada`, `Batedor`, `Vencedor`.

### 3.6 `No<T>` (Estruturas/No.cs)
**Responsabilidade:** ser a **caixinha** de uma estrutura encadeada. Guarda um `Valor` e a referência para o `Proximo` nó. É **genérico** (`<T>`) — funciona com qualquer tipo.

### 3.7 `PilhaCartas` (Estruturas/PilhaCartas.cs)
**Responsabilidade:** Pilha (LIFO) de cartas. Métodos: `Empilhar`, `Desempilhar`, `VerTopo`, `EstaVazia`, `Quantidade`.

### 3.8 `FilaLog` (Estruturas/FilaLog.cs)
**Responsabilidade:** Fila (FIFO) de mensagens. Métodos: `Enfileirar`, `Desenfileirar`, `MontarTexto`, `EstaVazia`, `Quantidade`.

### 3.9 `ListaCartas` e `ListaJogos` (Estruturas/)
**Responsabilidade:** Listas encadeadas. `ListaCartas` guarda cartas (mão, morto, jogo); `ListaJogos` guarda jogos (mesa). Métodos: `Adicionar`, `Remover`, `ParaVetor`, `Quantidade`, etc. `ListaCartas` ainda tem `AdicionarOrdenado` (insere já em ordem).

### 3.10 Services
- `BaralhoService` — `CriarBaralhoDuplo`, `Embaralhar`.
- `LogService` — `Registrar`, `Imprimir`, `SalvarEmArquivo`.
- `PontuacaoService` — `Calcular`, `Detalhar`.
- `PartidaService` — `Iniciar`, `Jogar` e todos os métodos privados que conduzem os turnos.

---

## 4. Estruturas de Dados

Esta é a parte central do trabalho. Aqui explicamos **o que é** cada estrutura, **como** ela foi implementada e **por que** foi escolhida.

### 4.1 O que é uma PILHA (Stack)

Uma **pilha** é uma estrutura **LIFO** — *Last In, First Out* ("o último a entrar é o primeiro a sair").

Analogia: uma **pilha de pratos**. Você sempre coloca um prato **em cima** e tira o prato **de cima**. Não dá para tirar um prato do meio sem mexer nos de cima.

Operações principais:
- **Empilhar (push):** coloca um elemento no topo.
- **Desempilhar (pop):** remove e devolve o elemento do topo.
- **Ver topo (peek):** olha o topo sem remover.

**Por que usamos Pilha no MONTE?**
No Buraco, o monte fica com as cartas viradas para baixo e o jogador **sempre compra a carta de cima**. Isso é exatamente o comportamento de uma pilha: a última carta colocada (topo) é a primeira a ser comprada. Modelar o monte como pilha é **natural e fiel à regra do jogo**.

**Por que usamos Pilha no LIXO?**
O lixo (descarte) também é empilhado: cada carta descartada vai **por cima** das anteriores. A carta que pode ser "comprada" pelo adversário é justamente a **do topo** (a última descartada). De novo, LIFO descreve a realidade.

### 4.2 O que é uma FILA (Queue)

Uma **fila** é uma estrutura **FIFO** — *First In, First Out* ("o primeiro a entrar é o primeiro a sair").

Analogia: uma **fila de banco**. Quem chega primeiro é atendido primeiro.

Operações principais:
- **Enfileirar (enqueue):** coloca um elemento no **fim** da fila.
- **Desenfileirar (dequeue):** remove e devolve o elemento do **início** da fila.

**Por que usamos Fila no LOG?**
O log é a **história da partida em ordem cronológica**. O primeiro evento que aconteceu deve ser o primeiro a ser mostrado. Guardando os eventos numa fila (enfileirando ao acontecer) e lendo do início no final (desenfileirando), reproduzimos a partida **exatamente na ordem real**. FIFO é a definição perfeita de "ordem de chegada".

### 4.3 O que é uma LISTA ENCADEADA (Linked List)

Uma **lista encadeada** é uma sequência de **nós**, em que cada nó guarda um valor e aponta para o **próximo**. Diferente da pilha e da fila, a lista permite **percorrer**, **inserir** e **remover** em posições variadas e **procurar** elementos.

Analogia: uma **caça ao tesouro** — cada pista (nó) leva à próxima. Se você quiser tirar uma pista do meio, basta fazer a pista anterior apontar para a seguinte.

Vantagem sobre o vetor (array): a lista **cresce e diminui** sem precisar de tamanho fixo e **sem empurrar** todos os elementos quando você remove um do meio (num vetor, remover do meio obriga a deslocar tudo).

**Por que usamos Lista na MÃO do jogador?**
A mão muda o tempo todo: o jogador **compra** (adiciona), **baixa jogos** (remove várias cartas) e **descarta** (remove). Uma lista encadeada lida bem com esse vai-e-vem, crescendo e diminuindo livremente.

**Por que usamos Lista na MESA (jogos)?**
A mesa de um jogador é um conjunto de jogos que **cresce** ao longo da partida e que precisamos **percorrer** para tentar encaixar cartas e somar pontos. Inserir no fim e percorrer tudo é exatamente o que uma lista faz bem.

**Por que usamos Lista no MORTO e nas cartas de um JOGO?**
O morto é um conjunto de 11 cartas que depois "vira" a mão — uma lista resolve. As cartas de um jogo também ficam numa lista, mantida **em ordem** (`AdicionarOrdenado`) para ficar fácil de ler.

### 4.4 Quadro-resumo das escolhas

| Elemento do jogo | Estrutura | Tipo | Por quê |
|---|---|---|---|
| Monte | `PilhaCartas` | Pilha (LIFO) | Compra-se sempre a carta de cima |
| Lixo | `PilhaCartas` | Pilha (LIFO) | Descarte empilhado; topo é a última descartada |
| Log | `FilaLog` | Fila (FIFO) | Eventos em ordem cronológica |
| Mão | `ListaCartas` | Lista | Muda muito (compra/baixa/descarta) |
| Morto | `ListaCartas` | Lista | Conjunto que depois vira a mão |
| Cartas de um jogo | `ListaCartas` | Lista | Sequência ordenada que cresce |
| Mesa (jogos) | `ListaJogos` | Lista | Conjunto de jogos que cresce e é percorrido |

---

## 5. Fluxo do Jogo

### Passo 1 — Criação do baralho
`BaralhoService.CriarBaralhoDuplo()` monta um vetor de **104 cartas**: dois baralhos de 52 (4 naipes × 13 números × 2 cópias).

### Passo 2 — Embaralhamento
`BaralhoService.Embaralhar()` usa o algoritmo **Fisher-Yates**: do fim para o começo, troca cada carta com uma posição sorteada. É justo (toda ordem é igualmente provável) e rápido (**O(n)**).

### Passo 3 — Distribuição (`PartidaService.Iniciar`)
- 11 cartas para cada jogador (vão para a `Mao`, que é uma Lista);
- 2 **mortos** de 11 cartas (`MortoA`, `MortoB`, Listas);
- o **restante** (60 cartas) é empilhado no **Monte** (Pilha);
- o **Lixo** começa vazio.

### Passo 4 — Compra (`Comprar`)
No início do turno, o jogador **compra**:
- **do Monte** (desempilha a carta do topo), **ou**
- **do Lixo inteiro** (quando a heurística `DeveComprarDoLixo` julga vantajoso — por exemplo, o topo é curinga ou encaixa em algum jogo).

### Passo 5 — Formação de jogos (`FaseDeJogos`)
O jogador tenta, em ordem:
1. **Encaixar** cartas da mão nos jogos já baixados (`TentarEncaixar`);
2. **Baixar** novas sequências de 3+ cartas do mesmo naipe (`CriarNovosJogos`);
3. **Fechar canastra** usando um curinga numa sequência de 6 cartas (`FecharCanastraComCuringa`).

Se a mão **esvazia** e o jogador ainda **não pegou o morto**, ele **pega o morto** (as 11 cartas viram a nova mão) e continua jogando.

Depois, o jogador **descarta** uma carta no Lixo (`Descartar`), encerrando a parte normal do turno.

### Passo 6 — Batida (`RegistrarBatida`)
Um jogador **bate** (encerra a partida) quando:
- já **pegou o morto**, **e**
- tem **pelo menos uma canastra**, **e**
- **zera a mão** (com ou sem descarte).

Há uma regra interna (`PodeReduzirPara`) que **impede batidas ilegais** e garante que sempre sobre carta para o descarte quando ainda não dá para bater.

A partida também termina se o **monte acabar** (ninguém bate) ou pelo limite de segurança de turnos.

### Passo 7 — Pontuação (`PontuacaoService` + `Apurar`)
Para cada jogador:

```
pontos =  bônus das canastras (200 limpa, 100 suja, 500 real, 250 meia real)
        + valor das cartas baixadas na mesa
        - valor das cartas que sobraram na mão
        - 100 se NÃO pegou o morto
        + 100 se foi quem bateu
```

Quem tiver **mais pontos** vence (empate é possível).

---

## 6. Explicação do Código (arquivo por arquivo)

> Os arquivos `.cs` já estão **totalmente comentados** linha a linha. Abaixo destacamos os trechos mais importantes e explicamos a **lógica** por trás deles, com foco nas estruturas de dados (que é o coração do trabalho).

### 6.1 `Estruturas/No.cs` — o nó genérico

```csharp
public class No<T>
{
    public T Valor;        // o dado guardado
    public No<T> Proximo;  // referência para o próximo nó (null = fim)

    public No(T valor)
    {
        Valor = valor;
        Proximo = null;    // ao nascer, o nó não está ligado a ninguém
    }
}
```

**Lógica:** todo encadeamento (pilha, fila, listas) é feito de nós ligados pelo campo `Proximo`. Como é **genérico** (`<T>`), o mesmo nó serve para `Carta`, `string` ou `JogoMesa`. Isso evita repetir código e é um conceito importante de POO (reaproveitamento via generics).

### 6.2 `Estruturas/PilhaCartas.cs` — a Pilha

Campos:
```csharp
private No<Carta> topo;   // só precisamos saber quem é o topo
private int quantidade;   // contador, para não ter que percorrer só para contar
```

**Empilhar** (insere no topo, O(1)):
```csharp
public void Empilhar(Carta carta)
{
    No<Carta> novo = new No<Carta>(carta); // cria o nó
    novo.Proximo = topo;                   // o novo aponta para o antigo topo
    topo = novo;                           // o novo passa a ser o topo
    quantidade++;
}
```
Linha a linha: criamos um nó com a carta; ligamos esse nó ao que era o topo; promovemos o novo nó a topo; somamos 1 no contador. Tudo em tempo **constante** — não percorremos nada.

**Desempilhar** (remove do topo, O(1)):
```csharp
public Carta Desempilhar()
{
    if (topo == null)
        throw new InvalidOperationException("...pilha vazia.");
    Carta cartaDoTopo = topo.Valor;  // guarda para devolver
    topo = topo.Proximo;             // topo passa a ser o de baixo
    quantidade--;
    return cartaDoTopo;
}
```
Lógica: se está vazia, é erro. Senão, guardamos a carta do topo, "descemos" o topo para o próximo nó e devolvemos a carta removida. O nó antigo fica sem referência e é descartado automaticamente pelo .NET.

**VerTopo** apenas devolve `topo.Valor` sem remover — usado para decidir se vale comprar o lixo.

### 6.3 `Estruturas/FilaLog.cs` — a Fila

Campos:
```csharp
private No<string> inicio; // de onde sai a próxima mensagem
private No<string> fim;    // onde entra a mensagem mais nova
private int quantidade;
```
Guardar `inicio` **e** `fim` é o truque que faz **enfileirar** custar O(1) (sem precisar percorrer até o fim).

**Enfileirar** (insere no fim):
```csharp
public void Enfileirar(string mensagem)
{
    No<string> novo = new No<string>(mensagem);
    if (fim == null) { inicio = novo; fim = novo; }   // fila estava vazia
    else { fim.Proximo = novo; fim = novo; }          // liga no fim e avança o fim
    quantidade++;
}
```

**Desenfileirar** (remove do início):
```csharp
public string Desenfileirar()
{
    if (inicio == null) throw new InvalidOperationException("...fila vazia.");
    string mensagem = inicio.Valor;
    inicio = inicio.Proximo;          // avança o início
    if (inicio == null) fim = null;   // se esvaziou, o fim também zera
    quantidade--;
    return mensagem;
}
```
Lógica: pega a mensagem da frente, avança o `inicio`; se a fila ficou vazia, zera o `fim` também (senão `fim` ficaria apontando para um nó que não existe mais na fila).

**MontarTexto** percorre do `inicio` ao `fim` **sem remover**, juntando tudo com `StringBuilder` — usado para salvar o log em arquivo.

### 6.4 `Estruturas/ListaCartas.cs` — a Lista encadeada de cartas

**Adicionar** (insere no fim):
```csharp
public void Adicionar(Carta carta)
{
    No<Carta> novo = new No<Carta>(carta);
    if (inicio == null) inicio = novo;          // lista vazia
    else {
        No<Carta> atual = inicio;
        while (atual.Proximo != null) atual = atual.Proximo; // anda até o fim
        atual.Proximo = novo;                   // liga o último ao novo
    }
    quantidade++;
}
```

**AdicionarOrdenado** insere a carta **já na posição certa** (ordenada pelo rank; curingas vão para o fim). É usada pelos **jogos** para manter as cartas em ordem e facilitar a leitura. A função `Chave` devolve `int.MaxValue` para curinga (joga para o fim) e o `RankSequencia` para as demais.

**Remover** (por referência):
```csharp
public bool Remover(Carta carta)
{
    No<Carta> atual = inicio, anterior = null;
    while (atual != null) {
        if (object.ReferenceEquals(atual.Valor, carta)) {
            if (anterior == null) inicio = atual.Proximo; // era o primeiro
            else anterior.Proximo = atual.Proximo;        // "pula" o nó
            quantidade--; return true;
        }
        anterior = atual; atual = atual.Proximo;
    }
    return false;
}
```
**Por que comparar por referência (`ReferenceEquals`)?** Porque no baralho duplo existem **duas cartas iguais** (mesmo número e naipe), mas são **objetos diferentes**. Comparando por referência, removemos **exatamente** a carta que queremos, e não a sua "gêmea".

**ParaVetor** copia a lista para um vetor. Isso é importante quando vamos **remover cartas enquanto analisamos a mão**: percorrer uma cópia (vetor) e mexer na lista original ao mesmo tempo é mais seguro do que mexer na lista enquanto a percorremos.

### 6.5 `Estruturas/ListaJogos.cs` — a Lista de jogos (mesa)
Mesma ideia da `ListaCartas`, mas guardando `JogoMesa`. Tem `Adicionar` (no fim) e `ParaVetor` (para percorrer). É a **mesa** de cada jogador.

### 6.6 `Models/Carta.cs`
Pontos de destaque:
- `RankSequencia` transforma o Ás (número 1) em **14**, porque no Buraco a sequência mais forte termina no Ás (`...Q, K, A`).
- `Valor` define os pontos (A e 2 = 15; 3..7 = 5; 8..K = 10).
- `ToString` monta o texto (`K♠`, `2♦(C)`), marcando o curinga com `(C)`.

### 6.7 `Models/JogoMesa.cs`
O método central é **`Classificar`**:
```csharp
public TipoCanastra Classificar()
{
    if (!EhCanastra()) return TipoCanastra.NaoEhCanastra; // menos de 7 cartas
    bool limpa = !ContemCuringa();
    bool comAs = ContemAsNatural();
    if (comAs && limpa) return TipoCanastra.Real;     // 7+, sem curinga, com Ás
    if (comAs && !limpa) return TipoCanastra.MeiaReal;// 7+, com curinga, com Ás
    if (limpa) return TipoCanastra.Limpa;             // 7+, sem curinga
    return TipoCanastra.Suja;                         // 7+, com curinga
}
```
**Lógica:** primeiro verifica se já é canastra (7+). Depois usa dois indicadores — **tem curinga?** e **chega ao Ás?** — para escolher entre as quatro categorias. `PontosBonus` traduz cada categoria nos pontos (500/250/200/100).

`MinNatural`/`MaxNatural` acham as **pontas** da sequência (onde dá para encaixar), ignorando curingas. `ContemRank` evita repetir um número na mesma sequência.

### 6.8 `Models/Jogador.cs` e `Models/Partida.cs`
São classes de **dados**. `Jogador` junta `Nome`, `Mao` (Lista), `Jogos` (Lista), `Pontuacao` e `ComprouMorto`. `Partida` junta os dois jogadores, o `Monte` (Pilha), o `Lixo` (Pilha), os dois mortos (Listas) e os controles de turno/encerramento.

### 6.9 `Services/BaralhoService.cs`
`CriarBaralhoDuplo` usa **três laços** (2 cópias × 4 naipes × 13 números) para preencher o vetor de 104 cartas. `Embaralhar` é o **Fisher-Yates**:
```csharp
for (int i = baralho.Length - 1; i > 0; i--) {
    int j = sorteio.Next(i + 1);   // sorteia 0..i
    // troca baralho[i] com baralho[j]
}
```

### 6.10 `Services/LogService.cs`
Envolve a `FilaLog`. `Registrar` faz `Enfileirar`. `Imprimir` faz **`Desenfileirar` em laço** (esvaziando a fila e mostrando tudo na ordem real — demonstra o FIFO na prática). `SalvarEmArquivo` usa `MontarTexto` (que **não** esvazia) e por isso é chamado **antes** de `Imprimir`.

### 6.11 `Services/PontuacaoService.cs`
`Calcular` soma bônus das canastras + cartas na mesa − cartas na mão − penalidade do morto + bônus da batida. `Detalhar` monta o texto que explica parcela por parcela (aparece no resumo final).

### 6.12 `Services/PartidaService.cs` — o cérebro
É o maior arquivo. Vale entender os métodos por blocos:

- **`Iniciar`** — cria o estado, embaralha e distribui (11/11, dois mortos, resto no monte).
- **`Jogar`** — laço principal: enquanto a partida não acabou, executa um turno, troca a vez e conta as rodadas. No fim chama `Apurar`. Tem um **limite de segurança** (`MAX_TURNOS`) para garantir que sempre termina.
- **`ExecutarTurno`** — orquestra um turno: comprar → fase de jogos → (batida sem descarte?) → descartar → (pegar morto via descarte?) → (batida com descarte?).
- **`Comprar` / `DeveComprarDoLixo`** — compra do monte ou do lixo, conforme a heurística.
- **`FaseDeJogos`** — repete encaixar/criar/fechar canastra enquanto houver mudança; pega o morto se a mão esvaziar.
- **`TentarEncaixar`, `CriarNovosJogos`, `FecharCanastraComCuringa`** — as três formas de baixar cartas.
- **`PodeAdicionarAoJogo`** — a regra que diz se uma carta encaixa num jogo (mesmo naipe, na ponta certa, sem repetir rank; curinga no máximo 1 por jogo).
- **`PodeReduzirPara`** — a regra que **impede batida ilegal** e garante carta para o descarte.
- **`Descartar` / `EscolherDescarte`** — escolhe e joga uma carta no lixo (prefere cartas isoladas e de maior valor; nunca descarta curinga sem necessidade).
- **`PegarMorto`, `TemCanastra`, `RegistrarBatida`, `EncerrarPorMonteVazio`** — morto, verificação de canastra e fins de partida.
- **`Apurar` / `ImprimirResumoFinal`** — calcula pontos, decide o vencedor e imprime o resumo.

**Por que a "fase de jogos" é um laço `while (houveMudanca)`?**
Porque baixar uma carta pode **abrir** uma nova possibilidade (ex.: encaixar uma carta deixa um jogo de 6 que agora pode virar canastra com curinga). Repetimos até não haver mais nada a fazer. Como cada passo **reduz a mão** (ou pega o morto), o laço **sempre termina**.

---

## 7. Possíveis Perguntas do Professor

> 30 perguntas com respostas completas. Estude estas — são as mais prováveis.

**1) Por que você usou uma PILHA para o monte?**
Porque no Buraco compramos **sempre a carta de cima** do monte. Esse é o comportamento **LIFO** (último a entrar, primeiro a sair) de uma pilha. As operações de empilhar e desempilhar mexem só no topo e custam **O(1)**. Modelar o monte como pilha é fiel à regra do jogo e eficiente.

**2) Por que o lixo também é uma pilha?**
Porque o descarte é **empilhado**: cada carta vai por cima da anterior. A carta que pode ser comprada é a **do topo** (a última descartada). Quando o jogador "compra o lixo", na nossa implementação ele leva o lixo inteiro, o que fazemos desempilhando tudo. LIFO descreve bem o lixo.

**3) Por que o log é uma FILA?**
Porque o log é a **história em ordem cronológica**. O primeiro evento deve ser o primeiro a ser mostrado — isso é **FIFO**. Enfileiramos cada evento quando ele acontece e, no final, desenfileiramos do início, reproduzindo a partida na ordem exata.

**4) Por que você implementou uma LISTA ENCADEADA?**
Porque a mão, o morto, a mesa e as cartas de cada jogo **mudam de tamanho** e exigem **inserir/remover/percorrer** em posições variadas. A lista encadeada cresce e diminui sem tamanho fixo e remove um elemento do meio só "reapontando" o nó anterior, sem deslocar todo o resto (como faria um vetor).

**5) Qual a diferença entre pilha, fila e lista?**
- **Pilha (LIFO):** insere e remove só no **topo**.
- **Fila (FIFO):** insere no **fim**, remove do **início**.
- **Lista:** permite **percorrer**, inserir/remover em **qualquer posição** e **buscar**. Pilha e fila são, na verdade, listas com **regras de acesso restritas**.

**6) Qual a complexidade das operações?**
- Pilha: `Empilhar`, `Desempilhar`, `VerTopo` → **O(1)**.
- Fila: `Enfileirar`, `Desenfileirar` → **O(1)** (porque guardamos `inicio` e `fim`).
- Lista: `Adicionar` no fim e `Remover`/buscar → **O(n)** (precisa percorrer); `ParaVetor` → **O(n)**.
- Embaralhar (Fisher-Yates) → **O(n)**.

**7) Por que `Enfileirar` é O(1) se a fila é encadeada?**
Porque guardamos um ponteiro para o **fim** da fila. Assim inserimos direto no último nó, sem percorrer a fila toda. Sem o ponteiro de fim, seria O(n).

**8) Por que você não usou `Stack<T>`, `Queue<T>` e `List<T>` do .NET?**
Porque o trabalho exige **implementar manualmente** as estruturas, para demonstrar que eu entendo **como elas funcionam por dentro**. Usei apenas vetores (`array`) e `StringBuilder`, que são recursos básicos.

**9) O que é o `No<T>` e por que é genérico?**
É o **nó** que forma as estruturas encadeadas: guarda um `Valor` e a referência para o `Proximo`. É genérico (`<T>`) para servir a qualquer tipo (`Carta`, `string`, `JogoMesa`) sem reescrever código. Isso evita duplicação.

**10) Como funciona o embaralhamento? É justo?**
Uso o algoritmo **Fisher-Yates**: do fim para o começo, troco cada carta com uma posição sorteada entre 0 e o índice atual. Ele é **justo** (toda permutação tem a mesma chance) e roda em **O(n)**.

**11) Por que o Ás vale rank 14 na sequência?**
Porque no Buraco a sequência mais valiosa termina no Ás (`...Q, K, A`). Tratando o Ás como a carta mais alta (14), o As fica "no topo" da sequência, o que permite identificar a **canastra real** (que chega ao Ás).

**12) Como você representa o curinga?**
Pela regra adotada, **todo 2 é curinga**. A propriedade `EhCuringa` verifica `Numero == 2`. É uma simplificação didática (não tratamos o "2 natural"), o que deixa o código mais simples sem perder os conceitos.

**13) Como o programa diferencia canastra limpa, suja, real e meia real?**
No método `Classificar` de `JogoMesa`, usando dois testes: **tem curinga?** e **chega ao Ás?**
- com Ás e sem curinga → **Real** (500);
- com Ás e com curinga → **Meia Real** (250);
- sem Ás e sem curinga → **Limpa** (200);
- sem Ás e com curinga → **Suja** (100).
Todas precisam de **7+ cartas** para serem canastra.

**14) O que é a batida e quais as condições para bater?**
Bater é **encerrar a partida** zerando a mão. Só é permitido se o jogador (a) **já pegou o morto** e (b) tem **pelo menos uma canastra**. O método `RegistrarBatida` marca o batedor e encerra. Quem bate ganha **+100** de bônus.

**15) Como o morto é tratado?**
Há **dois mortos** de 11 cartas (jogo de 2 pessoas). Quando o jogador **esvazia a mão pela primeira vez**, ele **pega um morto** (as 11 cartas viram a nova mão) e marca `ComprouMorto = true`. Quem termina **sem** pegar o morto leva **−100** de penalidade.

**16) Como a pontuação é calculada?**
`pontos = bônus das canastras + valor das cartas na mesa − valor das cartas na mão − 100 (se não pegou o morto) + 100 (se bateu)`. Está em `PontuacaoService.Calcular`, e `Detalhar` mostra cada parcela.

**17) Como é decidido o vencedor?**
No método `Apurar`: compara as pontuações dos dois jogadores; quem tiver **mais pontos** vence. Se empatarem, o resultado é **empate**.

**18) Por que a partida é automática e não interativa?**
Para garantir uma **demonstração completa e repetível**, sem depender de muitas digitações. O foco do trabalho são as **estruturas de dados**; o modo automático percorre todas elas (compra do monte/lixo, baixar jogos, morto, batida, pontuação) de ponta a ponta. Mesmo assim há uma **semente fixa** (opção 2) para reproduzir exatamente a mesma partida.

**19) A lógica de jogada é uma IA inteligente?**
Não — são **heurísticas simples** (regras diretas): encaixar o que der, baixar sequências de 3+, fechar canastra com curinga e descartar a carta mais "isolada". Foi proposital: uma IA ótima de Buraco fugiria do escopo da disciplina.

**20) Como você garante que a partida sempre termina (não trava)?**
De três formas: (a) toda compra do monte **reduz** o monte, que é finito; (b) a fase de jogos sempre **reduz a mão** ou pega o morto, então o laço interno termina; (c) há um **limite de segurança** `MAX_TURNOS` no laço principal.

**21) O que impede uma "batida ilegal"?**
O método `PodeReduzirPara`. Ele só deixa a mão chegar a zero se for uma **batida legal** (morto pego + canastra). Quando ainda não dá para bater, ele **obriga a manter cartas** para o descarte, evitando terminar a mão de forma inválida.

**22) Por que remover cartas comparando por referência e não por valor?**
Porque o baralho é **duplo**: existem duas cartas com o mesmo número e naipe, mas que são **objetos diferentes**. Comparando por **referência** (`ReferenceEquals`), removo exatamente a carta desejada, sem confundir com a cópia.

**23) Por que copiar a lista para um vetor (`ParaVetor`) antes de percorrer?**
Porque, ao analisar a mão, eu **removo cartas** dela (ao baixar jogos). Percorrer uma **cópia** (vetor) enquanto altero a lista original evita erros de "modificar a coleção enquanto a percorro".

**24) Qual a vantagem da lista encadeada sobre o vetor aqui?**
A mão e a mesa **mudam de tamanho** constantemente. No vetor, remover do meio obriga a **deslocar** todos os elementos seguintes (O(n)) e o tamanho é fixo. Na lista, removo "reapontando" um nó, e ela cresce/diminui à vontade.

**25) E a desvantagem da lista encadeada?**
**Acesso por índice é O(n)** (preciso percorrer) e cada nó gasta memória extra com a referência `Proximo`. Para este jogo, isso não é problema, pois as listas são pequenas (mão com ~11 a 30 cartas).

**26) Por que separar Models, Estruturas e Services?**
Por **separação de responsabilidades**: `Models` guardam dados, `Estruturas` definem como os dados são organizados e `Services` contêm as regras/processos. Isso deixa o código **organizado, legível e fácil de manter**, e é uma boa prática de POO.

**27) O que é encapsulamento e onde ele aparece?**
Encapsular é **esconder os detalhes internos** e expor só o necessário. Exemplo: na `PilhaCartas`, o campo `topo` é **privado**; quem usa a pilha só enxerga `Empilhar`, `Desempilhar` e `VerTopo`. Assim ninguém "quebra" a estrutura por fora.

**28) Por que algumas estruturas têm um campo `quantidade` separado?**
Para responder "quantos elementos têm?" em **O(1)**, sem precisar percorrer a estrutura inteira só para contar. Atualizamos o contador a cada inserção/remoção.

**29) O que acontece quando a fila do log é esvaziada na impressão?**
O método `Imprimir` usa `Desenfileirar` em laço, então ele **consome** a fila ao mostrar. Por isso, quando também queremos salvar em arquivo, chamamos `SalvarEmArquivo` **antes** (ele usa `MontarTexto`, que **não** consome).

**30) Como o sistema sabe onde uma carta "encaixa" em um jogo?**
No método `PodeAdicionarAoJogo`: a carta precisa ser do **mesmo naipe**, encaixar em uma das **pontas** da sequência (`MaxNatural+1` ou `MinNatural−1`) e **não repetir** um rank já presente. O curinga pode entrar (no máximo 1 por jogo) desde que haja espaço para estender a sequência.

**31) Por que cada jogo aceita no máximo 1 curinga?**
É uma **simplificação didática** que torna a classificação direta: **0 curinga = limpa**, **1 curinga = suja**. Isso evita casos ambíguos e deixa o código mais simples, mantendo todos os conceitos pedidos.

**32) Onde estão a Pilha, a Fila e a Lista no código, exatamente?**
- Pilha → `Estruturas/PilhaCartas.cs` (usada em `Partida.Monte` e `Partida.Lixo`).
- Fila → `Estruturas/FilaLog.cs` (usada em `LogService`).
- Lista → `Estruturas/ListaCartas.cs` (mão, morto, cartas de um jogo) e `Estruturas/ListaJogos.cs` (mesa).

**33) Como reproduzir exatamente a mesma partida para conferência?**
Use a **opção 2** do menu e informe uma **semente** (ex.: `100`). A mesma semente gera a mesma sequência de embaralhamento e, portanto, a **mesma partida** toda vez.
