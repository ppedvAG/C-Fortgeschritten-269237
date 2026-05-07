public class Himmelskoerper
{
    public string Name { get; set; }
    public HimmelskoerperTyp Typ { get; set; }

    public Himmelskoerper(string name, HimmelskoerperTyp typ)
    {
        Name = name;
        Typ = typ;
    }

    public override string ToString()
    {
        return $"{Name} ({Typ})";
    }
}