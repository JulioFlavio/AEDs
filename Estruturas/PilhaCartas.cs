using System;
using Buraco.Models;

namespace Buraco.Estruturas
{
    public class PilhaCartas
    {
        private No<Carta> topo;
        private int quantidade;

        public int Quantidade
        {
            get { return quantidade; }
        }

        public PilhaCartas()
        {
            topo = null;
            quantidade = 0;
        }

        public bool EstaVazia()
        {
            return topo == null;
        }

        public void Empilhar(Carta carta)
        {
            No<Carta> novo = new No<Carta>(carta);
            novo.Proximo = topo;
            topo = novo;
            quantidade++;
        }

        public Carta Desempilhar()
        {
            if (topo == null)
            {
                throw new InvalidOperationException("Tentativa de desempilhar de uma pilha vazia.");
            }

            Carta cartaDoTopo = topo.Valor;
            topo = topo.Proximo;
            quantidade--;
            return cartaDoTopo;
        }

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
