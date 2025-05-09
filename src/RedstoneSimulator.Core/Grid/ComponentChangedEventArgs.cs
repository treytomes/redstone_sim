using RedstoneSimulator.Core.Components;

namespace RedstoneSimulator.Core.Grid;

/// <summary>
/// Event arguments for component change events.
/// </summary>
public class ComponentChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the old component that was in the cell.
    /// </summary>
    public IComponent? OldComponent { get; }
    
    /// <summary>
    /// Gets the new component that is now in the cell.
    /// </summary>
    public IComponent? NewComponent { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentChangedEventArgs"/> class.
    /// </summary>
    /// <param name="oldComponent">The old component that was in the cell.</param>
    /// <param name="newComponent">The new component that is now in the cell.</param>
    public ComponentChangedEventArgs(IComponent? oldComponent, IComponent? newComponent)
    {
        OldComponent = oldComponent;
        NewComponent = newComponent;
    }
}
