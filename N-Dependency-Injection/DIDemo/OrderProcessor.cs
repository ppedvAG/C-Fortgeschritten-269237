// ============================================================
//  Constructor Injection – der normale Weg in der Praxis.
//
//  Der DI-Container schaut sich den Konstruktor an,
//  erkennt die benötigten Typen und liefert die passenden
//  Instanzen automatisch herein. Man muss „new" nie selbst
//  aufrufen.
// ============================================================

public class OrderProcessor
{
    private readonly ITransientService  _transient;
    private readonly IScopedService     _scoped;
    private readonly ISingletonService  _singleton;

    // Der Container ruft diesen Konstruktor auf und injiziert
    // die drei Services automatisch.
    public OrderProcessor(
        ITransientService  transient,
        IScopedService     scoped,
        ISingletonService  singleton)
    {
        _transient = transient;
        _scoped    = scoped;
        _singleton = singleton;
    }

    // IDs nach außen freigeben, damit Program.cs sie vergleichen kann
    public Guid TransientId  => _transient.Id;
    public Guid ScopedId     => _scoped.Id;
    public Guid SingletonId  => _singleton.Id;

    public void PrintDependencies(string label)
    {
        Console.WriteLine($"\n  [{label}]");
        Console.WriteLine($"    Transient  Id: {Short(_transient.Id)}");
        Console.WriteLine($"    Scoped     Id: {Short(_scoped.Id)}");
        Console.WriteLine($"    Singleton  Id: {Short(_singleton.Id)}");
    }

    private static string Short(Guid id) => id.ToString()[..8];
}
