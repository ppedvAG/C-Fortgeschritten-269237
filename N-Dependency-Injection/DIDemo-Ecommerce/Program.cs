using Microsoft.Extensions.DependencyInjection;

static string Short(Guid id) => id.ToString()[..8];
static string Line()         => new string('─', 60);
static string Same(Guid a, Guid b) => a == b ? "✓ gleich  (selbe Instanz)" : "✗ anders  (neue Instanz)";

// ============================================================
//  DI-Container konfigurieren
//
//  Analogie zu echten Anwendungen:
//    Singleton  = AppConfiguration  → einmal laden, für alle gleich
//    Scoped     = ShoppingCart      → pro Kunde/Request ein eigener Warenkorb
//    Transient  = PriceCalculator   → zustandsloser Helfer, jedes Mal frisch
// ============================================================

var services = new ServiceCollection();

services.AddSingleton<IAppConfiguration, AppConfiguration>();  // App-weite Konfig
services.AddScoped   <IShoppingCart,     ShoppingCart>();      // Pro Session/Request
services.AddTransient<IPriceCalculator,  PriceCalculator>();   // Zustandslos, immer neu

services.AddTransient<CheckoutService>();   // wird pro Auflösung neu erstellt

ServiceProvider provider = services.BuildServiceProvider();


// ============================================================
//  SZENARIO 1 – Singleton
//
//  Die AppConfiguration wird einmal erzeugt.
//  Zwei unterschiedliche Auflösungen aus dem Root-Provider,
//  dann eine aus einem Scope → immer dieselbe Instanz.
// ============================================================

Console.WriteLine(Line());
Console.WriteLine("SINGLETON  –  AppConfiguration");
Console.WriteLine("(Konfig wird einmal beim Start geladen, nie neu erzeugt)");
Console.WriteLine(Line());

var cfg1 = provider.GetRequiredService<IAppConfiguration>();
var cfg2 = provider.GetRequiredService<IAppConfiguration>();

Console.WriteLine($"\n  cfg1 Id: {Short(cfg1.Id)}  StoreName: {cfg1.StoreName}  TaxRate: {cfg1.TaxRate:P0}");
Console.WriteLine($"  cfg2 Id: {Short(cfg2.Id)}");
Console.WriteLine($"\n  cfg1 == cfg2 ?  {Same(cfg1.Id, cfg2.Id)}");

using (var scope = provider.CreateScope())
{
    var cfgFromScope = scope.ServiceProvider.GetRequiredService<IAppConfiguration>();
    Console.WriteLine($"\n  cfgFromScope Id: {Short(cfgFromScope.Id)}");
    Console.WriteLine($"  cfgFromScope == cfg1 ?  {Same(cfgFromScope.Id, cfg1.Id)}");
}


// ============================================================
//  SZENARIO 2 – Scoped
//
//  Scope = eine Kunden-Session (in ASP.NET Core: ein HTTP-Request).
//  Innerhalb von Kunde 1 teilen beide CheckoutService-Instanzen
//  denselben Warenkorb → Artikel, die Service A legt, sieht auch Service B.
//  Kunde 2 bekommt einen komplett neuen, leeren Warenkorb.
// ============================================================

Console.WriteLine();
Console.WriteLine(Line());
Console.WriteLine("SCOPED  –  ShoppingCart");
Console.WriteLine("(Pro Session ein eigener Warenkorb)");
Console.WriteLine(Line());

Guid cartIdKunde1;

