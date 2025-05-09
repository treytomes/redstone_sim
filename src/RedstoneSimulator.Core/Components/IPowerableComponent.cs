namespace RedstoneSimulator.Core.Components;

/// <summary>
/// Interface for components that can be powered.
/// </summary>
public interface IPowerableComponent : IComponent
{
    /// <summary>
    /// Gets or sets the power level of this component (0-15).
    /// </summary>
    new int PowerLevel { get; set; }
}
