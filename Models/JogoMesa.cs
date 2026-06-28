using System.Text;
using Buraco.Estruturas;

namespace Buraco.Models
{
    public class JogoMesa
    {
        public Naipe Naipe { get; private set; }
        private ListaCartas cartas;

        public JogoMesa(Naipe naipe)
        {
            Naipe = naipe;
            cartas = new ListaCartas();
        }

        public int Quantidade
        {
            get { return cartas.Quantidade; }
        }

        public void AdicionarCarta(Carta carta)
        {
            cartas.AdicionarOrdenado(carta);
        }

        public Carta[] Cartas
        {
            get { return cartas.ParaVetor(); }
        }

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

        public bool ContemCuringa()
        {
            return QuantidadeCuringas() > 0;
        }

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

        public bool ContemAsNatural()
        {
            return ContemRank(14);
        }

        public bool EhCanastra()
        {
            return Quantidade >= 7;
        }

        public TipoCanastra Classificar()
        {
            if (!EhCanastra())
            {
                return TipoCanastra.NaoEhCanastra;
            }

            bool limpa = !ContemCuringa();
            bool comAs = ContemAsNatural();

            if (comAs && limpa)
            {
                return TipoCanastra.Real;
            }
            if (comAs && !limpa)
            {
                return TipoCanastra.MeiaReal;
            }
            if (limpa)
            {
                return TipoCanastra.Limpa;
            }
            return TipoCanastra.Suja;
        }

        public int PontosBonus()
        {
            switch (Classificar())
            {
                case TipoCanastra.Real: return 500;
                case TipoCanastra.MeiaReal: return 250;
                case TipoCanastra.Limpa: return 200;
                case TipoCanastra.Suja: return 100;
                default: return 0;
            }
        }

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
