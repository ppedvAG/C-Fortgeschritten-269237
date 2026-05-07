using PluginBase;

namespace DownloadPlugin;

/// <summary>
/// Plugin zum Herunterladen von Dateien aus dem Internet über HttpClient.
/// </summary>
public class FileDownloadPlugin : IPlugin
{
    public string Name        => "File Downloader";
    public string Description => "Lädt Dateien aus dem Internet herunter und speichert sie lokal";
    public string Version     => "1.0.0";

    // ---------------------------------------------------------------
    // Interne Hilfsmethode – kein [PluginExport]
    // ---------------------------------------------------------------
    private static string DeriveFileName(string url)
    {
        try
        {
            string path = new Uri(url).LocalPath;
            string name = Path.GetFileName(path);
            return string.IsNullOrWhiteSpace(name) ? "download" : name;
        }
        catch
        {
            return "download";
        }
    }

    // ---------------------------------------------------------------
    // Öffentlich exportierte Methoden
    // ---------------------------------------------------------------

    [PluginExport("Lädt eine Datei von der angegebenen URL und gibt den Inhalt als Byte-Array zurück")]
    public async Task<byte[]> DownloadBytesAsync(string url)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("PluginDownloader/1.0");
        return await client.GetByteArrayAsync(url);
    }

    [PluginExport("Lädt eine Datei von der URL und speichert sie unter outputPath")]
    public async Task DownloadToFileAsync(string url, string outputPath)
    {
        byte[] data = await DownloadBytesAsync(url);
        await File.WriteAllBytesAsync(outputPath, data);
    }

    public void Execute()
    {
        Console.WriteLine("\n=== File Downloader Plugin ===");
        Console.Write("URL eingeben: ");
        string url = (Console.ReadLine() ?? "").Trim();

        if (string.IsNullOrWhiteSpace(url))
        {
            Console.WriteLine("Keine URL angegeben. Abbruch.");
            return;
        }

        string fileName = DeriveFileName(url);
        string outputPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

        Console.WriteLine($"Lade herunter: {url}");
        try
        {
            DownloadToFileAsync(url, outputPath).GetAwaiter().GetResult();
            Console.WriteLine($"Gespeichert unter: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Herunterladen: {ex.Message}");
        }
    }
}
