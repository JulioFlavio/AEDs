using System.Text;
using Buraco.Models;

namespace Buraco.Services
{
    public static class PontuacaoService
    {
        public static int Calcular(Jogador jogador, bool foiBatedor)
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

            return bonusCanastras + pontosCartasMesa - penalidadeMao - penalidadeMorto + bonusBatida;
        }

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
