using System.Reflection;
using PluginBase;

// ---------------------------------------------------------------
// Plugin-Ordner bestimmen
// ---------------------------------------------------------------
string pluginsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

if (!Directory.Exists(pluginsFolder))
{
    Directory.CreateDirectory(pluginsFolder);
    Console.WriteLine($"Plugins-Ordner wurde erstellt: {pluginsFolder}");
    Console.WriteLine("Lege die Plugin-DLLs dort ab und starte das Programm erneut.");
    Console.ReadLine();
    return;
}

// ---------------------------------------------------------------
// Plugins laden
// ---------------------------------------------------------------
List<IPlugin> plugins = PluginLoader.LoadFromFolder(pluginsFolder);

if (plugins.Count == 0)
{
    Console.WriteLine($"Keine Plugins im Ordner '{pluginsFolder}' gefunden.");
    Console.ReadLine();
    return;
}

// ---------------------------------------------------------------
// Hauptmenü
// ---------------------------------------------------------------
while (true)
{
    Console.WriteLine("\n========================================");
    Console.WriteLine("  Plugin Client");
    Console.WriteLine("========================================");

    for (int i = 0; i < plugins.Count; i++)
    {
        IPlugin p = plugins[i];
        Console.WriteLine($"  [{i + 1}]  {p.Name,-22} v{p.Version}  –  {p.Description}");
    }

    Console.WriteLine("  [0]  Beenden");
    Console.Write("\nAuswahl: ");

    if (!int.TryParse(Console.ReadLine(), out int choice) || choice == 0)
        break;

    if (choice < 1 || choice > plugins.Count)
    {
        Console.WriteLine("Ungültige Auswahl.");
        continue;
    }

    IPlugin selected = plugins[choice - 1];
    PluginInspector.PrintExportedMembers(selected);
    selected.Execute();
}

Console.WriteLine("Auf Wiedersehen!");


// ---------------------------------------------------------------
// Hilfklasse: Plugins per Reflection aus DLLs laden
// ---------------------------------------------------------------
static class PluginLoader
{
    public static List<IPlugin> LoadFromFolder(string folder)
    {
        var plugins = new List<IPlugin>();

        foreach (string dllPath in Directory.GetFiles(folder, "*.dll"))
        {
            try
            {
                Assembly asm = Assembly.LoadFrom(dllPath);

                foreach (Type type in asm.GetTypes())
                {
                    // Nur konkrete Klassen, die IPlugin implementieren
                    if (!type.IsAbstract && !type.IsInterface && typeof(IPlugin).IsAssignableFrom(type))
                    {
                        if (Activator.CreateInstance(type) is IPlugin plugin)
                            plugins.Add(plugin);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] Konnte '{Path.GetFileName(dllPath)}' nicht laden: {ex.Message}");
            }
        }

        return plugins;
    }
}

// ---------------------------------------------------------------
// Hilfklasse: Exportierte Methoden/Eigenschaften anzeigen
// ---------------------------------------------------------------
static class PluginInspector
{
    public static void PrintExportedMembers(IPlugin plugin)
    {
        Console.WriteLine($"\n--- Exportierte Member von '{plugin.Name}' ---");

        Type type = plugin.GetType();

        // Exportierte Methoden
        var methods = type
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttribute<PluginExportAttribute>() != null);

        foreach (MethodInfo m in methods)
        {
            string desc = m.GetCustomAttribute<PluginExportAttribute>()!.Description;
            Console.WriteLine($"  Methode:    {m.Name}()  –  {desc}");
        }

        // Exportierte Properties
        var props = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(p => p.GetCustomAttribute<PluginExportAttribute>() != null);

        foreach (PropertyInfo p in props)
        {
            string desc = p.GetCustomAttribute<PluginExportAttribute>()!.Description;
            Console.WriteLine($"  Eigenschaft: {p.Name}  –  {desc}");
        }

        Console.WriteLine();
    }
}
