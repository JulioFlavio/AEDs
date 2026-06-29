using System;
using System.Text;
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
    //   Esta versao do trabalho roda em MODO MANUAL (INTERATIVO), no estilo
    //   "hot-seat": os DOIS jogadores sao pessoas e jogam no MESMO computador,
    //   revezando a vez. A cada turno o programa mostra a mao do jogador da vez
    //   e pergunta o que ele quer fazer (comprar, baixar jogos, encaixar cartas,
    //   descartar). Assim, a partida acontece pelas ACOES DOS JOGADORES.
    //
    //   IMPORTANTE: o foco do trabalho continua sendo as ESTRUTURAS DE DADOS
    //   (Pilha no monte/lixo, Fila no log, Listas na mao/morto/mesa). A classe
    //   apenas as movimenta conforme as escolhas de quem esta jogando.
    // ============================================================================
    public class PartidaService
    {
        // Estado da partida (jogadores, monte, lixo, mortos...).
        private Partida partida;

        // Gerador de numeros aleatorios (para embaralhar). Recebe uma "semente"
        // para que a distribuicao possa ser repetida igualzinha, se desejado.
        private Random rnd;

        // Servico de log: registra cada acontecimento da partida (Fila/FIFO).
        private LogService log;

        // Detecta o fim da entrada (EOF) para o programa nao ficar travado lendo
        // de uma entrada que ja acabou (por exemplo, quando os comandos vem de um
        // arquivo). No uso normal (pessoa digitando) isto nunca acontece.
        private bool entradaAcabou = false;

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
        // Recebe os nomes dos dois jogadores (com valores padrao, caso o
        // Program nao informe nada).
        public void Iniciar(string nome1 = "Jogador 1", string nome2 = "Jogador 2")
        {
            // Cria o estado inicial da partida.
            partida = new Partida();
            partida.Jogadores = new Jogador[2];
            partida.Jogadores[0] = new Jogador(nome1);
            partida.Jogadores[1] = new Jogador(nome2);
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

            // Joga turno apos turno ate alguem bater, o monte/lixo acabarem ou
            // bater o limite de seguranca.
            while (!partida.Encerrada && turnos < MAX_TURNOS)
            {
                Jogador jog = partida.Jogadores[partida.JogadorDaVez];
                ExecutarTurnoInterativo(jog);
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
        // 3) UM TURNO DE UM JOGADOR (INTERATIVO)
        // ====================================================================
        private void ExecutarTurnoInterativo(Jogador jog)
        {
            // Hot-seat: limpa a tela e pede para passar o computador para o
            // jogador da vez, evitando que o outro veja a mao.
            LimparTela();
            Console.WriteLine("======================================================");
            Console.WriteLine(" Rodada " + partida.NumeroRodada + " - Vez de " + jog.Nome);
            Console.WriteLine("======================================================");
            Console.WriteLine("(Garanta que so " + jog.Nome + " esta vendo a tela.)");
            Console.Write("Pressione ENTER para ver as suas cartas...");
            LerLinhaCrua();
            if (entradaAcabou) { partida.Encerrada = true; return; }

            log.Registrar("");
            log.Registrar("Rodada " + partida.NumeroRodada + " - vez de " + jog.Nome
                          + " (mao: " + jog.Mao.Quantidade + " cartas).");

            MostrarEstadoPublico();
            MostrarMaoNumerada(jog);

            // 3.1) COMPRA: do monte ou do lixo.
            ComprarInterativo(jog);
            if (partida.Encerrada)
            {
                return; // o monte e o lixo acabaram durante a compra
            }

            // 3.2) FASE DE JOGOS: baixar novas sequencias e encaixar cartas.
            FaseDeJogosInterativa(jog);
            if (entradaAcabou) { partida.Encerrada = true; return; }

            // 3.3) BATIDA SEM DESCARTE: se zerou a mao de forma legal.
            if (jog.Mao.EstaVazia() && jog.ComprouMorto && TemCanastra(jog))
            {
                RegistrarBatida(jog, false);
                return;
            }

            // 3.4) DESCARTE: joga uma carta no lixo (encerra a parte normal do turno).
            if (!jog.Mao.EstaVazia())
            {
                DescartarInterativo(jog);
            }
            if (entradaAcabou) { partida.Encerrada = true; return; }

            // 3.5) Se o jogador descartou a ULTIMA carta e ainda NAO tinha pegado
            //      o morto, ele pega o morto agora e continua jogando.
            if (jog.Mao.EstaVazia() && !jog.ComprouMorto && HaMortoDisponivel())
            {
                PegarMorto(jog);
                FaseDeJogosInterativa(jog);
                if (entradaAcabou) { partida.Encerrada = true; return; }

                if (jog.Mao.EstaVazia() && jog.ComprouMorto && TemCanastra(jog))
                {
                    RegistrarBatida(jog, false);
                    return;
                }
                if (!jog.Mao.EstaVazia())
                {
                    DescartarInterativo(jog);
                }
            }

            // 3.6) BATIDA COM DESCARTE: se a mao zerou apos o descarte e e legal.
            if (jog.Mao.EstaVazia() && jog.ComprouMorto && TemCanastra(jog))
            {
                RegistrarBatida(jog, true);
                return;
            }

            // Fim normal do turno.
            Console.WriteLine();
            Console.WriteLine(jog.Nome + " encerrou o turno.");
            Console.Write("Pressione ENTER para passar a vez...");
            LerLinhaCrua();
            if (entradaAcabou) { partida.Encerrada = true; }
        }

        // ====================================================================
        // COMPRA (INTERATIVA)
        // ====================================================================
        private void ComprarInterativo(Jogador jog)
        {
            bool monteVazio = partida.Monte.EstaVazia();
            bool lixoVazio = partida.Lixo.EstaVazia();

            // Sem monte e sem lixo: nao ha de onde comprar -> encerra a partida.
            if (monteVazio && lixoVazio)
            {
                EncerrarPorMonteVazio();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("--- COMPRA ---");

            // Monte vazio (mas o lixo tem cartas): so resta comprar o lixo.
            if (monteVazio)
            {
                Console.WriteLine("O monte acabou: voce vai comprar o LIXO inteiro.");
                ComprarLixoInteiro(jog);
                return;
            }

            // Mostra as opcoes de compra.
            Console.WriteLine("  [1] Comprar 1 carta do MONTE (" + partida.Monte.Quantidade + " no monte)");
            if (!lixoVazio)
            {
                Console.WriteLine("  [2] Comprar o LIXO inteiro (" + partida.Lixo.Quantidade
                                  + " carta(s), topo: " + partida.Lixo.VerTopo().ToString() + ")");
            }
            else
            {
                Console.WriteLine("  [2] LIXO vazio (indisponivel)");
            }

            int op = LerInteiro("Opcao: ", 1, 2);

            if (op == 2 && !lixoVazio)
            {
                ComprarLixoInteiro(jog);
                return;
            }
            if (op == 2 && lixoVazio)
            {
                Console.WriteLine("Lixo vazio. Comprando do monte.");
            }

            // Compra do monte (carta do topo da pilha).
            Carta c = partida.Monte.Desempilhar();
            jog.Mao.Adicionar(c);
            Anunciar("  " + jog.Nome + " comprou do MONTE: " + c.ToString()
                     + " (restam " + partida.Monte.Quantidade + " no monte).");
        }

        // Pega o LIXO inteiro: desempilha tudo e poe na mao (regra adotada).
        private void ComprarLixoInteiro(Jogador jog)
        {
            int n = partida.Lixo.Quantidade;
            while (!partida.Lixo.EstaVazia())
            {
                jog.Mao.Adicionar(partida.Lixo.Desempilhar());
            }
            Anunciar("  " + jog.Nome + " comprou o LIXO inteiro (" + n + " carta(s)).");
        }

        // ====================================================================
        // FASE DE JOGOS (INTERATIVA): baixar e encaixar
        // ====================================================================
        private void FaseDeJogosInterativa(Jogador jog)
        {
            bool continuar = true;
            while (continuar && !entradaAcabou && !partida.Encerrada)
            {
                // Se o jogador esvaziou a mao e ainda NAO tinha pegado o morto,
                // ele pega o morto automaticamente (regra) e continua jogando.
                if (jog.Mao.EstaVazia() && !jog.ComprouMorto && HaMortoDisponivel())
                {
                    PegarMorto(jog);
                }

                // Mao vazia (ja com o morto pego): nao ha mais o que baixar.
                // A batida sera checada fora deste metodo.
                if (jog.Mao.EstaVazia())
                {
                    return;
                }

                MostrarMaoNumerada(jog);
                Console.WriteLine();
                Console.WriteLine("--- O que deseja fazer? ---");
                Console.WriteLine("  [1] Baixar um novo jogo (sequencia do mesmo naipe)");
                Console.WriteLine("  [2] Encaixar uma carta num jogo ja baixado");
                Console.WriteLine("  [3] Ver a mesa e a minha mao de novo");
                Console.WriteLine("  [4] Encerrar jogadas e ir para o DESCARTE");

                int op = LerInteiro("Opcao: ", 1, 4);

                if (op == 1) BaixarNovoJogoInterativo(jog);
                else if (op == 2) EncaixarInterativo(jog);
                else if (op == 3) { MostrarEstadoPublico(); }
                else if (op == 4) continuar = false;
            }
        }

        // Pergunta as cartas e tenta baixar uma NOVA sequencia na mesa.
        private void BaixarNovoJogoInterativo(Jogador jog)
        {
            if (jog.Mao.Quantidade < 3)
            {
                Console.WriteLine("Voce precisa de pelo menos 3 cartas para baixar um jogo.");
                return;
            }

            MostrarMaoNumerada(jog);
            Console.WriteLine("Escolha as cartas que formam a sequencia (mesmo naipe, numeros seguidos).");
            Carta[] sel = SelecionarCartasDaMao(jog);
            if (sel == null)
            {
                Console.WriteLine("Operacao cancelada.");
                return;
            }

            // Valida se as cartas escolhidas formam mesmo uma sequencia.
            Naipe naipe;
            string erro;
            if (!ValidarNovaSequencia(sel, out naipe, out erro))
            {
                Console.WriteLine("Jogada invalida: " + erro);
                return;
            }

            // Valida a regra de reducao de mao (batida/morto).
            if (!PodeReduzirPara(jog, jog.Mao.Quantidade - sel.Length))
            {
                Console.WriteLine("Jogada invalida: voce nao pode reduzir a sua mao assim agora.");
                Console.WriteLine("(So pode esvaziar a mao para PEGAR O MORTO, ou para BATER com o");
                Console.WriteLine(" morto ja pego e tendo ao menos uma canastra.)");
                return;
            }

            // Cria o jogo e move as cartas da mao para a mesa.
            JogoMesa novo = new JogoMesa(naipe);
            for (int i = 0; i < sel.Length; i++)
            {
                jog.Mao.Remover(sel[i]);
                novo.AdicionarCarta(sel[i]);
            }
            jog.Jogos.Adicionar(novo);
            Anunciar("  " + jog.Nome + " baixou um novo jogo: " + novo.ToString());
        }

        // Pergunta o jogo-alvo e a carta, e tenta encaixar.
        private void EncaixarInterativo(Jogador jog)
        {
            JogoMesa[] jogos = jog.Jogos.ParaVetor();
            if (jogos.Length == 0)
            {
                Console.WriteLine("Voce ainda nao baixou nenhum jogo para encaixar cartas.");
                return;
            }

            MostrarJogosNumerados(jog);
            int ij = LerInteiro("Escolha o numero do jogo onde quer encaixar: ", 0, jogos.Length - 1);
            JogoMesa alvo = jogos[ij];

            MostrarMaoNumerada(jog);
            Console.WriteLine("Escolha a carta da mao para encaixar nesse jogo.");
            Carta carta = SelecionarUmaCarta(jog, true);
            if (carta == null)
            {
                Console.WriteLine("Operacao cancelada.");
                return;
            }

            if (!PodeAdicionarAoJogo(alvo, carta))
            {
                Console.WriteLine("Essa carta nao encaixa nesse jogo.");
                return;
            }
            if (!PodeReduzirPara(jog, jog.Mao.Quantidade - 1))
            {
                Console.WriteLine("Jogada invalida: voce nao pode reduzir a sua mao assim agora.");
                return;
            }

            jog.Mao.Remover(carta);
            alvo.AdicionarCarta(carta);
            Anunciar("  " + jog.Nome + " encaixou " + carta.ToString()
                     + " em um jogo de " + alvo.Naipe
                     + " (agora com " + alvo.Quantidade + " cartas).");
        }

        // ====================================================================
        // VALIDACAO DE UMA NOVA SEQUENCIA (para BAIXAR)
        // ====================================================================
        // Verifica se as cartas escolhidas formam uma sequencia valida:
        //  - 3 ou mais cartas;
        //  - todas as naturais do mesmo naipe;
        //  - no maximo 1 curinga;
        //  - numeros consecutivos (o curinga pode tapar 1 buraco OU estender
        //    uma ponta);
        //  - sem repetir o mesmo numero.
        // Devolve o naipe do jogo em "naipeJogo" e a mensagem de erro em "erro".
        private bool ValidarNovaSequencia(Carta[] sel, out Naipe naipeJogo, out string erro)
        {
            naipeJogo = Naipe.Copas; // valor provisorio; sera ajustado abaixo
            erro = "";

            if (sel == null || sel.Length < 3)
            {
                erro = "um jogo precisa de pelo menos 3 cartas.";
                return false;
            }

            // Conta curingas, conta naturais e descobre o naipe (pela 1a natural).
            int curingas = 0;
            int naturais = 0;
            bool achouNaipe = false;
            for (int i = 0; i < sel.Length; i++)
            {
                if (sel[i].EhCuringa)
                {
                    curingas++;
                }
                else
                {
                    naturais++;
                    if (!achouNaipe)
                    {
                        naipeJogo = sel[i].Naipe;
                        achouNaipe = true;
                    }
                    else if (sel[i].Naipe != naipeJogo)
                    {
                        erro = "todas as cartas naturais devem ser do mesmo naipe.";
                        return false;
                    }
                }
            }

            if (curingas > 1)
            {
                erro = "cada jogo aceita no maximo 1 curinga.";
                return false;
            }
            if (naturais < 2)
            {
                erro = "faltam cartas naturais para formar a sequencia.";
                return false;
            }

            // Copia os ranks das naturais e ordena (insertion sort manual).
            int[] ranks = new int[naturais];
            int idx = 0;
            for (int i = 0; i < sel.Length; i++)
            {
                if (!sel[i].EhCuringa)
                {
                    ranks[idx] = sel[i].RankSequencia;
                    idx++;
                }
            }
            for (int i = 1; i < ranks.Length; i++)
            {
                int chave = ranks[i];
                int j = i - 1;
                while (j >= 0 && ranks[j] > chave)
                {
                    ranks[j + 1] = ranks[j];
                    j--;
                }
                ranks[j + 1] = chave;
            }

            // Nao pode repetir numero.
            for (int i = 1; i < ranks.Length; i++)
            {
                if (ranks[i] == ranks[i - 1])
                {
                    erro = "nao pode repetir o mesmo numero na sequencia.";
                    return false;
                }
            }

            // Conta os "buracos" entre as naturais (numeros que faltam).
            int buracos = 0;
            for (int i = 1; i < ranks.Length; i++)
            {
                buracos += (ranks[i] - ranks[i - 1] - 1);
            }

            if (curingas == 0)
            {
                if (buracos != 0)
                {
                    erro = "as cartas nao sao consecutivas (e nao ha curinga para tapar o buraco).";
                    return false;
                }
            }
            else // exatamente 1 curinga
            {
                if (buracos > 1)
                {
                    erro = "ha mais de um buraco na sequencia para apenas 1 curinga.";
                    return false;
                }
                if (buracos == 0)
                {
                    // Sem buraco: o curinga so pode ESTENDER uma ponta, e para
                    // isso precisa haver espaco (abaixo do menor ou acima do maior).
                    int min = ranks[0];
                    int max = ranks[ranks.Length - 1];
                    if (!(min > 3 || max < 14))
                    {
                        erro = "nao ha espaco para o curinga estender a sequencia.";
                        return false;
                    }
                }
            }

            return true;
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
        // DESCARTE (INTERATIVO)
        // ====================================================================
        private void DescartarInterativo(Jogador jog)
        {
            Console.WriteLine();
            Console.WriteLine("DESCARTE: escolha 1 carta para jogar no lixo (encerra o seu turno).");
            MostrarMaoNumerada(jog);

            // O descarte e OBRIGATORIO: nao permite cancelar.
            Carta c = SelecionarUmaCarta(jog, false);
            if (c == null)
            {
                return; // so chega aqui se a mao estiver vazia ou a entrada acabou
            }

            jog.Mao.Remover(c);
            partida.Lixo.Empilhar(c);
            Anunciar("  " + jog.Nome + " descartou " + c.ToString()
                     + " no lixo (mao: " + jog.Mao.Quantidade + " cartas).");
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
            Anunciar("  " + jog.Nome + " esvaziou a mao e PEGOU O " + nome
                     + " (" + qtd + " cartas). Continue jogando!");
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
            Anunciar("  *** " + jog.Nome + " BATEU" + tipo + "! A partida foi encerrada. ***");
        }

        // Encerra a partida porque nao ha mais de onde comprar (ninguem bateu).
        private void EncerrarPorMonteVazio()
        {
            partida.Encerrada = true;
            Anunciar("  As cartas para compra acabaram. A partida e encerrada (ninguem bateu).");
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

        // ====================================================================
        // EXIBICAO NA TELA (ajuda o jogador a enxergar o estado do jogo)
        // ====================================================================

        // Mostra o que e PUBLICO: monte (quantidade), topo do lixo, mortos
        // disponiveis e os jogos ja baixados por cada jogador na mesa.
        private void MostrarEstadoPublico()
        {
            Console.WriteLine();
            Console.WriteLine("------------------- MESA -------------------");
            Console.WriteLine("Monte: " + partida.Monte.Quantidade + " carta(s).");
            if (partida.Lixo.EstaVazia())
            {
                Console.WriteLine("Lixo: vazio.");
            }
            else
            {
                Console.WriteLine("Lixo: " + partida.Lixo.Quantidade
                                  + " carta(s) (topo: " + partida.Lixo.VerTopo().ToString() + ").");
            }

            int mortos = 0;
            if (partida.MortoADisponivel) mortos++;
            if (partida.MortoBDisponivel) mortos++;
            Console.WriteLine("Mortos ainda disponiveis: " + mortos);

            for (int i = 0; i < partida.Jogadores.Length; i++)
            {
                Jogador j = partida.Jogadores[i];
                Console.WriteLine("Jogos de " + j.Nome + ":");
                JogoMesa[] jogos = j.Jogos.ParaVetor();
                if (jogos.Length == 0)
                {
                    Console.WriteLine("   (nenhum jogo baixado)");
                }
                else
                {
                    for (int k = 0; k < jogos.Length; k++)
                    {
                        Console.WriteLine("   " + jogos[k].ToString());
                    }
                }
            }
            Console.WriteLine("--------------------------------------------");
        }

        // Mostra a mao do jogador com um NUMERO (indice) antes de cada carta,
        // para que ele possa escolher cartas digitando esses numeros.
        private void MostrarMaoNumerada(Jogador jog)
        {
            Carta[] mao = jog.Mao.ParaVetor();
            Console.WriteLine();
            Console.WriteLine("Sua mao (" + mao.Length + " carta(s)):");
            if (mao.Length == 0)
            {
                Console.WriteLine("  (vazia)");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("  ");
            for (int i = 0; i < mao.Length; i++)
            {
                sb.Append("[" + i + "]" + mao[i].ToString() + "   ");
                // Quebra a linha a cada 6 cartas para nao ficar tudo numa linha so.
                if ((i + 1) % 6 == 0 && (i + 1) < mao.Length)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append("  ");
                }
            }
            Console.WriteLine(sb.ToString());
        }

        // Mostra os jogos ja baixados pelo jogador, numerados (para encaixe).
        private void MostrarJogosNumerados(Jogador jog)
        {
            JogoMesa[] jogos = jog.Jogos.ParaVetor();
            Console.WriteLine("Seus jogos na mesa:");
            for (int i = 0; i < jogos.Length; i++)
            {
                Console.WriteLine("  [" + i + "] " + jogos[i].ToString());
            }
        }

        // ====================================================================
        // LEITURA DA ENTRADA (com validacao simples)
        // ====================================================================

        // Le uma linha "crua" da entrada. Se a entrada acabou (EOF), marca a
        // flag e devolve texto vazio (para o programa nao travar).
        private string LerLinhaCrua()
        {
            string s = Console.ReadLine();
            if (s == null)
            {
                entradaAcabou = true;
                return "";
            }
            return s;
        }

        // Le uma linha ja "limpa" (sem espacos nas pontas), mostrando um prompt.
        private string LerTexto(string prompt)
        {
            Console.Write(prompt);
            return LerLinhaCrua().Trim();
        }

        // Le um numero inteiro dentro de [min, max], repetindo ate ser valido.
        private int LerInteiro(string prompt, int min, int max)
        {
            while (true)
            {
                if (entradaAcabou) return min; // protecao contra EOF
                string s = LerTexto(prompt);
                int v;
                if (int.TryParse(s, out v) && v >= min && v <= max)
                {
                    return v;
                }
                if (entradaAcabou) return min;
                Console.WriteLine("Digite um numero entre " + min + " e " + max + ".");
            }
        }

        // Mostra a mao numerada e le UMA carta escolhida pelo jogador.
        // Se "permitirCancelar" for true, ENTER vazio cancela e devolve null.
        private Carta SelecionarUmaCarta(Jogador jog, bool permitirCancelar)
        {
            while (true)
            {
                if (entradaAcabou) return null;

                Carta[] mao = jog.Mao.ParaVetor();
                if (mao.Length == 0) return null;

                string dica = permitirCancelar ? " (ENTER para cancelar)" : "";
                string linha = LerTexto("Numero da carta" + dica + ": ");

                if (linha == "")
                {
                    if (permitirCancelar) return null;
                    if (entradaAcabou) return null;
                    Console.WriteLine("Voce precisa escolher uma carta.");
                    continue;
                }

                int idx;
                if (!int.TryParse(linha, out idx) || idx < 0 || idx >= mao.Length)
                {
                    Console.WriteLine("Numero invalido. Tente novamente.");
                    continue;
                }
                return mao[idx];
            }
        }

        // Mostra a mao numerada e le VARIAS cartas (numeros separados por espaco).
        // ENTER vazio cancela e devolve null. Tambem devolve null se houver um
        // numero invalido ou repetido (o jogador tenta de novo pelo menu).
        private Carta[] SelecionarCartasDaMao(Jogador jog)
        {
            if (entradaAcabou) return null;

            Carta[] mao = jog.Mao.ParaVetor();
            string linha = LerTexto("Numeros das cartas separados por espaco (ENTER para cancelar): ");
            if (linha == "") return null;

            string[] partes = linha.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length == 0) return null;

            Carta[] sel = new Carta[partes.Length];
            bool[] usado = new bool[mao.Length];

            for (int i = 0; i < partes.Length; i++)
            {
                int idx;
                if (!int.TryParse(partes[i], out idx) || idx < 0 || idx >= mao.Length)
                {
                    Console.WriteLine("Numero invalido: '" + partes[i] + "'.");
                    return null;
                }
                if (usado[idx])
                {
                    Console.WriteLine("Voce repetiu o numero " + idx + ".");
                    return null;
                }
                usado[idx] = true;
                sel[i] = mao[idx];
            }
            return sel;
        }

        // Limpa a tela entre os turnos (hot-seat). Em terminais que nao suportam
        // limpar a tela, apenas ignora o erro.
        private void LimparTela()
        {
            try
            {
                Console.Clear();
            }
            catch
            {
                // Alguns terminais/saidas redirecionadas nao permitem limpar.
            }
        }

        // Mostra a mensagem na tela E registra no log (Fila). Usado para os
        // ACONTECIMENTOS importantes do jogo (comprar, baixar, encaixar,
        // descartar, pegar morto, bater...).
        private void Anunciar(string mensagem)
        {
            Console.WriteLine(mensagem);
            log.Registrar(mensagem);
        }
    }
}
