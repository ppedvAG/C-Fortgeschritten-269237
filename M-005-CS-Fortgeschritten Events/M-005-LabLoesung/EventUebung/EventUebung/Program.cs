
PrimeComponent pc = new PrimeComponent();
pc.Prime += (sender, i) => Console.WriteLine($"Primzahl: {i}");
pc.Prime100 += (sender, i) => Console.WriteLine($"Hundertste Primzahl: {i}");
pc.NotPrime += (sender, args) => Console.WriteLine($"Keine Primzahl: {args.Item1}, teilbar durch {args.Item2}");
pc.StartProcess();

int methode()
{
    return 1;
}