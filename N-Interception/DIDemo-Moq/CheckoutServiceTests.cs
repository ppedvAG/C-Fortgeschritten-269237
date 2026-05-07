using Moq;
using Xunit;

// Moq: Fake-Implementierungen von Interfaces.
// Setup()  = was soll zurückgegeben werden?
// Verify() = wurde die Methode aufgerufen?

public class CheckoutServiceTests
{
    private static (Mock<IAppConfiguration> cfg, Mock<IShoppingCart> cart, Mock<IPriceCalculator> calc, CheckoutService sut)
    Build(decimal taxRate = 0.19m)
    {
        var cfg  = new Mock<IAppConfiguration>();
        cfg.Setup(c => c.TaxRate).Returns(taxRate);

        var cart = new Mock<IShoppingCart>();
        cart.Setup(c => c.Items).Returns(new List<string>());

        var calc = new Mock<IPriceCalculator>();
        calc.Setup(c => c.GrossPrice(It.IsAny<decimal>(), It.IsAny<decimal>()))
            .Returns((decimal net, decimal tax) => Math.Round(net * (1 + tax), 2));

        return (cfg, cart, calc, new CheckoutService(cfg.Object, cart.Object, calc.Object));
    }

    // Verify: wurde cart.Add() mit dem richtigen Artikel aufgerufen?
    [Fact]
    public void AddItem_RuftCart_Add_Auf()
    {
        var (_, cart, _, sut) = Build();
        sut.AddItem("Laptop", 100m);
        cart.Verify(c => c.Add("Laptop"), Times.Once);
    }

    // Setup: Calculator-Mock gibt festen Wert zurück → Rückgabe prüfen
    [Fact]
    public void AddItem_GibtBruttoPreisZurueck()
    {
        var (_, _, calc, sut) = Build();
        calc.Setup(c => c.GrossPrice(100m, 0.19m)).Returns(119.00m);
        Assert.Equal(119.00m, sut.AddItem("Maus", 100m));
    }

    // Verify: Steuersatz aus Config landet beim Calculator
    [Fact]
    public void AddItem_UebergibtSteuersatzAusConfig()
    {
        var (_, _, calc, sut) = Build(taxRate: 0.07m);
        sut.AddItem("Buch", 20m);
        calc.Verify(c => c.GrossPrice(20m, 0.07m), Times.Once);
    }

    [Fact]
    public void AddItem_KeinNullSteuersatz()
    {
        var (_, _, calc, sut) = Build(taxRate: 0.19m);
        sut.AddItem("Tablet", 500m);
        calc.Verify(c => c.GrossPrice(It.IsAny<decimal>(), 0m), Times.Never);
    }

    // Exception aus einer Abhängigkeit wird weitergeleitet
    [Fact]
    public void AddItem_ExceptionWirdWeitergeleitet()
    {
        var (_, _, calc, sut) = Build();
        calc.Setup(c => c.GrossPrice(It.IsAny<decimal>(), It.IsAny<decimal>()))
            .Throws<InvalidOperationException>();
        Assert.Throws<InvalidOperationException>(() => sut.AddItem("X", 10m));
    }
}
