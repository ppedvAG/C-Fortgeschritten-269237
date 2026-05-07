using Microsoft.Extensions.DependencyInjection;

// DispatchProxy: .NET generiert zur Laufzeit eine Proxy-Klasse.
// Jeder Methodenaufruf landet in Invoke() – der Aufrufer merkt nichts.

// --- 1. Direktes Wrapping ---
IPriceCalculator real  = new PriceCalculator();
IPriceCalculator proxy = LoggingProxy<IPriceCalculator>.Wrap(real);

Console.WriteLine("Ohne Proxy:");
real.GrossPrice(100m, 0.19m);

Console.WriteLine("Mit Proxy:");
proxy.GrossPrice(100m, 0.19m);   // → Invoke() fängt den Aufruf ab

// --- 2. Proxy im DI-Container (Praxis-Einsatz) ---
// Factory-Lambda: Container liefert immer den gewrappten Service.
// CheckoutService weiß nicht, dass ein Proxy dazwischen liegt.
var services = new ServiceCollection();
services.AddTransient<PriceCalculator>();
services.AddTransient<IPriceCalculator>(sp =>
    LoggingProxy<IPriceCalculator>.Wrap(sp.GetRequiredService<PriceCalculator>()));
services.AddSingleton<IAppConfiguration, AppConfiguration>();
services.AddScoped<IShoppingCart, ShoppingCart>();
services.AddTransient<CheckoutService>();

var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();
var checkout = scope.ServiceProvider.GetRequiredService<CheckoutService>();

Console.WriteLine("\nCheckoutService aus Container (PriceCalculator ist intern ein Proxy):");
checkout.AddItem("Laptop", 899m);
