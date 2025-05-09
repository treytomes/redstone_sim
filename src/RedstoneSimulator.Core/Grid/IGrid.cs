// src\RedstoneSimulator.Core\Grid\IGrid.cs

using OpenTK.Mathematics;

namespace RedstoneSimulator.Core.Grid;

/// <summary>
/// Interface for a grid that can hold cells with components.
/// </summary>
public interface IGrid
{
    /// <summary>
    /// Gets the cell at the specified position.
    /// </summary>
    /// <param name="position">The position to get the cell at.</param>
    /// <returns>The cell at the specified position, or null if no cell exists.</returns>
    Cell? GetCell(Vector2i position);
    
    /// <summary>
    /// Gets all cells in the grid.
    /// </summary>
    /// <returns>A collection of all cells in the grid.</returns>
    IEnumerable<Cell> GetAllCells();
    
    /// <summary>
    /// Gets the positions of cells that have been marked for update.
    /// </summary>
    /// <returns>A collection of positions for cells that need updating.</returns>
    IEnumerable<Vector2i> GetMarkedCells();
    
    /// <summary>
    /// Clears the list of cells marked for update.
    /// </summary>
    void ClearMarkedCells();
    
    /// <summary>
    /// Adds a cell to the grid at the specified position.
    /// </summary>
    /// <param name="position">The position to add the cell at.</param>
    /// <param name="cell">The cell to add.</param>
    void AddCell(Vector2i position, Cell cell);
    
    /// <summary>
    /// Removes the cell at the specified position.
    /// </summary>
    /// <param name="position">The position to remove the cell from.</param>
    /// <returns>True if a cell was removed, false otherwise.</returns>
    bool RemoveCell(Vector2i position);
    
    /// <summary>
    /// Marks a cell for update in the next simulation tick.
    /// </summary>
    /// <param name="position">The position of the cell to mark.</param>
    void MarkCellForUpdate(Vector2i position);
    
    /// <summary>
    /// Clears all cells from the grid.
    /// </summary>
    void Clear();
}