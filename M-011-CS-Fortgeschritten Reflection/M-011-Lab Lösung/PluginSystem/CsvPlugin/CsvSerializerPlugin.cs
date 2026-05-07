using System.Reflection;
using System.Text;
using PluginBase;

namespace CsvPlugin;

/// <summary>
/// Plugin zum Serialisieren von Objekten in das CSV-Format.
/// Nutzt Reflection, um alle öffentlichen Eigenschaften eines Objekts auszulesen.
/// </summary>
public class CsvSerializerPlugin : IPlugin
{
    public string Name        => "CSV Serializer";
    public string Description => "Serialisiert Objekte mittels Reflection in das CSV-Format";
    public string Version     => "1.0.0";

    // ---------------------------------------------------------------
    // Interne Hilfsmethode – kein [PluginExport], wird ausgeblendet
    // ---------------------------------------------------------------
    private static PropertyInfo[] GetProperties(object obj)
        => obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

    // ---------------------------------------------------------------
    // Öffentlich exportierte Methoden
    // ---------------------------------------------------------------

    [PluginExport("Erzeugt die CSV-Kopfzeile (Spaltennamen) aus den Properties eines Objekts")]
    public string GetCsvHeader(object obj)
    {
        var props = GetProperties(obj);
        return string.Join(";", props.Select(p => p.Name));
    }

    [PluginExport("Serialisiert ein Objekt in eine CSV-Datenzeile")]
    public string SerializeToCsv(object obj)
    {
        var props = GetProperties(obj);
        var values = props.Select(p =>
        {
            string? value = p.GetValue(obj)?.ToString() ?? string.Empty;
            // Enthält der Wert ein Semikolon oder Anführungszeichen, in Anführungszeichen einschließen
            if (value.Contains(';') || value.Contains('"'))
                value = $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        });
        return string.Join(";", values);
    }

    [PluginExport("Serialisiert eine Liste von Objekten (inkl. Kopfzeile) in einen CSV-String")]
    public string SerializeListToCsv<T>(IEnumerable<T> items)
    {
        var list = items.ToList();
        if (list.Count == 0) return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine(GetCsvHeader(list[0]!));
        foreach (var item in list)
            sb.AppendLine(SerializeToCsv(item!));
        return sb.ToString();
    }

    public void Execute()
    {
        Console.WriteLine("\n=== CSV Serializer Plugin ===");
        Console.Write("Name eingeben:  ");
        string name = Console.ReadLine() ?? "Max Mustermann";

        Console.Write("Alter eingeben: ");
        int.TryParse(Console.ReadLine(), out int age);

        Console.Write("Stadt eingeben: ");
        string city = Console.ReadLine() ?? "Berlin";

        // Anonymes Objekt – Reflection liest alle Properties zur Laufzeit
        var person = new { Name = name, Alter = age, Stadt = city, Datum = DateTime.Now.ToShortDateString() };

        Console.WriteLine("\n--- CSV-Ausgabe ---");
        Console.WriteLine(GetCsvHeader(person));
        Console.WriteLine(SerializeToCsv(person));
    }
}
