using System.Text.Json.Serialization;
using System.Xml.Serialization;

public class Fahrzeug
{
    public int ID;

    public int MaxGeschwindigkeit { get; set; }

    public FahrzeugMarke Marke;
    
    public Fahrzeug(int id, int v, FahrzeugMarke fm)
    {
        ID = id;
        MaxGeschwindigkeit = v;
        Marke = fm;
    }

    public Fahrzeug() { }
}

public enum FahrzeugMarke { Audi = 1, BMW = 2, VW = 3 }