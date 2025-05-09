// src\RedstoneSimulator.Core\Grid\GridEventArgs.cs

using OpenTK.Mathematics;
using RedstoneSimulator.Core.Components;

namespace RedstoneSimulator.Core.Grid;

/// <summary>
/// Provides data for the grid component-related events.
/// </summary>
public class GridComponentEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GridComponentEventArgs"/> class.
    /// </summary>
    /// <param name="position">The position of the component in the grid.</param>
    /// <param name="component">The component involved in the event.</param>
    public GridComponentEventArgs(Vector2i position, IComponent component)
    {
        Position = position;
        Component = component;
    }

    /// <summary>
    /// Gets the position of the component in the grid.
    /// </summary>
    public Vector2i Position { get; }

    /// <summary>
    /// Gets the component involved in the event.
    /// </summary>
    public IComponent Component { get; }
}
