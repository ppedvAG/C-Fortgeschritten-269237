// Interfaces aus dem Ecommerce-Demo – hier wiederverwendet

public interface IAppConfiguration
{
    Guid    Id        { get; }
    string  StoreName { get; }
    decimal TaxRate   { get; }
}

public interface IShoppingCart
{
    Guid                  Id    { get; }
    IReadOnlyList<string> Items { get; }
    void Add(string item);
}

public interface IPriceCalculator
{
    Guid    Id { get; }
    decimal GrossPrice(decimal netPrice, decimal taxRate);
}

// ── Echte Implementierungen ────────────────────────────────

public class AppConfiguration : IAppConfiguration
{
    public Guid    Id        { get; } = Guid.NewGuid();
    public string  StoreName { get; } = "MyShop GmbH";
    public decimal TaxRate   { get; } = 0.19m;
}

public class ShoppingCart : IShoppingCart
{
    public Guid Id { get; } = Guid.NewGuid();
    private readonly List<string> _items = new();
    public IReadOnlyList<string> Items => _items;
    public void Add(string item) => _items.Add(item);
}

public class PriceCalculator : IPriceCalculator
{
    public Guid    Id { get; } = Guid.NewGuid();
    public decimal GrossPrice(decimal netPrice, decimal taxRate)
        => Math.Round(netPrice * (1 + taxRate), 2);
}

public class CheckoutService
{
    private readonly IAppConfiguration _config;
    private readonly IShoppingCart     _cart;
    private readonly IPriceCalculator  _calculator;

    public CheckoutService(
        IAppConfiguration config,
        IShoppingCart     cart,
        IPriceCalculator  calculator)
    {
        _config     = config;
        _cart       = cart;
        _calculator = calculator;
    }

    public decimal AddItem(string name, decimal netPrice)
    {
        _cart.Add(name);
        return _calculator.GrossPrice(netPrice, _config.TaxRate);
    }

    public IReadOnlyList<string> GetCartItems() => _cart.Items;
}
