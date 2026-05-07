// ============================================================
//  CheckoutService – Constructor Injection
//
//  In einer echten Anwendung würde dieser Service z. B. von
//  einem Controller oder einem anderen Service verwendet.
//  Der DI-Container erkennt die Konstruktor-Parameter und
//  liefert automatisch die richtigen Instanzen herein –
//  niemand ruft „new CheckoutService(...)" manuell auf.
// ============================================================

public class CheckoutService
{
    private readonly IAppConfiguration _config;      // Singleton
    private readonly IShoppingCart     _cart;         // Scoped
    private readonly IPriceCalculator  _calculator;   // Transient

    public CheckoutService(
        IAppConfiguration config,
        IShoppingCart     cart,
        IPriceCalculator  calculator)
    {
        _config     = config;
        _cart       = cart;
        _calculator = calculator;
    }

    // IDs nach außen – damit Program.cs sie vergleichen kann
    public Guid ConfigId     => _config.Id;
    public Guid CartId       => _cart.Id;
    public Guid CalculatorId => _calculator.Id;

    /// <summary>
    /// Legt einen Artikel in den Warenkorb und berechnet den Brutto-Preis.
    /// </summary>
    public void AddItem(string name, decimal netPrice)
    {
        _cart.Add(name);
        decimal gross = _calculator.GrossPrice(netPrice, _config.TaxRate);
        Console.WriteLine($"    + {name,-20}  netto: {netPrice,6:F2} €  →  brutto: {gross,6:F2} €  (MwSt. {_config.TaxRate:P0})");
    }

    /// <summary>
    /// Zeigt alle Artikel im Warenkorb dieser Session.
    /// </summary>
    public void PrintCart()
    {
        Console.WriteLine($"    Warenkorb [{Short(_cart.Id)}]: " +
                          (_cart.Items.Count > 0 ? string.Join(", ", _cart.Items) : "(leer)"));
    }

    public void PrintServiceIds(string label)
    {
        Console.WriteLine($"\n  [{label}]");
        Console.WriteLine($"    AppConfig  Id (Singleton):  {Short(_config.Id)}");
        Console.WriteLine($"    Cart       Id (Scoped):     {Short(_cart.Id)}");
        Console.WriteLine($"    Calculator Id (Transient):  {Short(_calculator.Id)}");
    }

    private static string Short(Guid id) => id.ToString()[..8];
}
