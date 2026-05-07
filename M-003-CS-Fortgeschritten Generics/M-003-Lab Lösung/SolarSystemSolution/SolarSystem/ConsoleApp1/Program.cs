class Program
{
    static void Main()
    {
        Node<Himmelskoerper> sonne = new(
            new Himmelskoerper("Sonne", HimmelskoerperTyp.Sonne));

        Node<Himmelskoerper> erde = new(
            new Himmelskoerper("Erde", HimmelskoerperTyp.Planet), sonne);

        Node<Himmelskoerper> mond = new(
            new Himmelskoerper("Mond", HimmelskoerperTyp.Mond), erde);

        Node<Himmelskoerper> mars = new(
            new Himmelskoerper("Mars", HimmelskoerperTyp.Planet), sonne);

        Node<Himmelskoerper> phobos = new(
            new Himmelskoerper("Phobos", HimmelskoerperTyp.Mond), mars);

        Node<Himmelskoerper> deimos = new(
            new Himmelskoerper("Deimos", HimmelskoerperTyp.Mond), mars);

        PrintTree(sonne);
    }

    static void PrintTree(Node<Himmelskoerper> node, int level = 0)
    {
        string indent = new(' ', level * 2);

        if (node.Parent == null)
        {
            Console.WriteLine($"{indent}{node.Value}");
        }
        else
        {
            Console.WriteLine($"{indent}{node.Value} umkreist {node.Parent.Value.Name}");
        }

        foreach (var child in node.Children)
        {
            PrintTree(child, level + 1);
        }
    }
}