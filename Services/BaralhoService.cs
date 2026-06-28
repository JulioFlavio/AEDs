using System;
using Buraco.Models;

namespace Buraco.Services
{
    // ============================================================================
    // CLASSE: BaralhoService
    // ----------------------------------------------------------------------------
    // OBJETIVO DA CLASSE:
    //   Cuidar da CRIACAO e do EMBARALHAMENTO do baralho duplo (104 cartas).
    //   Os metodos sao "static" porque sao funcoes utilitarias: nao precisam
    //   guardar estado proprio, apenas recebem dados e devolvem um resultado.
    // ============================================================================
    public static class BaralhoService
    {
        // Os quatro naipes, em um vetor, para montar o baralho com um laco.
        private static readonly Naipe[] NAIPES = new Naipe[]
        {
            Naipe.Copas, Naipe.Ouros, Naipe.Paus, Naipe.Espadas
        };

        // ------------------------------------------------------------------
        // CriarBaralhoDuplo: monta o baralho com 104 cartas (2 baralhos de 52).
        // ------------------------------------------------------------------
        public static Carta[] CriarBaralhoDuplo()
        {
            // 2 baralhos x 4 naipes x 13 numeros = 104 cartas.
            Carta[] baralho = new Carta[104];
            int indice = 0;

            // Laco externo: repete duas vezes (dois baralhos identicos).
            for (int copia = 0; copia < 2; copia++)
            {
                // Para cada naipe...
                for (int n = 0; n < NAIPES.Length; n++)
                {
                    // ...cria as 13 cartas (numeros de 1 a 13).
                    for (int numero = 1; numero <= 13; numero++)
                    {
                        baralho[indice] = new Carta(numero, NAIPES[n]);
                        indice++;
                    }
                }
            }

            return baralho;
        }

        // ------------------------------------------------------------------
        // Embaralhar: embaralha o vetor de cartas usando o algoritmo de
        // Fisher-Yates (um metodo classico e justo de embaralhamento).
        //
        // IDEIA: do fim para o comeco, troca cada carta com uma posicao
        // sorteada entre 0 e o indice atual. Isso gera uma ordem aleatoria
        // em tempo O(n) (percorre o vetor uma unica vez).
        // ------------------------------------------------------------------
        public static void Embaralhar(Carta[] baralho, Random sorteio)
        {
            for (int i = baralho.Length - 1; i > 0; i--)
            {
                // Sorteia uma posicao j entre 0 e i (inclusive).
                int j = sorteio.Next(i + 1);

                // Troca as cartas das posicoes i e j.
                Carta temp = baralho[i];
                baralho[i] = baralho[j];
                baralho[j] = temp;
            }
        }
    }
}
