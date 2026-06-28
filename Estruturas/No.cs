namespace Buraco.Estruturas
{
    // ============================================================================
    // CLASSE: No<T>  (No generico / "nodo" de uma estrutura encadeada)
    // ----------------------------------------------------------------------------
    // OBJETIVO DA CLASSE:
    //   Representar UMA "caixinha" de uma estrutura de dados encadeada.
    //   Cada No guarda um VALOR e um ponteiro/referencia para o PROXIMO No.
    //
    //   Esta mesma classe e reutilizada por TODAS as nossas estruturas manuais:
    //     - PilhaCartas  (usa No<Carta>)
    //     - FilaLog       (usa No<string>)
    //     - ListaCartas   (usa No<Carta>)
    //     - ListaJogos    (usa No<JogoMesa>)
    //
    //   Por isso ela e GENERICA (<T>): funciona com qualquer tipo de dado.
    //   Esse e um conceito classico de Estruturas de Dados: a "lista encadeada"
    //   nada mais e do que varios Nos ligados um no outro como uma corrente.
    // ============================================================================
    public class No<T>
    {
        // Valor armazenado dentro do No (a "carga" que ele transporta).
        public T Valor;

        // Referencia para o proximo No da corrente.
        // Quando vale null, significa que este e o ULTIMO No (fim da estrutura).
        public No<T> Proximo;

        // Construtor: cria um No ja guardando o valor recebido.
        // O "Proximo" comeca como null porque, ao ser criado, o No ainda
        // nao esta ligado a ninguem.
        public No(T valor)
        {
            Valor = valor;
            Proximo = null;
        }
    }
}
