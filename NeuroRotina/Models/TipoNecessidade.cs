namespace NeuroRotina.Models;

public record TipoNecessidade(
    string Chave,
    string Nome,
    string Frase,
    string Confirmacao);

public static class Necessidades
{
    public static readonly TipoNecessidade[] Todas =
    {
        new("agua", "Água", "Eu quero beber água.", "Pedi ÁGUA!"),
        new("banheiro", "Banheiro", "Eu quero ir ao banheiro.", "Pedi BANHEIRO!"),
        new("brincar", "Brincar", "Eu quero brincar.", "Pedi BRINCAR!"),
        new("comer", "Comer", "Eu quero comer.", "Pedi COMER!"),
        new("ajuda", "Ajuda", "Eu preciso de ajuda.", "Pedi AJUDA!")
    };

    public static TipoNecessidade? Buscar(string chave) =>
        Todas.FirstOrDefault(n => n.Chave == chave);
}
