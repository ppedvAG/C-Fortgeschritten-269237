using System;

class Tier
{
    public string Name { get; set; } = string.Empty;
}

class Hund : Tier
{
    public string ChipNummer { get; set; } = string.Empty;
}


// Schritt 3: Kontravarianz
// in T bedeutet: T wird nur angenommen (Parameter), nie als Rueckgabewert benutzt.
// Dadurch gilt: Wenn Hund : Tier, dann ist IAktenSchreiber<Tier> kompatibel zu IAktenSchreiber<Hund>.
interface IAktenSchreiber<in T>
{
    void Speichere(T patient);

    // Fehlerfall (waere Compilerfehler):
    // T LadeLetztenEintrag();
    // Warum nicht?
    // Bei in T darf T nicht als Rueckgabewert vorkommen.
    // Was waere sonst?
    // Dann muesste ein "nur-schreibendes" Interface ploetzlich konkrete T-Werte liefern,
    // obwohl die konkrete Implementierung dafuer evtl. gar nicht den richtigen Typ garantieren kann.
}

class TierAkteSchreiber : IAktenSchreiber<Tier>
{
    public void Speichere(Tier patient)
    {
        Console.WriteLine($"Gespeichert: {patient.Name}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        IAktenSchreiber<Tier> tierSchreiber = new TierAkteSchreiber();

        // Erlaubt (kontravariant):
        IAktenSchreiber<Hund> hundSchreiber = tierSchreiber;

        // Fehlerfall (Compilerfehler):
        // IAktenSchreiber<Tier> alleTiereSchreiber = hundSchreiber;
        // Warum nicht?
        // Ein IAktenSchreiber<Hund> akzeptiert nur Hunde, aber kein beliebiges Tier.
        // Was waere sonst?
        // Dann duerfte man z.B. eine Katze an alleTiereSchreiber uebergeben,
        // die konkrete Hund-Implementierung kann das aber nicht verarbeiten.

        hundSchreiber.Speichere(new Hund
        {
            Name = "Luna",
            ChipNummer = "DE-9999"
        });
    }
}
