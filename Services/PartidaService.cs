using System;
using Buraco.Models;
using Buraco.Estruturas;

namespace Buraco.Services
{
    // ============================================================================
    // CLASSE: PartidaService  (o "cerebro" / controle da partida)
    // ----------------------------------------------------------------------------
    // OBJETIVO DA CLASSE:
    //   Controlar TODO o andamento da partida de Buraco: distribuir as cartas,
    //   conduzir os turnos (comprar, baixar/encaixar jogos, descartar), tratar
    //   o morto, detectar a batida e, no fim, apurar a pontuacao.
    //
    //   Esta versao do trabalho roda em MODO AUTOMATICO: o proprio programa
    //   joga pelos dois jogadores, usando regras simples (heuristicas) de
    //   decisao. Assim a partida sempre roda do inicio ao fim, gerando um log
    //   completo - o que e otimo para apresentar e demonstrar as estruturas.
    //
    //   IMPORTANTE: a "inteligencia" dos jogadores e proposital e simples
    //   (academica). O foco do trabalho sao as ESTRUTURAS DE DADOS, e nao uma
    //   IA otima de Buraco.
    // ============================================================================
    public class PartidaService
    {
        // Estado da partida (jogadores, monte, lixo, mortos...).
        private Partida partida;

        // Gerador de numeros aleatorios (para embaralhar). Recebe uma "semente"
        // para que a partida possa ser repetida igualzinha, se desejado.
        private Random rnd;

        // Servico de log: registra cada acontecimento da partida.
        private LogService log;

        // Os quatro naipes, usados em varios lacos.
        private readonly Naipe[] NAIPES = new Naipe[]
        {
            Naipe.Copas, Naipe.Ouros, Naipe.Paus, Naipe.Espadas
        };

        // Limite de seguranca de turnos, para garantir que a partida sempre
        // termina (evita um possivel laco infinito em casos raros).
        private const int MAX_TURNOS = 4000;

        // Permite ao Program acessar o log e o resultado da partida.
        public LogService Log { get { return log; } }
        public Partida Partida { get { return partida; } }

        // CONSTRUTOR: recebe a semente do embaralhamento.
        public PartidaService(int semente)
        {
            rnd = new Random(semente);
            log = new LogService();
        }

        // ====================================================================
        // 1) PREPARACAO DA PARTIDA
        // ====================================================================
        public void Iniciar()
        {
            // Cria o estado inicial da partida.
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

            // Cria e embaralha o baralho duplo.
            Carta[] baralho = BaralhoService.CriarBaralhoDuplo();
            BaralhoService.Embaralhar(baralho, rnd);
            log.Registrar("Baralho duplo criado e embaralhado: " + baralho.Length + " cartas.");

            // DISTRIBUICAO:
            //  - 11 cartas para cada jogador
            //  - 2 mortos de 11 cartas cada
            //  - o restante forma o monte
            int idx = 0;

            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < 11; k++)
                {
                    partida.Jogadores[j].Mao.Adicionar(baralho[idx]);
                    idx++;
                }
            }

            for (int k = 0; k < 11; k++)
            {
                partida.MortoA.Adicionar(baralho[idx]);
                idx++;
            }
            for (int k = 0; k < 11; k++)
            {
                partida.MortoB.Adicionar(baralho[idx]);
                idx++;
            }

            // Todas as cartas que sobraram vao para o MONTE (pilha).
            while (idx < baralho.Length)
            {
                partida.Monte.Empilhar(baralho[idx]);
                idx++;
            }

