using System;
using Buraco.Services;

namespace Buraco
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            bool sair = false;
            while (!sair)
            {
                Console.WriteLine();
                Console.WriteLine("============ BURACO - Trabalho de AED ============");
                Console.WriteLine(" 1 - Jogar uma partida (embaralhamento aleatorio)");
                Console.WriteLine(" 2 - Jogar uma partida (semente fixa - repetivel)");
                Console.WriteLine(" 3 - Mostrar as regras adotadas no trabalho");
                Console.WriteLine(" 0 - Sair");
                Console.WriteLine("==================================================");
                Console.Write("Escolha uma opcao: ");

                string opcao = Console.ReadLine();

                if (opcao == "1")
                {
                    int semente = new Random().Next();
                    JogarPartida(semente);
                }
                else if (opcao == "2")
                {
                    Console.Write("Digite a semente (numero inteiro): ");
                    int semente;
                    try
                    {
                        semente = int.Parse(Console.ReadLine());
                    }
                    catch (Exception)
                    {
                        semente = 42;
                        Console.WriteLine("Entrada invalida. Usando a semente padrao 42.");
                    }
                    JogarPartida(semente);
                }
                else if (opcao == "3")
                {
                    MostrarRegras();
                }
                else if (opcao == "0")
                {
                    sair = true;
                    Console.WriteLine("Encerrando. Ate a proxima!");
                }
                else
                {
                    Console.WriteLine("Opcao invalida. Tente novamente.");
                }
            }
        }

        static void JogarPartida(int semente)
        {
            Console.WriteLine();
            Console.WriteLine(">> Iniciando partida (semente = " + semente + ")...");

            PartidaService ps = new PartidaService(semente);
            ps.Iniciar();
            ps.Jogar();

            try
            {
                ps.Log.SalvarEmArquivo("log_partida.txt");
                Console.WriteLine(">> Log completo salvo no arquivo 'log_partida.txt'.");
            }
            catch (Exception e)
            {
                Console.WriteLine(">> Nao foi possivel salvar o log em arquivo: " + e.Message);
            }

            Console.WriteLine();
            Console.WriteLine("------------------- LOG DA PARTIDA -------------------");
            ps.Log.Imprimir();

            Console.WriteLine();
            ps.ImprimirResumoFinal();

            Console.WriteLine();
            Console.Write("Pressione ENTER para voltar ao menu...");
            Console.ReadLine();
        }

        static void MostrarRegras()
        {
            Console.WriteLine();
            Console.WriteLine("===== REGRAS ADOTADAS NESTE TRABALHO =====");
            Console.WriteLine("- Baralho duplo: 104 cartas (2 x 52), sem coringas extras.");
            Console.WriteLine("- O numero 2 e sempre tratado como CURINGA.");
            Console.WriteLine("- Distribuicao: 11 cartas por jogador, 2 mortos de 11, resto no monte.");
            Console.WriteLine("- Compra: 1 carta do MONTE ou o LIXO inteiro.");
            Console.WriteLine("- Jogos: sequencias do mesmo naipe (3+ cartas). As alto (...K, A).");
            Console.WriteLine("- Cada jogo aceita no maximo 1 curinga (0 = limpa, 1 = suja).");
            Console.WriteLine("- Canastra = 7+ cartas.");
            Console.WriteLine("    Suja .............. 7+ com curinga ............. +100");
            Console.WriteLine("    Limpa ............. 7+ sem curinga ............. +200");
            Console.WriteLine("    Meia Real ......... 7+ com curinga e com As .... +250");
            Console.WriteLine("    Real .............. 7+ sem curinga e com As .... +500");
            Console.WriteLine("- Morto: pego ao esvaziar a mao pela 1a vez. Quem nao pega: -100.");
            Console.WriteLine("- Batida: so com o morto ja pego E tendo ao menos 1 canastra. +100.");
            Console.WriteLine("- Valor das cartas: A e 2 = 15; 3..7 = 5; 8..K = 10.");
            Console.WriteLine("- Pontos = bonus de canastras + cartas na mesa - cartas na mao");
            Console.WriteLine("           - 100 (se nao pegou o morto) + 100 (se bateu).");
            Console.WriteLine("==========================================");
            Console.WriteLine();
            Console.Write("Pressione ENTER para voltar ao menu...");
            Console.ReadLine();
        }
    }
}
