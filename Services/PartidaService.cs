using System;
using Buraco.Models;
using Buraco.Estruturas;

namespace Buraco.Services
{
    public class PartidaService
    {
        private Partida partida;
        private Random rnd;
        private LogService log;

        private readonly Naipe[] NAIPES = new Naipe[]
        {
            Naipe.Copas, Naipe.Ouros, Naipe.Paus, Naipe.Espadas
        };

        private const int MAX_TURNOS = 4000;

        public LogService Log { get { return log; } }
        public Partida Partida { get { return partida; } }

        public PartidaService(int semente)
        {
            rnd = new Random(semente);
            log = new LogService();
        }

        public void Iniciar()
        {
            partida = new Partida();
            partida.Jogadores = new Jogador[2];
            partida.Jogadores[0] = new Jogador("Jogador 1");
            partida.Jogadores[1] = new Jogador("Jogador 2");
            partida.Monte = new PilhaCartas();
            partida.Lixo = new PilhaCartas();
            partida.MortoA = new ListaCartas();
            partida.MortoB = new ListaCartas();
            partida.MortoADisponivel = true;
            partida.MortoBDisponivel = true;
            partida.JogadorDaVez = 0;
            partida.NumeroRodada = 1;
            partida.Encerrada = false;

            Carta[] baralho = BaralhoService.CriarBaralhoDuplo();
            BaralhoService.Embaralhar(baralho, rnd);
            log.Registrar("Baralho duplo criado e embaralhado: " + baralho.Length + " cartas.");

            int i = 0;

            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < 11; k++)
                {
                    partida.Jogadores[j].Mao.Adicionar(baralho[i]);
                    i++;
                }
            }

            for (int k = 0; k < 11; k++)
            {
                partida.MortoA.Adicionar(baralho[i]);
                i++;
            }
            for (int k = 0; k < 11; k++)
            {
                partida.MortoB.Adicionar(baralho[i]);
                i++;
            }

            while (i < baralho.Length)
            {
                partida.Monte.Empilhar(baralho[i]);
                i++;
            }

