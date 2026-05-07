using System.Drawing;
using System.Drawing.Imaging;
using PluginBase;

namespace GrayscalePlugin;

/// <summary>
/// Plugin zum Umwandeln einer Bilddatei in ein Graustufenbild.
/// Benötigt NuGet-Paket: System.Drawing.Common
/// </summary>
public class GrayscaleConverterPlugin : IPlugin
{
    public string Name        => "Grayscale Converter";
    public string Description => "Wandelt ein Farbbild in ein Graustufenbild um";
    public string Version     => "1.0.0";

    // ---------------------------------------------------------------
    // Interne Hilfsmethode – kein [PluginExport]
    // ---------------------------------------------------------------
    private static ImageFormat ResolveFormat(string extension)
        => extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => ImageFormat.Jpeg,
            ".bmp"            => ImageFormat.Bmp,
            ".gif"            => ImageFormat.Gif,
            _                 => ImageFormat.Png
        };

    // ---------------------------------------------------------------
    // Öffentlich exportierte Methoden
    // ---------------------------------------------------------------

    [PluginExport("Konvertiert ein Farbbild pixel-weise in Graustufen und speichert es unter outputPath")]
    public void ConvertToGrayscale(string inputPath, string outputPath)
    {
        using var original = new Bitmap(inputPath);
        using var grayscale = new Bitmap(original.Width, original.Height);

        for (int y = 0; y < original.Height; y++)
        {
            for (int x = 0; x < original.Width; x++)
            { 
                Color pixel = original.GetPixel(x, y);
                // Luminanz-gewichtete Formel (ITU-R BT.601)
                int lum = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
                grayscale.SetPixel(x, y, Color.FromArgb(pixel.A, lum, lum, lum));
            }
        }

        grayscale.Save(outputPath, ResolveFormat(Path.GetExtension(outputPath)));
    }

    public void Execute()
    {
        Console.WriteLine("\n=== Grayscale Converter Plugin ===");
        Console.Write("Pfad zur Bilddatei eingeben: ");
        string inputPath = (Console.ReadLine() ?? "").Trim();

        if (!File.Exists(inputPath))
        {
            Console.WriteLine("Datei nicht gefunden. Abbruch.");
            return;
        }

        string ext        = Path.GetExtension(inputPath);
        string baseName   = Path.GetFileNameWithoutExtension(inputPath);
        string dir        = Path.GetDirectoryName(inputPath) ?? ".";
        string outputPath = Path.Combine(dir, $"{baseName}_grau{ext}");

        try
        {
            ConvertToGrayscale(inputPath, outputPath);
            Console.WriteLine($"Graustufenbild gespeichert: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler: {ex.Message}");
        }
    }
}
