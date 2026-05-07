namespace PluginBase;

/// <summary>
/// Basisinterface für alle Plugins.
/// Die Eigenschaften sind read-only – konkrete Werte werden im Plugin-Konstruktor gesetzt.
/// </summary>
public interface IPlugin
{
    /// <summary>Anzeigename des Plugins.</summary>
    string Name { get; }

    /// <summary>Kurze Beschreibung was das Plugin tut.</summary>
    string Description { get; }

    /// <summary>Versionsnummer des Plugins.</summary>
    string Version { get; }

    /// <summary>Startet die interaktive Ausführung des Plugins.</summary>
    void Execute();
}
