namespace Buraco.Models
{
    // ============================================================================
    // CLASSE: Carta
    // ----------------------------------------------------------------------------
    // OBJETIVO DA CLASSE:
    //   Representar UMA carta do baralho. E a "menor peca" do jogo.
    //   Toda carta tem um numero e um naipe.
    //
    //   CONVENCOES ADOTADAS:
    //     Numero: 1 = As, 2 = curinga, 3..10 = numericas,
    //             11 = Valete (J), 12 = Dama (Q), 13 = Rei (K).
    //     O numero 2 e sempre tratado como CURINGA (pode substituir qualquer
    //     carta numa sequencia). Essa e uma simplificacao didatica para nao
    //     precisar tratar o "2 natural".
    // ============================================================================
    public class Carta
    {
        // ATRIBUTO: numero da carta (1 a 13, conforme a convencao acima).
        // "private set" -> so e definido no construtor; depois nao muda mais
        // (uma carta nao troca de numero durante o jogo).
        public int Numero { get; private set; }

        // ATRIBUTO: naipe da carta (Copas, Ouros, Paus ou Espadas).
        public Naipe Naipe { get; private set; }

        // CONSTRUTOR: cria a carta com numero e naipe informados.
        public Carta(int numero, Naipe naipe)
        {
            Numero = numero;
            Naipe = naipe;
        }

        // ------------------------------------------------------------------
        // PROPRIEDADE: EhCuringa
        // Indica se esta carta e um curinga. Pela regra adotada, todo 2 e curinga.
        // ------------------------------------------------------------------
        public bool EhCuringa
        {
            get { return Numero == 2; }
        }

        // ------------------------------------------------------------------
        // PROPRIEDADE: RankSequencia
        // Devolve o "valor de ordem" da carta dentro de uma sequencia.
        // Tratamos o As como a carta MAIS ALTA (rank 14), pois no Buraco a
        // sequencia mais valiosa termina no As (...Q, K, A).
        // (Nao chamamos esta propriedade para curingas, pois eles nao tem
        //  posicao fixa na sequencia.)
        // ------------------------------------------------------------------
        public int RankSequencia
        {
            get
            {
                if (Numero == 1)
                {
                    return 14; // As alto
                }
                return Numero;
            }
        }

        // ------------------------------------------------------------------
        // PROPRIEDADE: Valor
        // Quantos PONTOS a carta vale na contagem final.
        // Tabela adotada (comum no Buraco):
        //   As (1) e 2 (curinga) ...... 15 pontos
        //   3,4,5,6,7 ................. 5 pontos
        //   8,9,10,J,Q,K ............. 10 pontos
        // ------------------------------------------------------------------
        public int Valor
        {
            get
            {
                if (Numero == 1)
                {
                    return 15; // As
                }
                if (Numero == 2)
                {
                    return 15; // curinga
                }
                if (Numero >= 3 && Numero <= 7)
                {
                    return 5;
                }
                return 10; // 8,9,10,11(J),12(Q),13(K)
            }
        }

        // ------------------------------------------------------------------
        // PROPRIEDADE: NomeNumero
        // Devolve o texto do numero da carta (A, 2, ..., 10, J, Q, K).
        // ------------------------------------------------------------------
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

        // ------------------------------------------------------------------
        // PROPRIEDADE: SimboloNaipe
        // Devolve o simbolo do naipe para mostrar na tela.
        // ------------------------------------------------------------------
        public string SimboloNaipe
        {
            get
            {
                switch (Naipe)
                {
                    case Naipe.Copas: return "♥";   // copas
                    case Naipe.Ouros: return "♦";   // ouros
                    case Naipe.Paus: return "♣";    // paus
                    case Naipe.Espadas: return "♠"; // espadas
                    default: return "?";
                }
            }
        }

        // ------------------------------------------------------------------
        // ToString: como a carta aparece em texto.
        // Exemplos: "A♥", "K♠", "2♦(C)" (o (C) marca o curinga).
        // ------------------------------------------------------------------
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
