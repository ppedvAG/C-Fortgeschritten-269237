using Castle.DynamicProxy;

// ============================================================
//
//  Castle.DynamicProxy – Interceptors
//  ─────────────────────────────────────────────────────────
//  Statt [HarmonyPatch]-Attribute implementiert man IInterceptor.
//  Das Pendant zu HarmonyLib:
//
//    HarmonyLib Prefix  →  Code VOR  invocation.Proceed()
//    HarmonyLib Postfix →  Code NACH invocation.Proceed()
//    HarmonyLib Cancel  →  Proceed() einfach NICHT aufrufen
//
//  Castle generiert zur Laufzeit eine neue IL-Klasse (kein
//  nativer Speicher-Patch) → funktioniert auf macOS/Linux/Windows.
//
// ============================================================


// ── Interceptor 1: Logging (analog Harmony Prefix + Postfix) ─────────────────

public class LoggingInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        string args = string.Join(", ", invocation.Arguments.Select(a => a ?? "null"));

        // → analog Harmony Prefix
        Console.WriteLine($"  [Logging Prefix] Aufruf: {invocation.Method.Name}({args})");

        invocation.Proceed();   // ← Originalmethod ausführen

        // ← analog Harmony Postfix
        string result = invocation.ReturnValue is null
            ? "void"
            : invocation.ReturnValue.ToString()!;

        Console.WriteLine($"  [Logging Postfix] Ergebnis von {invocation.Method.Name}: {result}");
    }
}


// ── Interceptor 2: Betrugsprüfung bei Refund (analog Harmony Cancel) ─────────

public class FraudCheckInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        // Nur Refund-Aufrufe prüfen
        if (invocation.Method.Name == nameof(IPaymentService.Refund))
        {
            decimal amount = (decimal)invocation.Arguments[1];

            Console.WriteLine($"  [FraudCheck Prefix] Betrugsprüfung: Betrag={amount:F2} €");

            if (amount > 1000m)
            {
                Console.WriteLine($"  [FraudCheck] ✗ Abgelehnt – Betrag > 1000 €! Original wird NICHT ausgeführt.");
                // Proceed() wird NICHT aufgerufen → analog Harmony Prefix mit return false
                return;
            }

            Console.WriteLine($"  [FraudCheck] ✓ Prüfung bestanden.");
        }

        invocation.Proceed();   // Originalmethod ausführen
    }
}
