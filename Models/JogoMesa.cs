using System.Text;
using Buraco.Estruturas;

namespace Buraco.Models
{
    // ============================================================================
    // CLASSE: JogoMesa
    // ----------------------------------------------------------------------------
    // OBJETIVO DA CLASSE:
    //   Representar UM jogo baixado na mesa, ou seja, uma SEQUENCIA de cartas
    //   do mesmo naipe (ex.: 5♥ 6♥ 7♥). Quando a sequencia chega a 7 ou mais
    //   cartas, ela vira uma CANASTRA.
    //
    //   As cartas do jogo sao guardadas numa LISTA ENCADEADA (ListaCartas),
    //   mantida em ordem para facilitar a leitura.
    //
    //   REGRAS ADOTADAS PARA UM JOGO:
    //     - Todas as cartas naturais sao do MESMO naipe.
    //     - As cartas formam uma sequencia de numeros consecutivos.
    //     - O 2 (curinga) pode entrar como "substituto", mas neste trabalho
    //       cada jogo aceita NO MAXIMO 1 curinga (simplificacao didatica:
    //       0 curinga = limpa; 1 curinga = suja).
    // ============================================================================
    public class JogoMesa
    {
        // ATRIBUTO: naipe ao qual o jogo pertence (todas as naturais sao dele).
        public Naipe Naipe { get; private set; }

        // ATRIBUTO: as cartas do jogo, guardadas numa lista encadeada manual
        // e mantidas em ordem (AdicionarOrdenado).
        private ListaCartas cartas;

        // CONSTRUTOR: cria um jogo vazio de um determinado naipe.
        public JogoMesa(Naipe naipe)
        {
            Naipe = naipe;
            cartas = new ListaCartas();
        }

        // Quantidade de cartas no jogo.
        public int Quantidade
        {
            get { return cartas.Quantidade; }
        }

        // Adiciona uma carta ao jogo, mantendo a ordem.
        public void AdicionarCarta(Carta carta)
        {
            cartas.AdicionarOrdenado(carta);
        }

        // Devolve um vetor com as cartas do jogo (para percorrer/contar/exibir).
        public Carta[] Cartas
        {
            get { return cartas.ParaVetor(); }
        }

        // ------------------------------------------------------------------
        // QuantidadeCuringas: conta quantos curingas (cartas 2) estao no jogo.
        // ------------------------------------------------------------------
        public int QuantidadeCuringas()
        {
            int total = 0;
            Carta[] v = Cartas;
            for (int i = 0; i < v.Length; i++)
            {
                if (v[i].EhCuringa)
                {
                    total++;
                }
            }
            return total;
        }

        // Indica se o jogo possui pelo menos um curinga.
        public bool ContemCuringa()
        {
            return QuantidadeCuringas() > 0;
        }

        // ------------------------------------------------------------------
        // ContemRank: verifica se o jogo ja possui uma carta NATURAL com um
        // determinado rank (numero de ordem). Evita repetir numeros na sequencia.
        // ------------------------------------------------------------------
        public bool ContemRank(int rank)
        {
            Carta[] v = Cartas;
            for (int i = 0; i < v.Length; i++)
            {
                if (!v[i].EhCuringa && v[i].RankSequencia == rank)
                {
                    return true;
                }
            }
            return false;
        }

        // ------------------------------------------------------------------
        // MinNatural / MaxNatural: menor e maior rank entre as cartas NATURAIS.
        // Servem para saber as "pontas" da sequencia (onde da para encaixar
        // novas cartas).
        // ------------------------------------------------------------------
        public int MinNatural()
        {
            int min = 99;
            Carta[] v = Cartas;
            for (int i = 0; i < v.Length; i++)
            {
                if (!v[i].EhCuringa && v[i].RankSequencia < min)
                {
                    min = v[i].RankSequencia;
                }
            }
            // Se nao houver carta natural (nao deve acontecer), devolve 0.
            return (min == 99) ? 0 : min;
        }

        public int MaxNatural()
        {
            int max = 0;
            Carta[] v = Cartas;
            for (int i = 0; i < v.Length; i++)
            {
                if (!v[i].EhCuringa && v[i].RankSequencia > max)
                {
                    max = v[i].RankSequencia;
                }
            }
            return max;
        }

        // Indica se o jogo contem o As natural (rank 14) -> usado para "real".
        public bool ContemAsNatural()
        {
            return ContemRank(14);
        }

        // Indica se o jogo ja e uma CANASTRA (7 ou mais cartas).
        public bool EhCanastra()
        {
            return Quantidade >= 7;
        }

        // ------------------------------------------------------------------
        // Classificar: decide o TIPO de canastra (ou que ainda nao e canastra).
        // Segue as regras adotadas (ver enum TipoCanastra).
        // ------------------------------------------------------------------
        public TipoCanastra Classificar()
        {
            if (!EhCanastra())
            {
                return TipoCanastra.NaoEhCanastra;
            }

            bool limpa = !ContemCuringa();   // sem curinga = limpa
            bool comAs = ContemAsNatural();  // chega ao As = candidata a "real"

            if (comAs && limpa)
            {
                return TipoCanastra.Real;      // As + sem curinga
            }
            if (comAs && !limpa)
            {
                return TipoCanastra.MeiaReal;  // As + com curinga
            }
            if (limpa)
            {
                return TipoCanastra.Limpa;     // sem curinga
            }
            return TipoCanastra.Suja;          // com curinga
        }

        // ------------------------------------------------------------------
        // PontosBonus: pontos de BONUS que a canastra vale (alem das cartas).
        // ------------------------------------------------------------------
        public int PontosBonus()
        {
            switch (Classificar())
            {
                case TipoCanastra.Real: return 500;
                case TipoCanastra.MeiaReal: return 250;
                case TipoCanastra.Limpa: return 200;
                case TipoCanastra.Suja: return 100;
                default: return 0; // ainda nao e canastra
            }
        }

        // ------------------------------------------------------------------
        // PontosDasCartas: soma o valor (em pontos) de cada carta do jogo.
        // ------------------------------------------------------------------
        public int PontosDasCartas()
        {
            int soma = 0;
            Carta[] v = Cartas;
            for (int i = 0; i < v.Length; i++)
            {
                soma += v[i].Valor;
            }
            return soma;
        }

        // ------------------------------------------------------------------
        // NomeClassificacao: texto amigavel do tipo do jogo, para o relatorio.
        // ------------------------------------------------------------------
        public string NomeClassificacao()
        {
            switch (Classificar())
            {
                case TipoCanastra.Real: return "CANASTRA REAL";
                case TipoCanastra.MeiaReal: return "MEIA CANASTRA REAL";
                case TipoCanastra.Limpa: return "CANASTRA LIMPA";
                case TipoCanastra.Suja: return "CANASTRA SUJA";
                default: return "sequencia (" + Quantidade + " cartas)";
            }
        }

        // ------------------------------------------------------------------
        // ToString: mostra o jogo em texto, com as cartas e a classificacao.
        // Exemplo: "5♥ 6♥ 7♥ 8♥ 9♥ 10♥ J♥  -> CANASTRA LIMPA (+200)"
        // ------------------------------------------------------------------
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Carta[] v = Cartas;
            for (int i = 0; i < v.Length; i++)
            {
                sb.Append(v[i].ToString());
                sb.Append(" ");
            }
            sb.Append(" -> ");
            sb.Append(NomeClassificacao());
            if (EhCanastra())
            {
                sb.Append(" (+" + PontosBonus() + ")");
            }
            return sb.ToString();
        }
    }
}
