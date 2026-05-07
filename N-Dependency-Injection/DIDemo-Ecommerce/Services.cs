// ============================================================
//
//  SINGLETON  →  IAppConfiguration
//  ─────────────────────────────────────────────────────────
//  Wird einmal beim App-Start geladen (z. B. aus appsettings.json).
//  Steuersatz, Store-Name usw. sind für alle Anfragen gleich.
//  → Immer dieselbe Instanz, egal wer fragt.
//
//  SCOPED  →  IShoppingCart
//  ─────────────────────────────────────────────────────────
//  Ein Warenkorb pro Kunden-Session (in ASP.NET: pro HTTP-Request).
//  Alle Services innerhalb derselben Session teilen denselben Warenkorb.
//  → Neue Instanz pro Scope (= pro Kunde / pro Request).
//
//  TRANSIENT  →  IPriceCalculator
//  ─────────────────────────────────────────────────────────
//  Zustandsloser Rechenhelfer. Braucht keinen gemeinsamen Zustand.
//  Wird frisch erzeugt, rechnet, und wird weggeworfen.
//  → Neue Instanz bei jeder Anforderung.
//
// ============================================================


// ── Interfaces ──────────────────────────────────────────────

public interface IAppConfiguration
{
    Guid   Id        { get; }   // zur Identitätsprüfung
    string StoreName { get; }
    decimal TaxRate  { get; }   // 0.19 = 19 %
}

public interface IShoppingCart
{
    Guid          Id    { get; }   // zur Identitätsprüfung
    IReadOnlyList<string> Items { get; }
    void Add(string item);
}

public interface IPriceCalculator
{
    Guid    Id      { get; }   // zur Identitätsprüfung
    decimal GrossPrice(decimal netPrice, decimal taxRate);
}


// ── Implementierungen ────────────────────────────────────────

/// <summary>
/// Singleton: einmal erzeugt, enthält App-weite Einstellungen.
/// </summary>
public class AppConfiguration : IAppConfiguration
{
    public Guid    Id        { get; } = Guid.NewGuid();
    public string  StoreName { get; } = "MyShop GmbH";
    public decimal TaxRate   { get; } = 0.19m;   // 19 % MwSt.
}

/// <summary>
/// Scoped: pro Kunden-Session ein frischer, leerer Warenkorb.
/// </summary>
public class ShoppingCart : IShoppingCart
{
    public Guid Id { get; } = Guid.NewGuid();

    private readonly List<string> _items = new();
    public IReadOnlyList<string> Items => _items;

    public void Add(string item) => _items.Add(item);
}

/// <summary>
/// Transient: zustandsloser Preisrechner – wird immer neu erzeugt.
/// </summary>
public class PriceCalculator : IPriceCalculator
{
    public Guid Id { get; } = Guid.NewGuid();

    public decimal GrossPrice(decimal netPrice, decimal taxRate)
        => Math.Round(netPrice * (1 + taxRate), 2);
}
