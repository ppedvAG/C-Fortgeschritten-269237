using System;

class Tier
{
    public string Name { get; set; } = string.Empty;
}

class Hund : Tier
{
    public string ChipNummer { get; set; } = string.Empty;
}


// Schritt 2: Kovarianz
// out T bedeutet: T wird nur ausgegeben (Rueckgabewert), nie als Eingabe benutzt.
// Dadurch gilt: Wenn Hund : Tier, dann ist IAktenLeser<Hund> kompatibel zu IAktenLeser<Tier>.
interface IAktenLeser<out T>
{
    T LadeLetztenEintrag();

    // Fehlerfall (waere Compilerfehler):
    // void Speichere(T patient);
    // Warum nicht?
    // Bei out T darf T nicht als Eingabeparameter vorkommen.
    // Was waere sonst?
    // Dann koennte ein eigentlich "nur-lesendes" Interface ploetzlich Werte
    // eines unpassenden Typs annehmen, und Kovarianz waere nicht mehr typsicher.
}

class HundeAkteLeser : IAktenLeser<Hund>
{
    public Hund LadeLetztenEintrag()
    {
        return new Hund
        {
            Name = "Bello",
            ChipNummer = "DE-4711"
        };
    }
}

class Program
{
    static void Main(string[] args)
    {
        IAktenLeser<Hund> hundeLeser = new HundeAkteLeser();

        // Erlaubt (kovariant):
        IAktenLeser<Tier> tierLeser = hundeLeser;

        // Fehlerfall (Compilerfehler):
        // IAktenLeser<Hund> nurHundeLeser = tierLeser;
        // Warum nicht?
        // Ein IAktenLeser<Tier> koennte auch eine Katze liefern.
        // Das ist nicht sicher als IAktenLeser<Hund> verwendbar.
        // Was waere sonst?
        // Der Aufrufer erwartet einen Hund, bekommt aber evtl. eine Katze.
        // Spaetestens beim Zugriff auf Hund-spezifische Eigenschaften waere das ein Laufzeitproblem.

        Tier tier = tierLeser.LadeLetztenEintrag();
        Console.WriteLine($"Gelesen: {tier.Name}");
    }
}
