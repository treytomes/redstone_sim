namespace RedstoneSimulator.Core.Components;

/// <summary>
/// Interface for components that can be activated.
/// </summary>
public interface IActivatableComponent : IComponent
{
    /// <summary>
    /// Activates this component.
    /// </summary>
    void Activate();
}