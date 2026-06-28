using Buraco.Models;

namespace Buraco.Estruturas
{
    // ============================================================================
    // CLASSE: ListaJogos  (LISTA ENCADEADA de JOGOS / a "mesa" do jogador)
    // ----------------------------------------------------------------------------
    // OBJETIVO DA CLASSE:
    //   Implementar uma LISTA ENCADEADA cujos elementos sao JOGOS (objetos
    //   JogoMesa), ou seja, as sequencias/canastras que o jogador ja baixou
    //   na mesa. NAO usa List<T> do .NET.
    //
    //   POR QUE UMA LISTA AQUI?
    //     A "mesa" de um jogador e um conjunto de jogos que vai CRESCENDO ao
    //     longo da partida (cada vez que ele baixa uma nova sequencia).
    //     Tambem precisamos PERCORRER todos os jogos para tentar encaixar
    //     cartas e para contar pontos no final. A lista encadeada atende bem:
    //     inserir no fim e percorrer todos os elementos.
    //
    //   Repare que esta classe e quase igual a parte de "lista" da ListaCartas,
    //   mas guarda JogoMesa em vez de Carta. Mantivemos separadas para o codigo
    //   ficar mais claro e didatico (cada lista com o seu tipo).
    // ============================================================================
    public class ListaJogos
    {
        // Primeiro No da lista de jogos.
        private No<JogoMesa> inicio;

        // Quantidade de jogos baixados na mesa.
        private int quantidade;

        // Propriedade de leitura: quantos jogos existem na mesa.
        public int Quantidade
        {
            get { return quantidade; }
        }

        // Construtor: cria a mesa vazia (nenhum jogo baixado).
        public ListaJogos()
        {
            inicio = null;
            quantidade = 0;
        }

        // Retorna true se nao houver nenhum jogo na mesa.
        public bool EstaVazia()
        {
            return inicio == null;
        }

        // ------------------------------------------------------------------
        // Adicionar: coloca um novo jogo (sequencia baixada) no fim da lista.
        // ------------------------------------------------------------------
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

        // ------------------------------------------------------------------
        // ParaVetor: devolve um vetor com todos os jogos da mesa, para
        // percorre-los com facilidade (encaixar cartas, contar pontos, etc.).
        // ------------------------------------------------------------------
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
