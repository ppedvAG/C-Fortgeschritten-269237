namespace PluginBase;

/// <summary>
/// Markiert Methoden oder Eigenschaften als öffentlich sichtbar für den Plugin-Client.
/// Methoden/Eigenschaften ohne dieses Attribut gelten als intern und werden ausgeblendet.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class PluginExportAttribute : Attribute
{
    /// <summary>Optionale Beschreibung der exportierten Methode/Eigenschaft.</summary>
    public string Description { get; }

    public PluginExportAttribute(string description = "")
    {
        Description = description;
    }
}