            log.Registrar("Distribuicao: 11 cartas para cada jogador, 2 mortos de 11 cartas e "
                          + partida.Monte.Quantidade + " cartas no monte.");
            log.Registrar("O lixo comeca vazio.");
            log.Registrar("=====================================================");
        }

        public void Jogar()
        {
            int turnos = 0;

            while (!partida.Encerrada && turnos < MAX_TURNOS)
            {
                Jogador jog = partida.Jogadores[partida.JogadorDaVez];
                ExecutarTurno(jog);
                turnos++;

                if (partida.Encerrada)
                {
                    break;
                }

                partida.JogadorDaVez = (partida.JogadorDaVez + 1) % 2;

                if (partida.JogadorDaVez == 0)
                {
                    partida.NumeroRodada++;
                }
            }

            if (!partida.Encerrada)
            {
                log.Registrar("Limite de turnos atingido. Encerrando a partida por seguranca.");
                partida.Encerrada = true;
            }

            Apurar();
        }

        private void ExecutarTurno(Jogador jog)
        {
            log.Registrar("");
            log.Registrar("Rodada " + partida.NumeroRodada + " - vez de " + jog.Nome
                          + " (mao: " + jog.Mao.Quantidade + " cartas).");

            Comprar(jog);
            if (partida.Encerrada)
            {
                return;
            }

            FaseDeJogos(jog);

            if (jog.Mao.EstaVazia() && jog.ComprouMorto && TemCanastra(jog))
            {
                RegistrarBatida(jog, false);
                return;
            }

            Descartar(jog);

            if (jog.Mao.EstaVazia() && !jog.ComprouMorto && HaMortoDisponivel())
            {
                PegarMorto(jog);
                FaseDeJogos(jog);

                if (jog.Mao.EstaVazia() && jog.ComprouMorto && TemCanastra(jog))
                {
                    RegistrarBatida(jog, false);
                    return;
                }
                if (!jog.Mao.EstaVazia())
                {
                    Descartar(jog);
                }
            }

            if (jog.Mao.EstaVazia() && jog.ComprouMorto && TemCanastra(jog))
            {
                RegistrarBatida(jog, true);
                return;
            }
        }

        private void Comprar(Jogador jog)
        {
            if (!partida.Lixo.EstaVazia() && DeveComprarDoLixo(jog))
            {
                int n = partida.Lixo.Quantidade;
                while (!partida.Lixo.EstaVazia())
                {
                    jog.Mao.Adicionar(partida.Lixo.Desempilhar());
                }
                log.Registrar("  " + jog.Nome + " comprou o LIXO inteiro (" + n + " carta(s)).");
                return;
            }

            if (partida.Monte.EstaVazia())
            {
                EncerrarPorMonteVazio();
                return;
            }

            Carta c = partida.Monte.Desempilhar();
            jog.Mao.Adicionar(c);
            log.Registrar("  " + jog.Nome + " comprou do MONTE: " + c.ToString()
                          + " (restam " + partida.Monte.Quantidade + " no monte).");
        }

        private bool DeveComprarDoLixo(Jogador jog)
        {
            Carta topo = partida.Lixo.VerTopo();

            if (topo.EhCuringa)
            {
                return true;
            }

            JogoMesa[] jogos = jog.Jogos.ParaVetor();
            for (int i = 0; i < jogos.Length; i++)
            {
                if (PodeAdicionarAoJogo(jogos[i], topo))
                {
                    return true;
                }
            }

            if (ContaVizinhosMesmoNaipe(jog, topo) >= 2)
            {
                return true;
            }

            return false;
        }

        private int ContaVizinhosMesmoNaipe(Jogador jog, Carta carta)
        {
            if (carta.EhCuringa)
            {
                return 0;
            }

            int conta = 0;
            int r = carta.RankSequencia;
            Carta[] mao = jog.Mao.ParaVetor();
            for (int i = 0; i < mao.Length; i++)
            {
                Carta c = mao[i];
                if (!c.EhCuringa && c.Naipe == carta.Naipe)
                {
                    int diff = c.RankSequencia - r;
                    if (diff < 0) diff = -diff;
                    if (diff >= 1 && diff <= 2)
                    {
                        conta++;
                    }
                }
            }
            return conta;
        }

        private void FaseDeJogos(Jogador jog)
        {
            bool houveMudanca = true;
            while (houveMudanca)
            {
                houveMudanca = false;

                if (TentarEncaixar(jog)) houveMudanca = true;
                if (CriarNovosJogos(jog)) houveMudanca = true;
                if (FecharCanastraComCuringa(jog)) houveMudanca = true;

                if (jog.Mao.EstaVazia() && !jog.ComprouMorto && HaMortoDisponivel())
                {
                    PegarMorto(jog);
                    houveMudanca = true;
                }
            }
        }

        private bool TentarEncaixar(Jogador jog)
        {
            bool encaixouAlgo = false;
            bool achouNestaVolta = true;

            while (achouNestaVolta)
            {
                achouNestaVolta = false;
                JogoMesa[] jogos = jog.Jogos.ParaVetor();
                Carta[] mao = jog.Mao.ParaVetor();

                for (int j = 0; j < jogos.Length && !achouNestaVolta; j++)
                {
                    for (int k = 0; k < mao.Length && !achouNestaVolta; k++)
                    {
                        if (PodeAdicionarAoJogo(jogos[j], mao[k])
                            && PodeReduzirPara(jog, jog.Mao.Quantidade - 1))
                        {
                            jog.Mao.Remover(mao[k]);
                            jogos[j].AdicionarCarta(mao[k]);
                            log.Registrar("  " + jog.Nome + " encaixou " + mao[k].ToString()
                                          + " em um jogo de " + jogos[j].Naipe
                                          + " (agora com " + jogos[j].Quantidade + " cartas).");
                            encaixouAlgo = true;
                            achouNestaVolta = true;
                        }
                    }
                }
            }

            return encaixouAlgo;
        }

        private bool CriarNovosJogos(Jogador jog)
        {
            bool criouAlgo = false;

            for (int n = 0; n < NAIPES.Length; n++)
            {
                Naipe naipe = NAIPES[n];

                Carta[] porRank = new Carta[15];
                Carta[] mao = jog.Mao.ParaVetor();
                for (int i = 0; i < mao.Length; i++)
                {
                    Carta c = mao[i];
                    if (!c.EhCuringa && c.Naipe == naipe)
                    {
                        int r = c.RankSequencia;
                        if (porRank[r] == null)
                        {
                            porRank[r] = c;
                        }
                    }
                }

                int rank = 3;
                while (rank <= 14)
                {
                    if (porRank[rank] != null)
                    {
                        int inicioSeq = rank;
                        while (rank + 1 <= 14 && porRank[rank + 1] != null)
                        {
                            rank++;
                        }
                        int fimSeq = rank;
                        int tamanho = fimSeq - inicioSeq + 1;

                        if (tamanho >= 3 && PodeReduzirPara(jog, jog.Mao.Quantidade - tamanho))
                        {
                            JogoMesa novo = new JogoMesa(naipe);
                            for (int x = inicioSeq; x <= fimSeq; x++)
                            {
                                jog.Mao.Remover(porRank[x]);
                                novo.AdicionarCarta(porRank[x]);
                            }
                            jog.Jogos.Adicionar(novo);
                            log.Registrar("  " + jog.Nome + " baixou um novo jogo de " + naipe
                                          + " com " + tamanho + " cartas: " + novo.ToString());
                            criouAlgo = true;
                        }

                        rank = fimSeq + 1;
                    }
                    else
                    {
                        rank++;
                    }
                }
            }

            return criouAlgo;
        }

        private bool FecharCanastraComCuringa(Jogador jog)
        {
            Carta curinga = null;
            Carta[] mao = jog.Mao.ParaVetor();
            for (int i = 0; i < mao.Length; i++)
            {
                if (mao[i].EhCuringa)
                {
                    curinga = mao[i];
                    break;
                }
            }
            if (curinga == null)
            {
                return false;
            }

            if (!PodeReduzirPara(jog, jog.Mao.Quantidade - 1))
            {
                return false;
            }

            JogoMesa[] jogos = jog.Jogos.ParaVetor();
            for (int j = 0; j < jogos.Length; j++)
            {
                JogoMesa m = jogos[j];
                if (m.Quantidade == 6 && !m.ContemCuringa() && PodeAdicionarAoJogo(m, curinga))
                {
                    jog.Mao.Remover(curinga);
                    m.AdicionarCarta(curinga);
                    log.Registrar("  " + jog.Nome + " usou um curinga e FECHOU uma canastra de "
                                  + m.Naipe + " (" + m.Quantidade + " cartas).");
                    return true;
                }
            }

            return false;
        }

        private bool PodeAdicionarAoJogo(JogoMesa m, Carta c)
        {
            if (c.EhCuringa)
            {
                if (m.ContemCuringa())
                {
                    return false;
                }
                if (m.Quantidade < 2)
                {
                    return false;
                }
                if (m.MaxNatural() < 14)
                {
                    return true;
                }
                if (m.MinNatural() > 3)
                {
                    return true;
                }
                return false;
            }

            if (c.Naipe != m.Naipe)
            {
                return false;
            }

            int r = c.RankSequencia;

            if (m.ContemRank(r))
            {
                return false;
            }

            int lo = m.MinNatural();
            int hi = m.MaxNatural();

            if (r == hi + 1 && r <= 14)
            {
                return true;
            }
            if (r == lo - 1 && r >= 3)
            {
                return true;
            }

            return false;
        }

        private bool PodeReduzirPara(Jogador jog, int resultante)
        {
            if (jog.ComprouMorto)
            {
                if (TemCanastra(jog))
                {
                    return resultante >= 0;
                }
                else
                {
                    return resultante >= 2;
                }
            }
            else
            {
                if (resultante >= 1)
                {
                    return true;
                }
                return HaMortoDisponivel();
            }
        }

        private void Descartar(Jogador jog)
        {
            Carta[] mao = jog.Mao.ParaVetor();
            if (mao.Length == 0)
            {
                return;
            }

            Carta escolhida = EscolherDescarte(jog, mao);
            jog.Mao.Remover(escolhida);
            partida.Lixo.Empilhar(escolhida);
            log.Registrar("  " + jog.Nome + " descartou " + escolhida.ToString()
                          + " no lixo (mao: " + jog.Mao.Quantidade + " cartas).");
        }

        private Carta EscolherDescarte(Jogador jog, Carta[] mao)
        {
            Carta melhor = null;
            int melhorValor = -1;

            for (int i = 0; i < mao.Length; i++)
            {
                Carta c = mao[i];
                if (c.EhCuringa)
                {
                    continue;
                }
                if (EhIsolada(jog, c) && c.Valor > melhorValor)
                {
                    melhor = c;
                    melhorValor = c.Valor;
                }
            }
            if (melhor != null)
            {
                return melhor;
            }

            for (int i = 0; i < mao.Length; i++)
            {
                Carta c = mao[i];
                if (c.EhCuringa)
                {
                    continue;
                }
                if (c.Valor > melhorValor)
                {
                    melhor = c;
                    melhorValor = c.Valor;
                }
            }
            if (melhor != null)
            {
                return melhor;
            }

            return mao[0];
        }

        private bool EhIsolada(Jogador jog, Carta carta)
        {
            Carta[] mao = jog.Mao.ParaVetor();
            for (int i = 0; i < mao.Length; i++)
            {
                Carta o = mao[i];
                if (object.ReferenceEquals(o, carta))
                {
                    continue;
                }
                if (!o.EhCuringa && o.Naipe == carta.Naipe
                    && (o.RankSequencia == carta.RankSequencia - 1
                        || o.RankSequencia == carta.RankSequencia + 1))
                {
                    return false;
                }
            }
            return true;
        }

        private bool HaMortoDisponivel()
        {
            return partida.MortoADisponivel || partida.MortoBDisponivel;
        }

        private void PegarMorto(Jogador jog)
        {
            ListaCartas morto;
            string nome;

            if (partida.MortoADisponivel)
            {
                morto = partida.MortoA;
                partida.MortoADisponivel = false;
                nome = "Morto A";
            }
            else
            {
                morto = partida.MortoB;
                partida.MortoBDisponivel = false;
                nome = "Morto B";
            }

            int qtd = morto.Quantidade;
            Carta[] cartas = morto.ParaVetor();
            for (int i = 0; i < cartas.Length; i++)
            {
                jog.Mao.Adicionar(cartas[i]);
            }

            jog.ComprouMorto = true;
            log.Registrar("  " + jog.Nome + " esvaziou a mao e PEGOU O " + nome
                          + " (" + qtd + " cartas).");
        }

        private bool TemCanastra(Jogador jog)
        {
            JogoMesa[] jogos = jog.Jogos.ParaVetor();
            for (int i = 0; i < jogos.Length; i++)
            {
                if (jogos[i].EhCanastra())
                {
                    return true;
                }
            }
            return false;
        }

        private void RegistrarBatida(Jogador jog, bool comDescarte)
        {
            partida.Batedor = jog;
            partida.Encerrada = true;
            string tipo = comDescarte ? " (com descarte)" : " (sem descarte)";
            log.Registrar("  *** " + jog.Nome + " BATEU" + tipo + "! A partida foi encerrada. ***");
        }

        private void EncerrarPorMonteVazio()
        {
            partida.Encerrada = true;
            log.Registrar("  O MONTE acabou. A partida e encerrada (ninguem bateu).");
        }

        private void Apurar()
        {
            log.Registrar("");
            log.Registrar("===================== APURACAO ======================");

            for (int i = 0; i < partida.Jogadores.Length; i++)
            {
                Jogador jog = partida.Jogadores[i];
                bool foiBatedor = object.ReferenceEquals(jog, partida.Batedor);
                int pts = PontuacaoService.Calcular(jog, foiBatedor);
                jog.Pontuacao = pts;
                log.Registrar(jog.Nome + " totalizou " + pts + " ponto(s).");
            }

            Jogador a = partida.Jogadores[0];
            Jogador b = partida.Jogadores[1];

            if (a.Pontuacao > b.Pontuacao)
            {
                partida.Vencedor = a;
            }
            else if (b.Pontuacao > a.Pontuacao)
            {
                partida.Vencedor = b;
            }
            else
            {
                partida.Vencedor = null;
            }

            if (partida.Vencedor == null)
            {
                log.Registrar("Resultado: EMPATE.");
            }
            else
            {
                log.Registrar("Vencedor: " + partida.Vencedor.Nome + "!");
            }
            log.Registrar("=====================================================");
        }

        public void ImprimirResumoFinal()
        {
            Console.WriteLine("==================== RESULTADO FINAL ====================");

            for (int i = 0; i < partida.Jogadores.Length; i++)
            {
                Jogador jog = partida.Jogadores[i];
                Console.WriteLine();
                Console.WriteLine(jog.Nome + "  ("
                                  + (jog.ComprouMorto ? "pegou o morto" : "NAO pegou o morto") + ")");
                Console.WriteLine("  Jogos baixados na mesa:");

                JogoMesa[] jogos = jog.Jogos.ParaVetor();
                if (jogos.Length == 0)
                {
                    Console.WriteLine("    (nenhum)");
                }
                else
                {
                    for (int j = 0; j < jogos.Length; j++)
                    {
                        Console.WriteLine("    " + jogos[j].ToString());
                    }
                }

                Console.WriteLine("  Cartas que sobraram na mao: " + jog.Mao.Quantidade);

                bool foiBatedor = object.ReferenceEquals(jog, partida.Batedor);
                Console.WriteLine(PontuacaoService.Detalhar(jog, foiBatedor));
            }

            Console.WriteLine();
            if (partida.Vencedor == null)
            {
                Console.WriteLine(">>> RESULTADO: EMPATE <<<");
            }
            else
            {
                Console.WriteLine(">>> VENCEDOR: " + partida.Vencedor.Nome
                                  + " com " + partida.Vencedor.Pontuacao + " pontos <<<");
            }
            Console.WriteLine("=========================================================");
        }
    }
}
