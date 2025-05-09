using RedstoneSimulator.Core.Common;

namespace RedstoneSimulator.Core.Components;

/// <summary>
/// Interface for components that can connect to other components.
/// </summary>
public interface IConnectableComponent : IComponent
{
    /// <summary>
    /// Determines if this component can connect to another component in the specified direction.
    /// </summary>
    /// <param name="direction">The direction to check.</param>
    /// <param name="otherComponent">The other component to check for connectivity.</param>
    /// <returns>True if this component can connect to the other component, false otherwise.</returns>
    new bool CanConnectTo(Direction direction, IComponent otherComponent);
}