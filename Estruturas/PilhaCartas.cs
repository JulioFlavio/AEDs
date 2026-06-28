using System;
using Buraco.Models;

namespace Buraco.Estruturas
{
    // ============================================================================
    // CLASSE: PilhaCartas  (PILHA implementada manualmente)
    // ----------------------------------------------------------------------------
    // OBJETIVO DA CLASSE:
    //   Implementar uma PILHA (Stack) de cartas usando Nos encadeados,
    //   SEM usar a classe Stack<T> do .NET.
    //
    //   PILHA = estrutura LIFO (Last In, First Out / "ultimo a entrar,
    //   primeiro a sair"). Pense numa pilha de pratos: voce sempre coloca
    //   e retira o prato pelo TOPO.
    //
    //   POR QUE USAR PILHA NO BURACO?
    //     - MONTE: as cartas ficam viradas para baixo e o jogador SEMPRE
    //              compra a carta de cima (o topo). Isso e exatamente LIFO.
    //     - LIXO : o descarte tambem e empilhado; a carta de cima e a ultima
    //              descartada, e e ela que pode ser "comprada" pelo adversario.
    //
    //   Implementacao: guardamos apenas a referencia para o No do TOPO.
    //   Empilhar e desempilhar mexem somente no topo -> custo O(1) (constante).
    // ============================================================================
    public class PilhaCartas
    {
        // Referencia para o No que esta no topo da pilha.
        // Se "topo" for null, a pilha esta vazia.
        private No<Carta> topo;

        // Contador de quantas cartas existem na pilha.
        // Mantemos esse numero para nao precisar percorrer tudo so para contar.
        private int quantidade;

        // Propriedade de leitura: quantas cartas a pilha tem agora.
        public int Quantidade
        {
            get { return quantidade; }
        }

        // Construtor: cria a pilha vazia.
        public PilhaCartas()
        {
            topo = null;
            quantidade = 0;
        }

        // Retorna true se a pilha estiver vazia (sem nenhuma carta).
        public bool EstaVazia()
        {
            return topo == null;
        }

        // ------------------------------------------------------------------
        // Empilhar: coloca uma carta no TOPO da pilha.
        // ------------------------------------------------------------------
        public void Empilhar(Carta carta)
        {
            // Cria um novo No carregando a carta recebida.
            No<Carta> novo = new No<Carta>(carta);

            // O novo No passa a apontar para quem era o topo ate agora.
            novo.Proximo = topo;

            // Agora o novo No vira o novo topo.
            topo = novo;

            // Aumentamos o contador de cartas.
            quantidade++;
        }

        // ------------------------------------------------------------------
        // Desempilhar: remove e devolve a carta que esta no TOPO.
        // ------------------------------------------------------------------
        public Carta Desempilhar()
        {
            // Se a pilha estiver vazia, nao ha o que remover -> erro.
            if (topo == null)
            {
                throw new InvalidOperationException("Tentativa de desempilhar de uma pilha vazia.");
            }

            // Guarda a carta do topo para poder devolve-la no final.
            Carta cartaDoTopo = topo.Valor;

            // O topo passa a ser o proximo No (o de baixo).
            topo = topo.Proximo;

            // Diminui o contador.
            quantidade--;

            // Devolve a carta removida.
            return cartaDoTopo;
        }

        // ------------------------------------------------------------------
        // VerTopo: olha a carta do topo SEM remove-la (apenas "espiar").
        // Util para decidir se vale a pena comprar o lixo.
        // ------------------------------------------------------------------
        public Carta VerTopo()
        {
            if (topo == null)
            {
                throw new InvalidOperationException("Tentativa de ver o topo de uma pilha vazia.");
            }

            return topo.Valor;
        }
    }
}
