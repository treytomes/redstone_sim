using RedstoneSimulator.Core.Common;

namespace RedstoneSimulator.Core.Components;

/// <summary>
/// Interface for wire components.
/// </summary>
public interface IWireComponent : IComponent
{
    /// <summary>
    /// Gets the component type.
    /// </summary>
    /// <returns>The component type.</returns>
    ComponentType GetComponentType();
}
