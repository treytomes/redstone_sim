namespace RedstoneSimulator.Core.Components;

/// <summary>
/// Represents serialized component data.
/// </summary>
public class ComponentData
{
    /// <summary>
    /// Gets or sets the type of the component.
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets additional properties for the component.
    /// </summary>
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
}