Console.WriteLine("\n  === Kunden-Session 1 ===");
using (var session1 = provider.CreateScope())
{
    // Zwei CheckoutServices innerhalb derselben Session
    var cs1a = session1.ServiceProvider.GetRequiredService<CheckoutService>();
    var cs1b = session1.ServiceProvider.GetRequiredService<CheckoutService>();

    cartIdKunde1 = cs1a.CartId;

    Console.WriteLine($"\n  cs1a legt Artikel in den Warenkorb:");
    cs1a.AddItem("Laptop",    899.00m);
    cs1a.AddItem("Maus",       29.90m);

    Console.WriteLine($"\n  cs1b legt Artikel in den Warenkorb:");
    cs1b.AddItem("USB-Hub",    19.99m);

    Console.WriteLine($"\n  cs1a sieht den Warenkorb:");
    cs1a.PrintCart();   // alle 3 Artikel – gleiche Instanz!

    Console.WriteLine($"\n  cs1b sieht den Warenkorb:");
    cs1b.PrintCart();   // ebenfalls alle 3 Artikel – gleiche Instanz!

    Console.WriteLine($"\n  cs1a.CartId == cs1b.CartId ?  {Same(cs1a.CartId, cs1b.CartId)}");
}

Console.WriteLine("\n  === Kunden-Session 2 (neue Session) ===");
using (var session2 = provider.CreateScope())
{
    var cs2 = session2.ServiceProvider.GetRequiredService<CheckoutService>();

    Console.WriteLine($"\n  cs2 sieht den Warenkorb (ohne Artikel von Kunde 1):");
    cs2.PrintCart();   // leer – neue Instanz!

    Console.WriteLine($"\n  cs2.CartId == cs1a.CartId (Session 1) ?  {Same(cs2.CartId, cartIdKunde1)}");
}


// ============================================================
//  SZENARIO 3 – Transient
//
//  Jeder CheckoutService erhält seinen eigenen PriceCalculator.
//  Er speichert keinen Zustand → kein Problem, dass es nie
//  dieselbe Instanz ist. Jede Rechnung ist unabhängig.
// ============================================================

Console.WriteLine();
Console.WriteLine(Line());
Console.WriteLine("TRANSIENT  –  PriceCalculator");
Console.WriteLine("(Zustandsloser Rechenhelfer, immer neu erzeugt)");
Console.WriteLine(Line());

var calc1 = provider.GetRequiredService<IPriceCalculator>();
var calc2 = provider.GetRequiredService<IPriceCalculator>();

Console.WriteLine($"\n  calc1 Id: {Short(calc1.Id)}   19% von 100,00 € = {calc1.GrossPrice(100m, 0.19m):F2} €");
Console.WriteLine($"  calc2 Id: {Short(calc2.Id)}   19% von 100,00 € = {calc2.GrossPrice(100m, 0.19m):F2} €");
Console.WriteLine($"\n  calc1 == calc2 ?  {Same(calc1.Id, calc2.Id)}");
Console.WriteLine("  (Ergebnis identisch – Instanz egal, weil kein Zustand)");


// ============================================================
//  SZENARIO 4 – Constructor Injection (alles zusammen)
//
//  Zwei CheckoutServices in derselben Session:
//    Transient  (PriceCalculator)  → unterschiedliche IDs
//    Scoped     (ShoppingCart)     → gleiche ID (gemeinsamer Warenkorb)
//    Singleton  (AppConfiguration) → gleiche ID (gleiche Konfig)
// ============================================================

Console.WriteLine();
Console.WriteLine(Line());
Console.WriteLine("CONSTRUCTOR INJECTION  –  alles zusammen");
Console.WriteLine("(Zwei CheckoutServices in einer Session)");
Console.WriteLine(Line());

using (var session = provider.CreateScope())
{
    var cs1 = session.ServiceProvider.GetRequiredService<CheckoutService>();
    var cs2 = session.ServiceProvider.GetRequiredService<CheckoutService>();

    cs1.PrintServiceIds("CheckoutService 1");
    cs2.PrintServiceIds("CheckoutService 2");

    Console.WriteLine();
    Console.WriteLine($"  AppConfig  cs1 == cs2 ?  {Same(cs1.ConfigId,     cs2.ConfigId)}");
    Console.WriteLine($"  Cart       cs1 == cs2 ?  {Same(cs1.CartId,       cs2.CartId)}");
    Console.WriteLine($"  Calculator cs1 == cs2 ?  {Same(cs1.CalculatorId, cs2.CalculatorId)}");
}

Console.WriteLine();
Console.WriteLine(Line());