            log.Registrar("Distribuicao: 11 cartas para cada jogador, 2 mortos de 11 cartas e "
                          + partida.Monte.Quantidade + " cartas no monte.");
            log.Registrar("O lixo comeca vazio.");
            log.Registrar("=====================================================");
        }

        // ====================================================================
        // 2) LOOP PRINCIPAL DA PARTIDA
        // ====================================================================
        public void Jogar()
        {
            int turnos = 0;

            // Joga turno apos turno ate alguem bater, o monte acabar ou bater
            // o limite de seguranca.
            while (!partida.Encerrada && turnos < MAX_TURNOS)
            {
                Jogador jog = partida.Jogadores[partida.JogadorDaVez];
                ExecutarTurno(jog);
                turnos++;

                if (partida.Encerrada)
                {
                    break;
                }

                // Passa a vez para o outro jogador.
                partida.JogadorDaVez = (partida.JogadorDaVez + 1) % 2;

                // Se voltou ao jogador 0, comecou uma nova rodada.
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

            // Calcula a pontuacao e descobre o vencedor.
            Apurar();
        }

        // ====================================================================
        // 3) UM TURNO DE UM JOGADOR
        // ====================================================================
        private void ExecutarTurno(Jogador jog)
        {
            log.Registrar("");
            log.Registrar("Rodada " + partida.NumeroRodada + " - vez de " + jog.Nome
                          + " (mao: " + jog.Mao.Quantidade + " cartas).");

            // 3.1) COMPRA: do monte ou do lixo.
            Comprar(jog);
            if (partida.Encerrada)
            {
                return; // o monte acabou durante a compra
            }

            // 3.2) FASE DE JOGOS: baixar novas sequencias e encaixar cartas.
            FaseDeJogos(jog);

            // 3.3) BATIDA SEM DESCARTE: se zerou a mao de forma legal.
            if (jog.Mao.EstaVazia() && jog.ComprouMorto && TemCanastra(jog))
            {
                RegistrarBatida(jog, false);
                return;
            }

            // 3.4) DESCARTE: joga uma carta no lixo (encerra a parte normal do turno).
            Descartar(jog);

            // 3.5) Se o jogador descartou a ULTIMA carta e ainda NAO tinha pegado
            //      o morto, ele pega o morto agora e continua jogando.
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

            // 3.6) BATIDA COM DESCARTE: se a mao zerou apos o descarte e e legal.
            if (jog.Mao.EstaVazia() && jog.ComprouMorto && TemCanastra(jog))
            {
                RegistrarBatida(jog, true);
                return;
            }
        }

        // ====================================================================
        // COMPRA
        // ====================================================================
        private void Comprar(Jogador jog)
        {
            // Decide se vale a pena comprar o lixo inteiro.
            if (!partida.Lixo.EstaVazia() && DeveComprarDoLixo(jog))
            {
                int n = partida.Lixo.Quantidade;
                // Pega o lixo inteiro: desempilha tudo e poe na mao.
                while (!partida.Lixo.EstaVazia())
                {
                    jog.Mao.Adicionar(partida.Lixo.Desempilhar());
                }
                log.Registrar("  " + jog.Nome + " comprou o LIXO inteiro (" + n + " carta(s)).");
                return;
            }

            // Senao, compra do monte (carta do topo da pilha).
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

        // Decide se o jogador deve comprar o lixo (heuristica simples).
        private bool DeveComprarDoLixo(Jogador jog)
        {
            Carta topo = partida.Lixo.VerTopo();

            // Curinga e sempre valioso: vale a pena pegar.
            if (topo.EhCuringa)
            {
                return true;
            }

            // Se a carta do topo encaixa em algum jogo ja baixado, vale a pena.
            JogoMesa[] jogos = jog.Jogos.ParaVetor();
            for (int i = 0; i < jogos.Length; i++)
            {
                if (PodeAdicionarAoJogo(jogos[i], topo))
                {
                    return true;
                }
            }

            // Se ja temos 2+ cartas vizinhas do mesmo naipe, pegar o lixo
            // ajuda a formar uma sequencia nova.
            if (ContaVizinhosMesmoNaipe(jog, topo) >= 2)
            {
                return true;
            }

            return false;
        }

        // Conta quantas cartas naturais da mao sao do mesmo naipe e estao
        // "perto" (rank dentro de +-2) da carta dada - indicio de sequencia.
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

        // ====================================================================
        // FASE DE JOGOS (baixar e encaixar)
        // ====================================================================
        private void FaseDeJogos(Jogador jog)
        {
            // Continua tentando enquanto houver alguma mudanca (encaixe, novo
            // jogo ou fechamento de canastra com curinga). Cada mudanca diminui
            // a mao ou pega o morto, entao o laco sempre termina.
            bool houveMudanca = true;
            while (houveMudanca)
            {
                houveMudanca = false;

                if (TentarEncaixar(jog)) houveMudanca = true;
                if (CriarNovosJogos(jog)) houveMudanca = true;
                if (FecharCanastraComCuringa(jog)) houveMudanca = true;

                // Se zerou a mao e ainda nao tinha pegado o morto, pega agora
                // e continua tentando baixar com a nova mao (as 11 cartas do morto).
                if (jog.Mao.EstaVazia() && !jog.ComprouMorto && HaMortoDisponivel())
                {
                    PegarMorto(jog);
                    houveMudanca = true;
                }
            }
        }

        // Tenta encaixar cartas da mao nos jogos ja baixados.
        private bool TentarEncaixar(Jogador jog)
        {
            bool encaixouAlgo = false;
            bool achouNestaVolta = true;

            // Reinicia a varredura sempre que encaixa uma carta, pois a mao muda.
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

        // Procura, na mao, sequencias novas (mesmo naipe, 3+ cartas seguidas)
        // e as baixa na mesa.
        private bool CriarNovosJogos(Jogador jog)
        {
            bool criouAlgo = false;

            for (int n = 0; n < NAIPES.Length; n++)
            {
                Naipe naipe = NAIPES[n];

                // Mapa rank -> uma carta representante daquele rank (indices 3..14).
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

                // Varre os ranks de 3 a 14 procurando trechos consecutivos.
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

                        // So baixa se tiver 3+ cartas E se for permitido reduzir a mao.
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

        // Usa um curinga da mao para transformar uma sequencia de 6 cartas
        // (sem curinga) em uma canastra de 7 cartas.
        private bool FecharCanastraComCuringa(Jogador jog)
        {
            // Procura um curinga na mao.
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

            // So vale se for permitido reduzir a mao.
            if (!PodeReduzirPara(jog, jog.Mao.Quantidade - 1))
            {
                return false;
            }

            // Procura um jogo de exatamente 6 cartas, sem curinga, com espaco
            // para receber o curinga (virando canastra de 7).
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

        // ====================================================================
        // REGRAS DE ENCAIXE E DE REDUCAO DA MAO
        // ====================================================================

        // Verifica se a carta "c" pode ser adicionada ao jogo "m".
        private bool PodeAdicionarAoJogo(JogoMesa m, Carta c)
        {
            // CURINGA: pela regra adotada, no maximo 1 por jogo, e precisa de
            // espaco para estender a sequencia (para cima ou para baixo).
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
                    return true; // da para subir
                }
                if (m.MinNatural() > 3)
                {
                    return true; // da para descer
                }
                return false;
            }

            // CARTA NATURAL: precisa ser do mesmo naipe e encaixar numa ponta.
            if (c.Naipe != m.Naipe)
            {
                return false;
            }

            int r = c.RankSequencia;

            // Nao pode repetir um rank que ja existe na sequencia.
            if (m.ContemRank(r))
            {
                return false;
            }

            int lo = m.MinNatural();
            int hi = m.MaxNatural();

            // Encaixa logo acima da maior carta?
            if (r == hi + 1 && r <= 14)
            {
                return true;
            }
            // Encaixa logo abaixo da menor carta?
            if (r == lo - 1 && r >= 3)
            {
                return true;
            }

            return false;
        }

        // Decide se e permitido deixar a mao com "resultante" cartas apos baixar.
        // Esta regra evita "batidas ilegais" e garante que sempre sobre uma
        // carta para o descarte quando ainda nao da para bater.
        private bool PodeReduzirPara(Jogador jog, int resultante)
        {
            if (jog.ComprouMorto)
            {
                // Ja pegou o morto:
                if (TemCanastra(jog))
                {
                    return resultante >= 0; // pode zerar a mao -> BATIDA legal
                }
                else
                {
                    return resultante >= 2; // mantem cartas p/ descartar sem bater
                }
            }
            else
            {
                // Ainda nao pegou o morto:
                if (resultante >= 1)
                {
                    return true;
                }
                // Pode zerar a mao se houver morto disponivel para pegar.
                return HaMortoDisponivel();
            }
        }

        // ====================================================================
        // DESCARTE
        // ====================================================================
        private void Descartar(Jogador jog)
        {
            Carta[] mao = jog.Mao.ParaVetor();
            if (mao.Length == 0)
            {
                return; // nada para descartar (situacao de batida ja tratada)
            }

            Carta escolhida = EscolherDescarte(jog, mao);
            jog.Mao.Remover(escolhida);
            partida.Lixo.Empilhar(escolhida);
            log.Registrar("  " + jog.Nome + " descartou " + escolhida.ToString()
                          + " no lixo (mao: " + jog.Mao.Quantidade + " cartas).");
        }

        // Escolhe qual carta descartar (heuristica simples):
        //  1) prefere uma carta "isolada" (sem vizinha do mesmo naipe);
        //  2) entre as candidatas, a de maior valor (para reduzir risco);
        //  3) nunca descarta curinga, a menos que so restem curingas.
        private Carta EscolherDescarte(Jogador jog, Carta[] mao)
        {
            Carta melhor = null;
            int melhorValor = -1;

            // 1) Cartas isoladas.
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

            // 2) Qualquer carta natural de maior valor.
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

            // 3) So restam curingas.
            return mao[0];
        }

        // Diz se uma carta nao tem nenhuma vizinha (rank +-1) do mesmo naipe na mao.
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

        // ====================================================================
        // MORTO, BATIDA E ENCERRAMENTO
        // ====================================================================

        // Indica se ainda existe algum morto disponivel para ser pego.
        private bool HaMortoDisponivel()
        {
            return partida.MortoADisponivel || partida.MortoBDisponivel;
        }

        // O jogador pega um morto: as 11 cartas viram parte da mao dele.
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

        // Verifica se o jogador tem pelo menos uma canastra (necessario p/ bater).
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

        // Registra a batida e encerra a partida.
        private void RegistrarBatida(Jogador jog, bool comDescarte)
        {
            partida.Batedor = jog;
            partida.Encerrada = true;
            string tipo = comDescarte ? " (com descarte)" : " (sem descarte)";
            log.Registrar("  *** " + jog.Nome + " BATEU" + tipo + "! A partida foi encerrada. ***");
        }

        // Encerra a partida porque o monte acabou (ninguem bateu).
        private void EncerrarPorMonteVazio()
        {
            partida.Encerrada = true;
            log.Registrar("  O MONTE acabou. A partida e encerrada (ninguem bateu).");
        }

        // ====================================================================
        // APURACAO FINAL
        // ====================================================================
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
                partida.Vencedor = null; // empate
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

        // ====================================================================
        // RESUMO FINAL (impresso direto na tela, fora do log)
        // ====================================================================
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
