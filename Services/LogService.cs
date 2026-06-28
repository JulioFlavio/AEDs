using System;
using System.IO;
using Buraco.Estruturas;

namespace Buraco.Services
{
    // ============================================================================
    // CLASSE: LogService
    // ----------------------------------------------------------------------------
    // OBJETIVO DA CLASSE:
    //   Registrar TODOS os acontecimentos da partida (o "log") e, no final,
    //   mostrar/salvar esse historico em ordem.
    //
    //   Internamente o log usa a nossa FILA manual (FilaLog), porque os eventos
    //   devem ser exibidos na MESMA ordem em que aconteceram (comportamento
    //   FIFO: primeiro a entrar, primeiro a sair).
    // ============================================================================
    public class LogService
    {
        // A fila que guarda as mensagens do log, em ordem cronologica.
        private FilaLog fila;

        // CONSTRUTOR: cria o log com a fila vazia.
        public LogService()
        {
            fila = new FilaLog();
        }

        // Quantas mensagens estao registradas.
        public int Quantidade
        {
            get { return fila.Quantidade; }
        }

        // ------------------------------------------------------------------
        // Registrar: adiciona uma mensagem ao final do log (Enfileirar).
        // ------------------------------------------------------------------
        public void Registrar(string mensagem)
        {
            fila.Enfileirar(mensagem);
        }

        // ------------------------------------------------------------------
        // SalvarEmArquivo: grava todo o log num arquivo de texto.
        // Usa MontarTexto (que NAO esvazia a fila), para podermos imprimir
        // depois tambem.
        // ------------------------------------------------------------------
        public void SalvarEmArquivo(string caminho)
        {
            File.WriteAllText(caminho, fila.MontarTexto());
        }

        // ------------------------------------------------------------------
        // Imprimir: mostra o log inteiro na tela.
        //
        // Aqui usamos DESENFILEIRAR de proposito: vamos retirando da frente da
        // fila e imprimindo, o que demonstra na pratica o comportamento FIFO
        // (a partida e "reproduzida" na ordem exata em que ocorreu).
        // Observacao: por isso este metodo ESVAZIA a fila; chamamos
        // SalvarEmArquivo antes, quando queremos as duas coisas.
        // ------------------------------------------------------------------
        public void Imprimir()
        {
            while (!fila.EstaVazia())
            {
                string mensagem = fila.Desenfileirar();
                Console.WriteLine(mensagem);
            }
        }
    }
}
