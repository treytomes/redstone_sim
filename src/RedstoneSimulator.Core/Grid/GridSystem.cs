// src\RedstoneSimulator.Core\Grid\GridSystem.cs

using OpenTK.Mathematics;
using RedstoneSimulator.Core.Components;
using RedstoneSimulator.Core.Common;
using System.Collections.Concurrent;

namespace RedstoneSimulator.Core.Grid;

/// <summary>
/// Manages the 2D grid structure for the redstone circuit simulator.
/// Handles component placement, retrieval, and grid operations.
/// </summary>
public class GridSystem
{
    #region Events

    /// <summary>
    /// Event raised when a component is placed in the grid.
    /// </summary>
    public event EventHandler<GridComponentEventArgs>? ComponentPlaced;

    /// <summary>
    /// Event raised when a component is removed from the grid.
    /// </summary>
    public event EventHandler<GridComponentEventArgs>? ComponentRemoved;

    /// <summary>
    /// Event raised when the grid is resized.
    /// </summary>
    public event EventHandler<GridResizeEventArgs>? GridResized;

    /// <summary>
    /// Event raised when the grid is cleared.
    /// </summary>
    public event EventHandler? GridCleared;

    /// <summary>
    /// Event raised when a cell is marked for update.
    /// </summary>
    public event EventHandler<GridCellEventArgs>? CellMarkedForUpdate;

    #endregion

    #region Fields

    /// <summary>
    /// The dictionary storing all cells in the grid.
    /// </summary>
    private readonly ConcurrentDictionary<Vector2i, Cell> _cells;

    /// <summary>
    /// The set of cells that need to be updated in the next simulation tick.
    /// </summary>
    private readonly HashSet<Vector2i> _cellsToUpdate;

    /// <summary>
    /// Lock object for thread synchronization.
    /// </summary>
    private readonly object _updateLock = new();

    /// <summary>
    /// The width of the grid if bounded mode is enabled.
    /// </summary>
    private int _width;

    /// <summary>
    /// The height of the grid if bounded mode is enabled.
    /// </summary>
    private int _height;

