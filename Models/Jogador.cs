using Buraco.Estruturas;

namespace Buraco.Models
{
    // ============================================================================
    // CLASSE: Jogador
    // ----------------------------------------------------------------------------
    // OBJETIVO DA CLASSE:
    //   Representar um jogador da partida, guardando tudo o que pertence a ele:
    //   o nome, as cartas na mao, os jogos baixados na mesa, a pontuacao e se
    //   ele ja pegou (comprou) o morto.
    // ============================================================================
    public class Jogador
    {
        // ATRIBUTO: nome do jogador (ex.: "Jogador 1").
        public string Nome { get; private set; }

        // ATRIBUTO: cartas que o jogador tem na mao.
        // Guardadas numa LISTA ENCADEADA manual (ListaCartas), pois a mao
        // muda muito (compra, baixa, descarta).
        public ListaCartas Mao { get; private set; }

        // ATRIBUTO: jogos que o jogador ja baixou na mesa (sequencias/canastras).
        // Guardados numa LISTA ENCADEADA manual de jogos (ListaJogos).
        // (Este atributo e um acrescimo ao minimo pedido; cada jogador tem a
        //  sua propria area de jogos na mesa.)
        public ListaJogos Jogos { get; private set; }

        // ATRIBUTO: pontuacao final apurada do jogador.
        public int Pontuacao { get; set; }

        // ATRIBUTO: indica se o jogador ja pegou o morto.
        // Importante porque so e possivel BATER depois de ter pegado o morto,
        // e quem nao pega o morto leva -100 de penalidade no final.
        public bool ComprouMorto { get; set; }

        // CONSTRUTOR: cria o jogador com o nome dado e estruturas vazias.
        public Jogador(string nome)
        {
            Nome = nome;
            Mao = new ListaCartas();
            Jogos = new ListaJogos();
            Pontuacao = 0;
            ComprouMorto = false;
        }
    }
}
