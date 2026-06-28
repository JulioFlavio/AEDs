namespace Buraco.Models
{
    public class Carta
    {
        public int Numero { get; private set; }
        public Naipe Naipe { get; private set; }

        public Carta(int numero, Naipe naipe)
        {
            Numero = numero;
            Naipe = naipe;
        }

        public bool EhCuringa
        {
            get { return Numero == 2; }
        }

        public int RankSequencia
        {
            get
            {
                if (Numero == 1)
                {
                    return 14;
                }
                return Numero;
            }
        }

        public int Valor
        {
            get
            {
                if (Numero == 1)
                {
                    return 15;
                }
                if (Numero == 2)
                {
                    return 15;
                }
                if (Numero >= 3 && Numero <= 7)
                {
                    return 5;
                }
                return 10;
            }
        }

        public string NomeNumero
        {
            get
            {
                switch (Numero)
                {
                    case 1: return "A";
                    case 11: return "J";
                    case 12: return "Q";
                    case 13: return "K";
                    default: return Numero.ToString();
                }
            }
        }

        public string SimboloNaipe
        {
            get
            {
                switch (Naipe)
                {
                    case Naipe.Copas: return "♥";
                    case Naipe.Ouros: return "♦";
                    case Naipe.Paus: return "♣";
                    case Naipe.Espadas: return "♠";
                    default: return "?";
                }
            }
        }

        public override string ToString()
        {
            string texto = NomeNumero + SimboloNaipe;
            if (EhCuringa)
            {
                texto = texto + "(C)";
            }
            return texto;
        }
    }
}
