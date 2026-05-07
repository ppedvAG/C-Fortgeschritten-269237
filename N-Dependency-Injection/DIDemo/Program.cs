using Microsoft.Extensions.DependencyInjection;

// Hilfsfunktionen
static string Short(Guid id) => id.ToString()[..8];   // nur die ersten 8 Zeichen der GUID
static string Line()         => new string('─', 55);
static string Same(Guid a, Guid b) => a == b ? "✓ gleich  (selbe Instanz)" : "✗ anders  (neue Instanz)";

// ============================================================
//  1. DI-Container aufbauen
//
//  ServiceCollection = Konfiguration (was gibt es?)
//  ServiceProvider   = fertiger Container (gibt Instanzen heraus)
// ============================================================

var services = new ServiceCollection();

//  Lebensdauer festlegen:
services.AddTransient<ITransientService,  TransientService>();   // Neu bei JEDER Anforderung
services.AddScoped   <IScopedService,     ScopedService>();      // Neu pro SCOPE
services.AddSingleton<ISingletonService,  SingletonService>();   // Einmal für die GESAMTE App

//  OrderProcessor selbst ist Transient (typisch für „Verarbeitungsklassen")
services.AddTransient<OrderProcessor>();

ServiceProvider provider = services.BuildServiceProvider();

// ============================================================
//  2. TRANSIENT
//     Neue Instanz bei jeder einzelnen Anforderung.
//     Einsatz: zustandslose Hilfsdienste (z. B. Mapper, Validator).
// ============================================================

Console.WriteLine(Line());
Console.WriteLine("TRANSIENT – Neue Instanz bei jeder Anforderung");
Console.WriteLine(Line());

var t1 = provider.GetRequiredService<ITransientService>();
var t2 = provider.GetRequiredService<ITransientService>();
var t3 = provider.GetRequiredService<ITransientService>();

Console.WriteLine($"\n  t1 Id: {Short(t1.Id)}");
Console.WriteLine($"  t2 Id: {Short(t2.Id)}");
Console.WriteLine($"  t3 Id: {Short(t3.Id)}");
Console.WriteLine($"\n  t1 == t2 ?  {Same(t1.Id, t2.Id)}");
Console.WriteLine($"  t1 == t3 ?  {Same(t1.Id, t3.Id)}");

// ============================================================
//  3. SCOPED
//     Innerhalb eines Scopes immer dieselbe Instanz.
//     Bei einem neuen Scope → neue Instanz.
//     Einsatz: Datenbankkontext (DbContext), Unit-of-Work.
// ============================================================

Console.WriteLine();
Console.WriteLine(Line());
Console.WriteLine("SCOPED – Gleich im Scope, neu bei neuem Scope");
Console.WriteLine(Line());

Guid idFromScope1;  // merken, um später mit Scope 2 zu vergleichen

using (var scope1 = provider.CreateScope())
{
    var s1a = scope1.ServiceProvider.GetRequiredService<IScopedService>();
    var s1b = scope1.ServiceProvider.GetRequiredService<IScopedService>();
    idFromScope1 = s1a.Id;

    Console.WriteLine($"\n  [Scope 1]");
    Console.WriteLine($"    s1a Id: {Short(s1a.Id)}");
    Console.WriteLine($"    s1b Id: {Short(s1b.Id)}");
    Console.WriteLine($"    s1a == s1b ?  {Same(s1a.Id, s1b.Id)}");   // gleich
}

using (var scope2 = provider.CreateScope())
{
    var s2a = scope2.ServiceProvider.GetRequiredService<IScopedService>();

    Console.WriteLine($"\n  [Scope 2]");
    Console.WriteLine($"    s2a Id: {Short(s2a.Id)}");
    Console.WriteLine($"    s2a == s1a (Scope 1) ?  {Same(s2a.Id, idFromScope1)}");   // anders
}

// ============================================================
//  4. SINGLETON
//     Immer dieselbe Instanz – egal wo und wie oft abgefragt.
//     Auch aus einem Scope heraus: gleiche Instanz.
//     Einsatz: Konfiguration, Caches, Logger-Factories.
// ============================================================

Console.WriteLine();
Console.WriteLine(Line());
Console.WriteLine("SINGLETON – Immer dieselbe Instanz (App-Lebensdauer)");
Console.WriteLine(Line());

var sg1 = provider.GetRequiredService<ISingletonService>();
var sg2 = provider.GetRequiredService<ISingletonService>();

Console.WriteLine($"\n  [Root-Provider]");
Console.WriteLine($"    sg1 Id: {Short(sg1.Id)}");
Console.WriteLine($"    sg2 Id: {Short(sg2.Id)}");
Console.WriteLine($"    sg1 == sg2 ?  {Same(sg1.Id, sg2.Id)}");   // gleich

using (var scope = provider.CreateScope())
{
    var sgFromScope = scope.ServiceProvider.GetRequiredService<ISingletonService>();

    Console.WriteLine($"\n  [Aus Scope heraus]");
    Console.WriteLine($"    sgFromScope Id: {Short(sgFromScope.Id)}");
    Console.WriteLine($"    sgFromScope == sg1 (Root) ?  {Same(sgFromScope.Id, sg1.Id)}");   // gleich
}

// ============================================================
//  5. CONSTRUCTOR INJECTION
//     So wird DI in der Praxis genutzt: Der Container erkennt
//     anhand des Konstruktors, welche Services gebraucht werden,
//     und injiziert sie automatisch.
//
//     Hier bekommen zwei OrderProcessor-Instanzen (beide Transient)
//     ihre Abhängigkeiten injiziert – innerhalb desselben Scopes.
//     → Transient:  jeder OrderProcessor hat eine andere Transient-ID
//     → Scoped:     beide teilen dieselbe Scoped-ID (gleicher Scope)
//     → Singleton:  beide haben dieselbe Singleton-ID (immer)
// ============================================================

Console.WriteLine();
Console.WriteLine(Line());
Console.WriteLine("CONSTRUCTOR INJECTION – OrderProcessor");
Console.WriteLine(Line());

using (var scope = provider.CreateScope())
{
    var op1 = scope.ServiceProvider.GetRequiredService<OrderProcessor>();
    var op2 = scope.ServiceProvider.GetRequiredService<OrderProcessor>();

    op1.PrintDependencies("OrderProcessor 1");
    op2.PrintDependencies("OrderProcessor 2");

    Console.WriteLine();
    Console.WriteLine($"  Transient  op1 == op2 ?  {Same(op1.TransientId, op2.TransientId)}");
    Console.WriteLine($"  Scoped     op1 == op2 ?  {Same(op1.ScopedId,    op2.ScopedId)}");
    Console.WriteLine($"  Singleton  op1 == op2 ?  {Same(op1.SingletonId, op2.SingletonId)}");
}

Console.WriteLine();
Console.WriteLine(Line());
