using RedstoneSimulator.Core.Common;

namespace RedstoneSimulator.Core.Components;

/// <summary>
/// Interface for directional components.
/// </summary>
public interface IDirectionalComponent : IComponent
{
    /// <summary>
    /// Sets the direction of this component.
    /// </summary>
    /// <param name="direction">The direction to set.</param>
    void SetDirection(Direction direction);
}