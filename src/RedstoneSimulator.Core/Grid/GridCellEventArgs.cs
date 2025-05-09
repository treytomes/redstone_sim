using OpenTK.Mathematics;

namespace RedstoneSimulator.Core.Grid;

/// <summary>
/// Provides data for cell-related events.
/// </summary>
public class GridCellEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GridCellEventArgs"/> class.
    /// </summary>
    /// <param name="position">The position of the cell in the grid.</param>
    public GridCellEventArgs(Vector2i position)
    {
        Position = position;
    }

    /// <summary>
    /// Gets the position of the cell in the grid.
    /// </summary>
    public Vector2i Position { get; }
}
