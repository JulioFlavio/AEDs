using Buraco.Models;

namespace Buraco.Estruturas
{
    public class ListaJogos
    {
        private No<JogoMesa> inicio;
        private int quantidade;

        public int Quantidade
        {
            get { return quantidade; }
        }

        public ListaJogos()
        {
            inicio = null;
            quantidade = 0;
        }

        public bool EstaVazia()
        {
            return inicio == null;
        }

        public void Adicionar(JogoMesa jogo)
        {
            No<JogoMesa> novo = new No<JogoMesa>(jogo);

            if (inicio == null)
            {
                inicio = novo;
            }
            else
            {
                No<JogoMesa> atual = inicio;
                while (atual.Proximo != null)
                {
                    atual = atual.Proximo;
                }
                atual.Proximo = novo;
            }

            quantidade++;
        }

        public JogoMesa[] ParaVetor()
        {
            JogoMesa[] vetor = new JogoMesa[quantidade];
            No<JogoMesa> atual = inicio;
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
