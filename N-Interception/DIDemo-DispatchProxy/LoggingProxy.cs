using System.Reflection;

// ============================================================
//
//  DispatchProxy – Transparenter Laufzeit-Proxy
//  ─────────────────────────────────────────────────────────
//  .NET erzeugt zur Laufzeit eine neue Klasse, die T (ein Interface)
//  implementiert. Jeder Methodenaufruf landet in Invoke().
//  Der originale Service wird dort aufgerufen – der Aufrufer
//  bemerkt den Proxy überhaupt nicht.
//
//  Typische Anwendungsfälle:
//    • Logging  (jeder Aufruf wird protokolliert)
//    • Caching  (Ergebnis beim ersten Aufruf speichern)
//    • Retry    (bei Exception automatisch wiederholen)
//    • Timing   (wie lange dauert ein Aufruf?)
//
// ============================================================

public class LoggingProxy<T> : DispatchProxy
{
    private T _target = default!;   // die echte Implementierung dahinter

    // ── Fabrikmethode ─────────────────────────────────────────────────────────
    // Create<T, LoggingProxy<T>>() ist die .NET-interne Methode, die den
    // Proxy zur Laufzeit generiert. Wir hängen danach nur noch das Zielobjekt an.
    public static T Wrap(T target)
    {
        var proxy = Create<T, LoggingProxy<T>>();
        ((LoggingProxy<T>)(object)proxy)._target = target;
        return proxy;
    }

    // ── Herzstück: wird für JEDEN Methodenaufruf auf dem Proxy ausgeführt ─────
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        string argStr = args is { Length: > 0 }
            ? string.Join(", ", args.Select(a => a ?? "null"))
            : "–";

        Console.WriteLine($"  → [{typeof(T).Name}] {targetMethod!.Name}({argStr})");

        var sw = System.Diagnostics.Stopwatch.StartNew();

        // Echten Service aufrufen
        object? result = targetMethod.Invoke((object?)_target, args);

        sw.Stop();

        string resultStr = result switch
        {
            null              => "void/null",
            IEnumerable<string> list => $"[{string.Join(", ", list)}]",
            _                 => result.ToString()!
        };

        Console.WriteLine($"  ← Ergebnis: {resultStr}  ({sw.ElapsedMilliseconds} ms)");
        return result;
    }
}
