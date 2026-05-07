namespace TPL_Uebung;

/// <summary>
/// Die Scanner Klasse überwacht kontinuierlich einen Ordner auf neue Bilder.
/// Neu gefundene Dateipfade werden in die gemeinsame ConcurrentQueue eingereiht.
/// Bereits bekannte Dateien werden über ein HashSet dedupliziert.
/// </summary>
public class Scanner
{
    // Der Ordner, den dieser Scanner überwacht
    public string ScanPath { get; private set; }

    // CancellationTokenSource — dient als "Schalter" zum Starten und Stoppen
    private CancellationTokenSource? _cts;

    // Merkt sich bereits gefundene Dateipfade, damit keine Datei doppelt eingereiht wird
    private readonly HashSet<string> _knownFiles = new();

    public Scanner(string scanPath)
    {
        ScanPath = scanPath;
    }

    /// <summary>
    /// Startet den internen Scan-Task. Läuft bereits ein Task, passiert nichts.
    /// </summary>
    public void Start()
    {
        // Neuen CancellationTokenSource anlegen (alter Token ist evtl. schon gecancelt)
        _cts = new CancellationTokenSource();
        CancellationToken token = _cts.Token;

        Task.Run(() =>
        {
            Console.WriteLine($"[Scanner] Starte: {ScanPath}");

            while (!token.IsCancellationRequested)
            {
                // Alle aktuellen Dateien im Ordner lesen
                foreach (string filePath in Directory.GetFiles(ScanPath))
                {
                    // Nur neue Dateien in die Queue einreihen — HashSet.Add gibt false zurück wenn schon vorhanden
                    if (!Program.ProcessedImages.Contains(filePath) &&
                        !Program.ImagePathQueue.Contains(filePath))
                    {
                        Program.ImagePathQueue.Enqueue(filePath);
                        Console.WriteLine($"[Scanner] Neu gefunden: {Path.GetFileName(filePath)}");
                    }
                }

                // Kurze Pause, damit der Task nicht dauerhaft die CPU belastet
                Thread.Sleep(1000);
            }

            Console.WriteLine($"[Scanner] Gestoppt: {ScanPath}");
        }, token);
    }

    /// <summary>
    /// Pausiert den Scan-Task durch Abbruch des CancellationToken.
    /// </summary>
    public void Pause()
    {
        _cts?.Cancel();
    }
}