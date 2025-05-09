namespace RedstoneSimulator.Core.Grid;

/// <summary>
/// Provides data for the grid resize event.
/// </summary>
public class GridResizeEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GridResizeEventArgs"/> class.
    /// </summary>
    /// <param name="oldWidth">The previous width of the grid.</param>
    /// <param name="oldHeight">The previous height of the grid.</param>
    /// <param name="newWidth">The new width of the grid.</param>
    /// <param name="newHeight">The new height of the grid.</param>
    public GridResizeEventArgs(int oldWidth, int oldHeight, int newWidth, int newHeight)
    {
        OldWidth = oldWidth;
        OldHeight = oldHeight;
        NewWidth = newWidth;
        NewHeight = newHeight;
    }

    /// <summary>
    /// Gets the previous width of the grid.
    /// </summary>
    public int OldWidth { get; }

    /// <summary>
    /// Gets the previous height of the grid.
    /// </summary>
    public int OldHeight { get; }

    /// <summary>
    /// Gets the new width of the grid.
    /// </summary>
    public int NewWidth { get; }

    /// <summary>
    /// Gets the new height of the grid.
    /// </summary>
    public int NewHeight { get; }
}
