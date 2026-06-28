using Buraco.Estruturas;

namespace Buraco.Models
{
    public class Partida
    {
        public Jogador[] Jogadores;
        public PilhaCartas Monte;
        public PilhaCartas Lixo;
        public ListaCartas MortoA;
        public ListaCartas MortoB;
        public bool MortoADisponivel;
        public bool MortoBDisponivel;
        public int JogadorDaVez;
        public int NumeroRodada;
        public bool Encerrada;
        public Jogador Batedor;
        public Jogador Vencedor;

        public Partida()
        {
            Encerrada = false;
            Batedor = null;
            Vencedor = null;
        }
    }
}
