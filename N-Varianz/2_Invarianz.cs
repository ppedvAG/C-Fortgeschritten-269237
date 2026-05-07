using System;

class Tier
{
    public string Name { get; set; } = string.Empty;
}

class Hund : Tier
{
    public string ChipNummer { get; set; } = string.Empty;
}


// Kovarianz und Kontravarianz kommen immer, 
//wenn wir ueber andere Typen nachdenken, die von Typen wie Tier und Hund abhaengen. Schauen wir uns dazu einige Beispiele an.


// Schritt 1:
// Weil T sowohl als Eingabe als auch als Rueckgabewert benutzt wird,
// ist das Interface invariant.
interface IPatientenakte<T>
{
    void Speichere(T patient);
    T LadeLetztenEintrag();
}

class HundeAkte : IPatientenakte<Hund>
{
    private Hund? _letzterEintrag;

    public void Speichere(Hund patient)
    {
        _letzterEintrag = patient;
    }

    public Hund LadeLetztenEintrag()
    {
        return _letzterEintrag ?? new Hund { Name = "Unbekannt", ChipNummer = "n/a" };
    }
}

class Program
{
    static void Main(string[] args)
    {
        IPatientenakte<Hund> hundeAkte = new HundeAkte();
        hundeAkte.Speichere(new Hund
        {
            Name = "Bello",
            ChipNummer = "DE-4711"
        });

        Hund letzterHund = hundeAkte.LadeLetztenEintrag();
        Console.WriteLine($"Patient: {letzterHund.Name}, Chip: {letzterHund.ChipNummer}");

        // Nicht erlaubt (invariant):
        // IPatientenakte<Tier> tierAkte = IPatientenakte<Hund> hundeAkte;

        //WARUM?

        // Dann dürfte man über tierAkte ein beliebiges Tier speichern:
        // tierAkte.Speichere(new Tier { Name = "Irgendein Tier" });

        //Aber laufzeit: ist es weiterhin eine HundeAkte, die eigentlich nur Hund erwartet.

        //Folge:

        // Hund letzterHund2 = hundeAkte.LadeLetztenEintrag(); // Laufzeitfehler: da ein Tier kein Hund ist


    }
}


