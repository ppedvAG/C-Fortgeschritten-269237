using System.Collections;
using System.Collections.Generic;

Main();
//Main
void Main()
{
    Kartendeck deck = new Kartendeck();
    // 2. Wir können jetzt mit foreach über unser Kartendeck iterieren, obwohl es gar keine echte Liste ist!
    // foreach (string karte in deck)
    // {
    //     Console.WriteLine(karte);
    // }

    Console.WriteLine(deck["1"]);


}

// 1. Klasse erbt von IEnumerable
public class Kartendeck : IEnumerable<string>
{
    // Die echten Daten sind privat versteckt!
    private List<string> _karten = new List<string> { "Ass", "König", "Dame", "Bube" };

    // --- DER INDEXER ---
    // Das Schlüsselwort "this" in Kombination mit eckigen Klammern macht den Indexer
    public string this[int index]
    {
        get { return _karten[index]; }
        set { _karten[index] = value; }
    }

    public string this[string index]
    {
        get { return _karten[int.Parse(index)]; }
        set { _karten[int.Parse(index)] = value; }
    }

    // --- DAS IENUMERABLE (Iterierbar machen) ---
    // Wir reichen einfach das Lesezeichen unserer internen Liste nach außen durch
    public IEnumerator<string> GetEnumerator()
    {
        return _karten.GetEnumerator();
    }

    // (Notwendiges Übel für ältere C#-Versionen, muss man fast immer mit einbauen)
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}