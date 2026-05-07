// ============================================================
//  Basis-Interface
//  Jede Service-Instanz bekommt im Konstruktor eine neue GUID.
//  Anhand der ID sehen wir sofort, ob eine neue Instanz
//  erstellt wurde oder ob dieselbe wiederverwendet wird.
// ============================================================

public interface IService
{
    Guid Id { get; }
}

// Marker-Interfaces für die drei Lebensdauern
public interface ITransientService : IService { }
public interface IScopedService    : IService { }
public interface ISingletonService : IService { }


// ============================================================
//  Implementierungen
//  Alle identisch – der Unterschied liegt nur in der
//  Registrierung im DI-Container (AddTransient / AddScoped /
//  AddSingleton).
// ============================================================

public class TransientService : ITransientService
{
    // Neue GUID im Konstruktor → bei jeder neuen Instanz andere ID
    public Guid Id { get; } = Guid.NewGuid();
}

public class ScopedService : IScopedService
{
    public Guid Id { get; } = Guid.NewGuid();
}

public class SingletonService : ISingletonService
{
    public Guid Id { get; } = Guid.NewGuid();
}
