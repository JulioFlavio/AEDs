using Buraco.Models;

namespace Buraco.Estruturas
{
    public class ListaCartas
    {
        private No<Carta> inicio;
        private int quantidade;

        public int Quantidade
        {
            get { return quantidade; }
        }

        public ListaCartas()
        {
            inicio = null;
            quantidade = 0;
        }

        public bool EstaVazia()
        {
            return inicio == null;
        }

        public void Adicionar(Carta carta)
        {
            No<Carta> novo = new No<Carta>(carta);

            if (inicio == null)
            {
                inicio = novo;
            }
            else
            {
                No<Carta> atual = inicio;
                while (atual.Proximo != null)
                {
                    atual = atual.Proximo;
                }
                atual.Proximo = novo;
            }

            quantidade++;
        }

        public void AdicionarOrdenado(Carta carta)
        {
            No<Carta> novo = new No<Carta>(carta);
            int chaveNova = Chave(carta);

            if (inicio == null || Chave(inicio.Valor) > chaveNova)
            {
                novo.Proximo = inicio;
                inicio = novo;
                quantidade++;
                return;
            }

            No<Carta> atual = inicio;
            while (atual.Proximo != null && Chave(atual.Proximo.Valor) <= chaveNova)
            {
                atual = atual.Proximo;
            }

            novo.Proximo = atual.Proximo;
            atual.Proximo = novo;
            quantidade++;
        }

        private int Chave(Carta carta)
        {
            if (carta.EhCuringa)
            {
                return int.MaxValue;
            }
            return carta.RankSequencia;
        }

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
                        inicio = atual.Proximo;
                    }
                    else
                    {
                        anterior.Proximo = atual.Proximo;
                    }
                    quantidade--;
                    return true;
                }
                anterior = atual;
                atual = atual.Proximo;
            }

            return false;
        }

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
