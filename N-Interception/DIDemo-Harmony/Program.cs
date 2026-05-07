// Castle.DynamicProxy – Interception
// Konzept: Proxy-Klasse wird zur Laufzeit generiert.
// Jeder Aufruf geht durch Interceptor(en) → Original kann
// ergänzt (Logging) oder blockiert (FraudCheck) werden.
//
// HarmonyLib macht dasselbe auf IL-Ebene (nativer Speicher-Patch),
// läuft aber nicht auf macOS .NET Core (mprotect Permission denied).
// Mapping: Prefix = vor Proceed(), Postfix = nach Proceed(), Cancel = kein Proceed()

using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

var gen = new ProxyGenerator();

// --- 1. Logging (Prefix + Postfix) ---
IPaymentService logged = gen.CreateInterfaceProxyWithTarget<IPaymentService>(
    new PaymentService(),
    new LoggingInterceptor()          // vor + nach jedem Aufruf
);
Console.WriteLine("Mit LoggingInterceptor:");
logged.ProcessPayment("ORDER-001", 250m);

// --- 2. FraudCheck (Cancel: Proceed() wird nicht aufgerufen) ---
IPaymentService guarded = gen.CreateInterfaceProxyWithTarget<IPaymentService>(
    new PaymentService(),
    new FraudCheckInterceptor()       // blockiert Refund > 1000 €
);
Console.WriteLine("\nMit FraudCheckInterceptor:");
guarded.Refund("ORDER-002", 200m);    // erlaubt
guarded.Refund("ORDER-003", 1500m);   // blockiert – Original läuft nicht

// --- 3. Pipeline: mehrere Interceptors in Reihe ---
IPaymentService combined = gen.CreateInterfaceProxyWithTarget<IPaymentService>(
    new PaymentService(),
    new LoggingInterceptor(),
    new FraudCheckInterceptor()
);
Console.WriteLine("\nLogging + FraudCheck kombiniert:");
combined.Refund("ORDER-004", 2000m);  // Logging feuert, FraudCheck blockiert

// --- 4. Im DI-Container (Factory-Lambda) ---
var services = new ServiceCollection();
services.AddSingleton<ProxyGenerator>();
services.AddTransient<PaymentService>();
services.AddTransient<IPaymentService>(sp =>
    sp.GetRequiredService<ProxyGenerator>()
      .CreateInterfaceProxyWithTarget<IPaymentService>(
          sp.GetRequiredService<PaymentService>(),
          new LoggingInterceptor()));

var provider = services.BuildServiceProvider();
Console.WriteLine("\nAus DI-Container (Aufrufer sieht nur IPaymentService):");
provider.GetRequiredService<IPaymentService>().ProcessPayment("ORDER-005", 99m);
