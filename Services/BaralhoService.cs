using System;
using Buraco.Models;

namespace Buraco.Services
{
    public static class BaralhoService
    {
        private static readonly Naipe[] NAIPES = new Naipe[]
        {
            Naipe.Copas, Naipe.Ouros, Naipe.Paus, Naipe.Espadas
        };

        public static Carta[] CriarBaralhoDuplo()
        {
            Carta[] baralho = new Carta[104];
            int indice = 0;

            for (int copia = 0; copia < 2; copia++)
            {
                for (int n = 0; n < NAIPES.Length; n++)
                {
                    for (int numero = 1; numero <= 13; numero++)
                    {
                        baralho[indice] = new Carta(numero, NAIPES[n]);
                        indice++;
                    }
                }
            }

            return baralho;
        }

        public static void Embaralhar(Carta[] baralho, Random sorteio)
        {
            for (int i = baralho.Length - 1; i > 0; i--)
            {
                int j = sorteio.Next(i + 1);
                Carta temp = baralho[i];
                baralho[i] = baralho[j];
                baralho[j] = temp;
            }
        }
    }
}
