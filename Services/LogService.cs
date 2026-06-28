using System;
using System.IO;
using Buraco.Estruturas;

namespace Buraco.Services
{
    public class LogService
    {
        private FilaLog fila;

        public LogService()
        {
            fila = new FilaLog();
        }

        public int Quantidade
        {
            get { return fila.Quantidade; }
        }

        public void Registrar(string mensagem)
        {
            fila.Enfileirar(mensagem);
        }

        public void SalvarEmArquivo(string caminho)
        {
            File.WriteAllText(caminho, fila.MontarTexto());
        }

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
