using System;
using System.Text;

namespace Buraco.Estruturas
{
    public class FilaLog
    {
        private No<string> inicio;
        private No<string> fim;
        private int quantidade;

        public int Quantidade
        {
            get { return quantidade; }
        }

        public FilaLog()
        {
            inicio = null;
            fim = null;
            quantidade = 0;
        }

        public bool EstaVazia()
        {
            return inicio == null;
        }

        public void Enfileirar(string mensagem)
        {
            No<string> novo = new No<string>(mensagem);

            if (fim == null)
            {
                inicio = novo;
                fim = novo;
            }
            else
            {
                fim.Proximo = novo;
                fim = novo;
            }

            quantidade++;
        }

        public string Desenfileirar()
        {
            if (inicio == null)
            {
                throw new InvalidOperationException("Tentativa de desenfileirar de uma fila vazia.");
            }

            string mensagem = inicio.Valor;
            inicio = inicio.Proximo;

            if (inicio == null)
            {
                fim = null;
            }

            quantidade--;
            return mensagem;
        }

        public string MontarTexto()
        {
            StringBuilder sb = new StringBuilder();
            No<string> atual = inicio;
            while (atual != null)
            {
                sb.AppendLine(atual.Valor);
                atual = atual.Proximo;
            }

            return sb.ToString();
        }
    }
}