    /// <summary>
    /// Flag indicating whether the grid has bounds.
    /// </summary>
    private bool _hasBounds;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="GridSystem"/> class with an unbounded grid.
    /// </summary>
    public GridSystem()
    {
        _cells = new ConcurrentDictionary<Vector2i, Cell>();
        _cellsToUpdate = new HashSet<Vector2i>();
        _hasBounds = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GridSystem"/> class with the specified dimensions.
    /// </summary>
    /// <param name="width">The width of the grid.</param>
    /// <param name="height">The height of the grid.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when width or height is less than 1.</exception>
    public GridSystem(int width, int height)
    {
        if (width < 1)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be at least 1.");
        if (height < 1)
            throw new ArgumentOutOfRangeException(nameof(height), "Height must be at least 1.");

        _cells = new ConcurrentDictionary<Vector2i, Cell>();
        _cellsToUpdate = new HashSet<Vector2i>();
        _width = width;
        _height = height;
        _hasBounds = true;

        // Initialize all cells within the bounds
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var position = new Vector2i(x, y);
                var cell = new Cell(x, y);
                _cells[position] = cell;
                cell.MarkedForUpdate += Cell_MarkedForUpdate;
            }
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the grid has bounds.
    /// </summary>
    public bool HasBounds => _hasBounds;

    /// <summary>
    /// Gets the width of the grid if it has bounds.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the grid is unbounded.</exception>
    public int Width
    {
        get
        {
            if (!_hasBounds)
                throw new InvalidOperationException("Cannot get the width of an unbounded grid.");
            return _width;
        }
    }

    /// <summary>
    /// Gets the height of the grid if it has bounds.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the grid is unbounded.</exception>
    public int Height
    {
        get
        {
            if (!_hasBounds)
                throw new InvalidOperationException("Cannot get the height of an unbounded grid.");
            return _height;
        }
    }

    /// <summary>
    /// Gets the number of cells in the grid.
    /// </summary>
    public int CellCount => _cells.Count;

    /// <summary>
    /// Gets the number of cells that need to be updated.
    /// </summary>
    public int UpdateQueueCount => _cellsToUpdate.Count;

    /// <summary>
    /// Gets a collection of all cell positions in the grid.
    /// </summary>
    public IEnumerable<Vector2i> CellPositions => _cells.Keys;

    #endregion

    #region Public Methods

    /// <summary>
    /// Places a component at the specified position in the grid.
    /// </summary>
    /// <param name="component">The component to place.</param>
    /// <param name="position">The position to place the component at.</param>
    /// <returns>True if the component was placed successfully, false otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the component is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the position is outside the grid bounds.</exception>
    public bool PlaceComponent(IComponent component, Vector2i position)
    {
        if (component == null)
            throw new ArgumentNullException(nameof(component));

        if (_hasBounds && !IsWithinBounds(position))
            throw new ArgumentOutOfRangeException(nameof(position), "Position is outside the grid bounds.");

        Cell cell = GetOrCreateCell(position);
        
        if (cell.Component != null)
            return false;

        try
        {
            cell.SetComponent(component);
            OnComponentPlaced(new GridComponentEventArgs(position, component));
            MarkCellForUpdate(position);
            MarkNeighborsForUpdate(position);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    /// <summary>
    /// Removes the component at the specified position from the grid.
    /// </summary>
    /// <param name="position">The position to remove the component from.</param>
    /// <returns>The removed component, or null if no component was removed.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the position is outside the grid bounds.</exception>
    public IComponent? RemoveComponent(Vector2i position)
    {
        if (_hasBounds && !IsWithinBounds(position))
            throw new ArgumentOutOfRangeException(nameof(position), "Position is outside the grid bounds.");

        if (!_cells.TryGetValue(position, out var cell))
            return null;

        IComponent? removedComponent = cell.Component;
        if (removedComponent != null)
        {
            cell.SetComponent(null);
            OnComponentRemoved(new GridComponentEventArgs(position, removedComponent));
            MarkNeighborsForUpdate(position);
        }

        return removedComponent;
    }

    /// <summary>
    /// Gets the component at the specified position in the grid.
    /// </summary>
    /// <param name="position">The position to get the component from.</param>
    /// <returns>The component at the specified position, or null if no component exists.</returns>
    public IComponent? GetComponentAt(Vector2i position)
    {
        return _cells.TryGetValue(position, out var cell) ? cell.Component : null;
    }

    /// <summary>
    /// Gets the cell at the specified position in the grid.
    /// For unbounded grids, creates a new cell if one doesn't exist at the position.
    /// </summary>
    /// <param name="position">The position to get the cell from.</param>
    /// <returns>The cell at the specified position, or null if the position is outside the grid bounds.</returns>
    public Cell? GetCellAt(Vector2i position)
    {
        // For bounded grids, return null if position is out of bounds
        if (_hasBounds && !IsWithinBounds(position))
        {
            return null;
        }
        
        // For unbounded grids or valid positions in bounded grids,
        // get the existing cell or create a new one
        return GetOrCreateCell(position);
    }

    /// <summary>
    /// Checks if a position is within the grid bounds.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True if the position is within the grid bounds, false otherwise.</returns>
    public bool IsWithinBounds(Vector2i position)
    {
        if (!_hasBounds)
            return true;

        return position.X >= 0 && position.X < _width && position.Y >= 0 && position.Y < _height;
    }

    /// <summary>
    /// Resizes the grid to the specified dimensions.
    /// </summary>
    /// <param name="width">The new width of the grid.</param>
    /// <param name="height">The new height of the grid.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when width or height is less than 1.</exception>
    public void ResizeGrid(int width, int height)
    {
        if (width < 1)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be at least 1.");
        if (height < 1)
            throw new ArgumentOutOfRangeException(nameof(height), "Height must be at least 1.");

        int oldWidth = _hasBounds ? _width : 0;
        int oldHeight = _hasBounds ? _height : 0;
        bool wasUnbounded = !_hasBounds;

        // If the grid was previously unbounded, we need to remove cells outside the new bounds
        if (wasUnbounded || width < oldWidth || height < oldHeight)
        {
            List<Vector2i> cellsToRemove = new List<Vector2i>();
            
            foreach (var position in _cells.Keys)
            {
                if (position.X >= width || position.Y >= height || position.X < 0 || position.Y < 0)
                {
                    cellsToRemove.Add(position);
                }
            }
            
            foreach (var position in cellsToRemove)
            {
                // Get the component directly from the cell to avoid bounds checking
                var component = _cells.TryGetValue(position, out var cell) ? cell.Component : null;
                
                // Remove the component from the cell if it exists
                if (component != null)
                {
                    cell!.SetComponent(null);
                    OnComponentRemoved(new GridComponentEventArgs(position, component));
                }
                
                // Remove the cell
                if (_cells.TryRemove(position, out cell))
                {
                    cell.MarkedForUpdate -= Cell_MarkedForUpdate;
                }
            }
        }
        
        // Update dimensions
        _width = width;
        _height = height;
        _hasBounds = true;
        
        // Create cells for new areas
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var position = new Vector2i(x, y);
                if (!_cells.ContainsKey(position))
                {
                    var cell = new Cell(x, y);
                    _cells[position] = cell;
                    cell.MarkedForUpdate += Cell_MarkedForUpdate;
                }
            }
        }
        
        OnGridResized(new GridResizeEventArgs(oldWidth, oldHeight, width, height));
    }

    /// <summary>
    /// Makes the grid unbounded, preserving all existing cells.
    /// </summary>
    public void MakeUnbounded()
    {
        if (!_hasBounds)
            return;
            
        int oldWidth = _width;
        int oldHeight = _height;
        
        _hasBounds = false;
        _width = 0;
        _height = 0;
        
        OnGridResized(new GridResizeEventArgs(oldWidth, oldHeight, 0, 0));
    }

    /// <summary>
    /// Clears all components from the grid.
    /// </summary>
    public void Clear()
    {
        List<Vector2i> positions = new List<Vector2i>(_cells.Keys);
        
        foreach (var position in positions)
        {
            RemoveComponent(position);
        }
        
        if (!_hasBounds)
        {
            foreach (var position in positions)
            {
                if (_cells.TryRemove(position, out var cell))
                {
                    cell.MarkedForUpdate -= Cell_MarkedForUpdate;
                }
            }
        }
        
        _cellsToUpdate.Clear();
        OnGridCleared(EventArgs.Empty);
    }

    /// <summary>
    /// Marks a cell for update in the next simulation tick.
    /// </summary>
    /// <param name="position">The position of the cell to mark.</param>
    /// <returns>True if the cell was marked, false if the position is invalid.</returns>
    public bool MarkCellForUpdate(Vector2i position)
    {
        if (!_cells.ContainsKey(position))
            return false;
            
        lock (_updateLock)
        {
            _cellsToUpdate.Add(position);
        }
        
        OnCellMarkedForUpdate(new GridCellEventArgs(position));
        return true;
    }

    /// <summary>
    /// Gets all cells that need to be updated in the next simulation tick.
    /// </summary>
    /// <returns>An enumerable of positions for cells that need updates.</returns>
    public IEnumerable<Vector2i> GetCellsToUpdate()
    {
        lock (_updateLock)
        {
            return _cellsToUpdate.ToList();
        }
    }

    /// <summary>
    /// Clears the update queue after processing updates.
    /// </summary>
    public void ClearUpdateQueue()
    {
        lock (_updateLock)
        {
            _cellsToUpdate.Clear();
        }
    }

    /// <summary>
    /// Updates all cells marked for update.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update.</param>
    /// <returns>The number of cells updated.</returns>
    public int UpdateMarkedCells(float deltaTime)
    {
        List<Vector2i> cellsToUpdate;
        
        lock (_updateLock)
        {
            cellsToUpdate = _cellsToUpdate.ToList();
            _cellsToUpdate.Clear();
        }
        
        foreach (var position in cellsToUpdate)
        {
            if (_cells.TryGetValue(position, out var cell))
            {
                cell.Update(deltaTime);
            }
        }
        
        return cellsToUpdate.Count;
    }

    /// <summary>
    /// Gets the neighboring cells around the specified position.
    /// </summary>
    /// <param name="position">The center position.</param>
    /// <returns>A dictionary of cells keyed by direction.</returns>
    public IDictionary<Direction, Cell?> GetNeighborCells(Vector2i position)
    {
        var neighbors = new Dictionary<Direction, Cell?>();
        
        foreach (var direction in DirectionExtensions.CardinalDirections)
        {
            Vector2i neighborPos = position + direction.ToVector();
            Cell? neighborCell = null;
            
            if (!_hasBounds || IsWithinBounds(neighborPos))
            {
                neighborCell = GetOrCreateCell(neighborPos);
            }
            
            neighbors[direction] = neighborCell;
        }
        
        return neighbors;
    }

    /// <summary>
    /// Gets the neighboring components around the specified position.
    /// </summary>
    /// <param name="position">The center position.</param>
    /// <returns>A dictionary of components keyed by direction.</returns>
    public IDictionary<Direction, IComponent?> GetNeighborComponents(Vector2i position)
    {
        var neighbors = new Dictionary<Direction, IComponent?>();
        
        foreach (var direction in DirectionExtensions.CardinalDirections)
        {
            Vector2i neighborPos = position + direction.ToVector();
            IComponent? component = null;
            
            if (!_hasBounds || IsWithinBounds(neighborPos))
            {
                component = GetComponentAt(neighborPos);
            }
            
            neighbors[direction] = component;
        }
        
        return neighbors;
    }

    /// <summary>
    /// Determines if a component can be placed at the specified position.
    /// </summary>
    /// <param name="component">The component to check.</param>
    /// <param name="position">The position to check.</param>
    /// <returns>True if the component can be placed, false otherwise.</returns>
    public bool CanPlaceComponentAt(IComponent component, Vector2i position)
    {
        if (component == null)
            return false;
            
        if (_hasBounds && !IsWithinBounds(position))
            return false;
            
        Cell cell = GetOrCreateCell(position);
        return cell.IsEmpty && component.CanBePlacedInCell(cell);
    }

    /// <summary>
    /// Finds all cells containing components of the specified type.
    /// </summary>
    /// <param name="componentType">The component type to search for.</param>
    /// <returns>A collection of positions where components of the specified type are located.</returns>
    public IEnumerable<Vector2i> FindComponentsByType(ComponentType componentType)
    {
        return _cells
            .Where(kvp => kvp.Value.Component?.ComponentType == componentType)
            .Select(kvp => kvp.Key);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Gets the cell at the specified position or creates a new one if it doesn't exist.
    /// </summary>
    /// <param name="position">The position to get or create a cell for.</param>
    /// <returns>The cell at the specified position.</returns>
    private Cell GetOrCreateCell(Vector2i position)
    {
        return _cells.GetOrAdd(position, pos => {
            var cell = new Cell(pos.X, pos.Y);
            cell.MarkedForUpdate += Cell_MarkedForUpdate;
            return cell;
        });
    }

    /// <summary>
    /// Marks the neighboring cells of the specified position for update.
    /// </summary>
    /// <param name="position">The position whose neighbors should be marked for update.</param>
    private void MarkNeighborsForUpdate(Vector2i position)
    {
        foreach (var direction in DirectionExtensions.CardinalDirections)
        {
            Vector2i neighborPos = position + direction.ToVector();
            
            if (!_hasBounds || IsWithinBounds(neighborPos))
            {
                MarkCellForUpdate(neighborPos);
            }
        }
    }

    /// <summary>
    /// Handles the MarkedForUpdate event from a cell.
    /// </summary>
    /// <param name="sender">The cell that was marked for update.</param>
    /// <param name="e">The event arguments.</param>
    private void Cell_MarkedForUpdate(object? sender, EventArgs e)
    {
        if (sender is Cell cell)
        {
            MarkCellForUpdate(new Vector2i(cell.X, cell.Y));
        }
    }

    #endregion

    #region Event Invocation Methods

    /// <summary>
    /// Raises the ComponentPlaced event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnComponentPlaced(GridComponentEventArgs e)
    {
        ComponentPlaced?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ComponentRemoved event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnComponentRemoved(GridComponentEventArgs e)
    {
        ComponentRemoved?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the GridResized event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnGridResized(GridResizeEventArgs e)
    {
        GridResized?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the GridCleared event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnGridCleared(EventArgs e)
    {
        GridCleared?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellMarkedForUpdate event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnCellMarkedForUpdate(GridCellEventArgs e)
    {
        CellMarkedForUpdate?.Invoke(this, e);
    }

    #endregion
}