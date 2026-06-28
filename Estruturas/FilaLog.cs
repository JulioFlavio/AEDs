using System;
using System.Text;

namespace Buraco.Estruturas
{
    // ============================================================================
    // CLASSE: FilaLog  (FILA implementada manualmente)
    // ----------------------------------------------------------------------------
    // OBJETIVO DA CLASSE:
    //   Implementar uma FILA (Queue) de mensagens de texto (string) usando Nos
    //   encadeados, SEM usar a classe Queue<T> do .NET.
    //
    //   FILA = estrutura FIFO (First In, First Out / "primeiro a entrar,
    //   primeiro a sair"). Pense numa fila de banco: quem chega primeiro
    //   e atendido primeiro.
    //
    //   POR QUE USAR FILA NO LOG?
    //     O log da partida e uma sequencia de acontecimentos em ORDEM
    //     CRONOLOGICA. O primeiro evento que acontece deve ser o primeiro
    //     a ser mostrado. Isso e exatamente o comportamento FIFO de uma fila:
    //     registramos os eventos no fim (Enfileirar) e, no final, "reproduzimos"
    //     a partida lendo do inicio (Desenfileirar) na mesma ordem em que
    //     ocorreram.
    //
    //   Para a fila ser eficiente, guardamos DOIS ponteiros:
    //     - inicio: onde retiramos (a "cabeca" da fila)
    //     - fim   : onde inserimos (a "cauda" da fila)
    //   Assim, tanto Enfileirar quanto Desenfileirar custam O(1) (constante).
    // ============================================================================
    public class FilaLog
    {
        // Primeiro No da fila (de onde sai a proxima mensagem).
        private No<string> inicio;

        // Ultimo No da fila (onde entra a mensagem mais nova).
        private No<string> fim;

        // Quantidade de mensagens guardadas.
        private int quantidade;

        // Propriedade de leitura: quantas mensagens existem na fila.
        public int Quantidade
        {
            get { return quantidade; }
        }

        // Construtor: cria a fila vazia.
        public FilaLog()
        {
            inicio = null;
            fim = null;
            quantidade = 0;
        }

        // Retorna true se a fila estiver vazia.
        public bool EstaVazia()
        {
            return inicio == null;
        }

        // ------------------------------------------------------------------
        // Enfileirar: adiciona uma mensagem no FIM da fila.
        // ------------------------------------------------------------------
        public void Enfileirar(string mensagem)
        {
            // Cria um novo No com a mensagem.
            No<string> novo = new No<string>(mensagem);

            if (fim == null)
            {
                // Fila estava vazia: o novo No e, ao mesmo tempo, inicio e fim.
                inicio = novo;
                fim = novo;
            }
            else
            {
                // Liga o antigo ultimo No ao novo, e atualiza o "fim".
                fim.Proximo = novo;
                fim = novo;
            }

            quantidade++;
        }

        // ------------------------------------------------------------------
        // Desenfileirar: remove e devolve a mensagem do INICIO da fila.
        // ------------------------------------------------------------------
        public string Desenfileirar()
        {
            if (inicio == null)
            {
                throw new InvalidOperationException("Tentativa de desenfileirar de uma fila vazia.");
            }

            // Guarda a mensagem que esta na frente.
            string mensagem = inicio.Valor;

            // Avanca o inicio para o proximo No.
            inicio = inicio.Proximo;

            // Se a fila ficou vazia, o "fim" tambem precisa virar null.
            if (inicio == null)
            {
                fim = null;
            }

            quantidade--;
            return mensagem;
        }

        // ------------------------------------------------------------------
        // MontarTexto: percorre a fila SEM remover nada e devolve todas as
        // mensagens juntas, uma por linha. Usado para salvar o log em arquivo.
        // ------------------------------------------------------------------
        public string MontarTexto()
        {
            // StringBuilder e usado para montar um texto grande de forma
            // eficiente (concatenar string com "+" varias vezes seria lento).
            StringBuilder sb = new StringBuilder();

            // Percorremos do inicio ate o fim seguindo os ponteiros "Proximo".
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
