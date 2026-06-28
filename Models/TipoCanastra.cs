namespace Buraco.Models
{
    // ============================================================================
    // ENUM: TipoCanastra
    // ----------------------------------------------------------------------------
    // OBJETIVO:
    //   Classificar um jogo (sequencia) de 7 ou mais cartas, dizendo QUE tipo
    //   de canastra ele e. Cada tipo vale uma pontuacao de bonus diferente.
    //
    //   REGRAS ADOTADAS NESTE TRABALHO (resumo):
    //     NaoEhCanastra -> tem menos de 7 cartas (ainda nao e canastra).
    //     Suja          -> 7+ cartas COM curinga, sem chegar ao As. Bonus 100.
    //     Limpa         -> 7+ cartas SEM curinga, sem chegar ao As.  Bonus 200.
    //     MeiaReal      -> 7+ cartas COM curinga e que chega ao As.  Bonus 250.
    //     Real          -> 7+ cartas SEM curinga e que chega ao As.  Bonus 500.
    //
    //   ("chegar ao As" = a sequencia, do mesmo naipe, contem o As natural no
    //    topo, ex.: ... J Q K A. E a canastra mais valiosa, chamada de "real".)
    // ============================================================================
    public enum TipoCanastra
    {
        NaoEhCanastra,
        Suja,
        Limpa,
        MeiaReal,
        Real
    }
}
