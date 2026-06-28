using Buraco.Estruturas;

namespace Buraco.Models
{
    // ============================================================================
    // CLASSE: Partida
    // ----------------------------------------------------------------------------
    // OBJETIVO DA CLASSE:
    //   Guardar TODO o estado da partida em um unico lugar (os jogadores e os
    //   "montes" de cartas em jogo). E uma classe de DADOS: ela apenas armazena
    //   as informacoes; quem MOVIMENTA as cartas e aplica as regras e a classe
    //   PartidaService (na pasta Services).
    //
    //   Aqui ficam claras as estruturas de dados pedidas pelo professor:
    //     - Monte  -> PILHA (PilhaCartas)
    //     - Lixo   -> PILHA (PilhaCartas)
    //     - Morto  -> LISTA (ListaCartas)  [sao dois mortos no jogo de 2 pessoas]
    //     - Mao / Jogos do jogador -> LISTAS (dentro da classe Jogador)
    // ============================================================================
    public class Partida
    {
        // Os dois jogadores da partida (vetor de tamanho 2).
        public Jogador[] Jogadores;

        // MONTE: pilha de onde se compra (a carta de cima e a proxima a sair).
        public PilhaCartas Monte;

        // LIXO: pilha de descartes (a carta de cima pode ser comprada).
        public PilhaCartas Lixo;

        // MORTOS: no Buraco para 2 jogadores existem DOIS mortos de 11 cartas.
        // Cada um e um conjunto de cartas (lista). Quando um jogador esvazia a
        // mao pela primeira vez, ele "pega" um morto, que vira a nova mao dele.
        public ListaCartas MortoA;
        public ListaCartas MortoB;

        // Controla se cada morto ainda esta disponivel para ser pego.
        public bool MortoADisponivel;
        public bool MortoBDisponivel;

        // Indice (0 ou 1) do jogador que esta jogando no momento.
        public int JogadorDaVez;

        // Numero da rodada atual (uma rodada = os dois jogadores jogarem uma vez).
        public int NumeroRodada;

        // Indica se a partida ja terminou.
        public bool Encerrada;

        // Quem BATEU (encerrou a partida ao zerar a mao). Pode ser null se a
        // partida terminou por outro motivo (ex.: o monte acabou).
        public Jogador Batedor;

        // Vencedor apurado no final. Pode ser null em caso de empate.
        public Jogador Vencedor;

        // CONSTRUTOR: cria a partida "crua". Quem preenche tudo (distribui as
        // cartas, etc.) e o metodo Iniciar() da classe PartidaService.
        public Partida()
        {
            Encerrada = false;
            Batedor = null;
            Vencedor = null;
        }
    }
}
