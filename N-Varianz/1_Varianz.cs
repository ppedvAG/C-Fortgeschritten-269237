class Tier
{
}
class Hund : Tier
{
}
class Program
{
    static void Main(string[] args)
    {
        Tier t = new Hund();   // OK: Child to parent
        Hund h = new Tier();   // Compiler error: Cannot implicitly convert Tier to Hund
    }
}