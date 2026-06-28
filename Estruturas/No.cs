namespace Buraco.Estruturas
{
    public class No<T>
    {
        public T Valor;
        public No<T> Proximo;

        public No(T valor)
        {
            Valor = valor;
            Proximo = null;
        }
    }
}
