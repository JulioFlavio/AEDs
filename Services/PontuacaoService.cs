using System.Text;
using Buraco.Models;

namespace Buraco.Services
{
    // ============================================================================
    // CLASSE: PontuacaoService
    // ----------------------------------------------------------------------------
    // OBJETIVO DA CLASSE:
    //   Calcular a pontuacao final de um jogador, somando bonus e descontando
    //   penalidades, de acordo com as regras adotadas.
    //
    //   FORMULA DA PONTUACAO DE UM JOGADOR:
    //       + bonus das canastras (200 limpa, 100 suja, 500 real, 250 meia real)
    //       + valor das cartas que estao baixadas na mesa
    //       - valor das cartas que sobraram na mao (penalidade)
    //       - 100 se NAO pegou o morto
    //       + 100 se foi quem BATEU
    //   = total de pontos do jogador
    // ============================================================================
    public static class PontuacaoService
    {
        // ------------------------------------------------------------------
        // Calcular: devolve o total de pontos do jogador (um numero inteiro).
        // "foiBatedor" indica se este jogador foi quem bateu.
        // ------------------------------------------------------------------
        public static int Calcular(Jogador jogador, bool foiBatedor)
        {
            int bonusCanastras = 0; // pontos de bonus das canastras
            int pontosCartasMesa = 0; // pontos das cartas baixadas

            // Percorre todos os jogos do jogador somando bonus e valor das cartas.
            JogoMesa[] jogos = jogador.Jogos.ParaVetor();
            for (int i = 0; i < jogos.Length; i++)
            {
                bonusCanastras += jogos[i].PontosBonus();
                pontosCartasMesa += jogos[i].PontosDasCartas();
            }

            // Penalidade: soma o valor das cartas que sobraram na mao.
            int penalidadeMao = 0;
            Carta[] mao = jogador.Mao.ParaVetor();
            for (int i = 0; i < mao.Length; i++)
            {
                penalidadeMao += mao[i].Valor;
            }

            // Penalidade por nao ter pegado o morto.
            int penalidadeMorto = jogador.ComprouMorto ? 0 : 100;

            // Bonus por ter batido.
            int bonusBatida = foiBatedor ? 100 : 0;

            // Total final.
            return bonusCanastras + pontosCartasMesa - penalidadeMao - penalidadeMorto + bonusBatida;
        }

        // ------------------------------------------------------------------
        // Detalhar: monta um texto explicando, parcela por parcela, como a
        // pontuacao do jogador foi calculada. Usado no resumo final na tela.
        // ------------------------------------------------------------------
        public static string Detalhar(Jogador jogador, bool foiBatedor)
        {
            int bonusCanastras = 0;
            int pontosCartasMesa = 0;
            JogoMesa[] jogos = jogador.Jogos.ParaVetor();
            for (int i = 0; i < jogos.Length; i++)
            {
                bonusCanastras += jogos[i].PontosBonus();
                pontosCartasMesa += jogos[i].PontosDasCartas();
            }

            int penalidadeMao = 0;
            Carta[] mao = jogador.Mao.ParaVetor();
            for (int i = 0; i < mao.Length; i++)
            {
                penalidadeMao += mao[i].Valor;
            }

            int penalidadeMorto = jogador.ComprouMorto ? 0 : 100;
            int bonusBatida = foiBatedor ? 100 : 0;
            int total = bonusCanastras + pontosCartasMesa - penalidadeMao - penalidadeMorto + bonusBatida;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("  Detalhamento da pontuacao:");
            sb.AppendLine("    (+) Bonus das canastras ....... " + bonusCanastras);
            sb.AppendLine("    (+) Cartas baixadas na mesa ... " + pontosCartasMesa);
            sb.AppendLine("    (-) Cartas na mao (penalidade)  " + penalidadeMao);
            sb.AppendLine("    (-) Nao pegou o morto ......... " + penalidadeMorto);
            sb.AppendLine("    (+) Bonus por bater ........... " + bonusBatida);
            sb.AppendLine("    ------------------------------------");
            sb.Append("    TOTAL ......................... " + total);
            return sb.ToString();
        }
    }
}
