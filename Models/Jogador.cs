using Buraco.Estruturas;

namespace Buraco.Models
{
    public class Jogador
    {
        public string Nome { get; private set; }
        public ListaCartas Mao { get; private set; }
        public ListaJogos Jogos { get; private set; }
        public int Pontuacao { get; set; }
        public bool ComprouMorto { get; set; }

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
