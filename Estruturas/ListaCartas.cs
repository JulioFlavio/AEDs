using Buraco.Models;

namespace Buraco.Estruturas
{
    // ============================================================================
    // CLASSE: ListaCartas  (LISTA ENCADEADA implementada manualmente)
    // ----------------------------------------------------------------------------
    // OBJETIVO DA CLASSE:
    //   Implementar uma LISTA ENCADEADA (linked list) de cartas, SEM usar
    //   List<T> do .NET.
    //
    //   LISTA = colecao de elementos em sequencia. Diferente da pilha e da fila,
    //   a lista permite percorrer todos os elementos, inserir, remover de
    //   qualquer posicao e procurar um elemento especifico.
    //
    //   POR QUE USAR LISTA ENCADEADA AQUI?
    //     - MAO DO JOGADOR: o jogador adiciona cartas (compra) e remove cartas
    //       (baixa jogos / descarta) o tempo todo, em posicoes variadas.
    //       Uma lista encadeada cresce e diminui sem precisar de tamanho fixo
    //       e sem "empurrar" todos os elementos como faria um vetor.
    //     - MORTO: tambem e um conjunto de 11 cartas que depois passa para a mao.
    //     - CARTAS DE UM JOGO (canastra): ver classe JogoMesa.
    //
    //   Esta lista e ENCADEADA SIMPLES: cada No aponta apenas para o proximo.
    // ============================================================================
    public class ListaCartas
    {
        // Primeiro No da lista. Se for null, a lista esta vazia.
        private No<Carta> inicio;

        // Quantidade de cartas na lista.
        private int quantidade;

        // Propriedade de leitura: quantas cartas a lista tem.
        public int Quantidade
        {
            get { return quantidade; }
        }

        // Construtor: cria a lista vazia.
        public ListaCartas()
        {
            inicio = null;
            quantidade = 0;
        }

        // Retorna true se a lista nao tiver nenhuma carta.
        public bool EstaVazia()
        {
            return inicio == null;
        }

        // ------------------------------------------------------------------
        // Adicionar: insere uma carta no FINAL da lista.
        // (Percorre ate o ultimo No e liga a nova carta nele.)
        // ------------------------------------------------------------------
        public void Adicionar(Carta carta)
        {
            No<Carta> novo = new No<Carta>(carta);

            if (inicio == null)
            {
                // Lista vazia: a nova carta vira o inicio.
                inicio = novo;
            }
            else
            {
                // Caminha ate o ultimo No (aquele cujo Proximo e null).
                No<Carta> atual = inicio;
                while (atual.Proximo != null)
                {
                    atual = atual.Proximo;
                }
                // Liga o ultimo No ao novo.
                atual.Proximo = novo;
            }

            quantidade++;
        }

        // ------------------------------------------------------------------
        // AdicionarOrdenado: insere a carta JA na posicao certa, de modo que
        // a lista fique ordenada pelo numero da carta (rank da sequencia).
        // Os curingas (2) sao tratados como "maiores que tudo" e ficam no fim,
        // pois nao tem posicao fixa numa sequencia.
        // Usado pelos JOGOS (canastras) para manter as cartas em ordem e
        // facilitar a leitura na hora de mostrar na tela.
        // ------------------------------------------------------------------
        public void AdicionarOrdenado(Carta carta)
        {
            No<Carta> novo = new No<Carta>(carta);
            int chaveNova = Chave(carta);

            // Caso 1: lista vazia OU a nova carta entra antes do inicio.
            if (inicio == null || Chave(inicio.Valor) > chaveNova)
            {
                novo.Proximo = inicio;
                inicio = novo;
                quantidade++;
                return;
            }

            // Caso 2: procura o ponto de insercao no meio/fim da lista.
            No<Carta> atual = inicio;
            while (atual.Proximo != null && Chave(atual.Proximo.Valor) <= chaveNova)
            {
                atual = atual.Proximo;
            }

            // Insere o novo No logo depois de "atual".
            novo.Proximo = atual.Proximo;
            atual.Proximo = novo;
            quantidade++;
        }

        // Funcao auxiliar: devolve a "chave" usada para ordenar a carta.
        // Curinga vai para o fim (valor altissimo); demais usam o rank.
        private int Chave(Carta carta)
        {
            if (carta.EhCuringa)
            {
                return int.MaxValue;
            }
            return carta.RankSequencia;
        }

        // ------------------------------------------------------------------
        // Remover: tira da lista a carta indicada (o MESMO objeto carta).
        // Usamos comparacao por referencia, pois no baralho duplo podem existir
        // duas cartas "iguais" (mesmo numero e naipe), mas que sao objetos
        // diferentes. Assim removemos exatamente a carta desejada.
        // Retorna true se removeu, false se nao encontrou.
        // ------------------------------------------------------------------
        public bool Remover(Carta carta)
        {
            No<Carta> atual = inicio;
            No<Carta> anterior = null;

            while (atual != null)
            {
                if (object.ReferenceEquals(atual.Valor, carta))
                {
                    if (anterior == null)
                    {
                        // E o primeiro No: o inicio passa a ser o proximo.
                        inicio = atual.Proximo;
                    }
                    else
                    {
                        // "Pula" o No atual, ligando o anterior ao proximo.
                        anterior.Proximo = atual.Proximo;
                    }
                    quantidade--;
                    return true;
                }
                anterior = atual;
                atual = atual.Proximo;
            }

            // Nao encontrou a carta na lista.
            return false;
        }

        // ------------------------------------------------------------------
        // ParaVetor: devolve um VETOR (array) com todas as cartas da lista,
        // na ordem em que estao encadeadas.
        // E muito util para PERCORRER as cartas sem expor os Nos internos,
        // principalmente quando vamos remover cartas enquanto analisamos a mao
        // (mexer na lista enquanto percorremos o vetor copiado e mais seguro).
        // ------------------------------------------------------------------
        public Carta[] ParaVetor()
        {
            Carta[] vetor = new Carta[quantidade];
            No<Carta> atual = inicio;
            int i = 0;
            while (atual != null)
            {
                vetor[i] = atual.Valor;
                i++;
                atual = atual.Proximo;
            }
            return vetor;
        }
    }
}
