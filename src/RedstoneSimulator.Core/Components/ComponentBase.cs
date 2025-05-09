using RedstoneSimulator.Core.Common;
using RedstoneSimulator.Core.Grid;

namespace RedstoneSimulator.Core.Components;

/// <summary>
/// Base class for all redstone circuit components.
/// Provides common implementation for the IComponent interface.
/// </summary>
public abstract class ComponentBase : IComponent, ICloneable
{
    private int _powerLevel;
    private Cell? _currentCell;
    
    /// <summary>
    /// Gets the type of this component.
    /// </summary>
    public abstract ComponentType ComponentType { get; }
    
    /// <summary>
    /// Gets or sets the current power level of this component.
    /// The value is clamped between 0 and 15.
    /// </summary>
    public int PowerLevel
    {
        get => _powerLevel;
        protected set => _powerLevel = Math.Clamp(value, 0, 15);
    }
    
    /// <summary>
    /// Gets or sets the orientation of this component.
    /// </summary>
    public Direction Orientation { get; set; } = Direction.North;
    
    /// <summary>
    /// Gets the list of directions that this component can connect to.
    /// </summary>
    public abstract IReadOnlyList<Direction> ConnectedSides { get; }
    
    /// <summary>
    /// Gets the cell that contains this component.
    /// </summary>
    protected Cell? CurrentCell => _currentCell;
    
    /// <summary>
    /// Updates the state of this component based on its neighbors.
    /// </summary>
    /// <param name="neighbors">Dictionary of neighboring components keyed by direction.</param>
    public abstract void UpdateState(IDictionary<Direction, IComponent>? neighbors);
    
    /// <summary>
    /// Gets the power output of this component in the specified direction.
    /// </summary>
    /// <param name="side">The direction to get power output for.</param>
    /// <returns>The power level (0-15) provided in the specified direction.</returns>
    public abstract int GetPowerOutput(Direction side);
    
    /// <summary>
    /// Determines if this component can connect to the specified direction.
    /// </summary>
    /// <param name="side">The direction to check.</param>
    /// <returns>True if the component can connect in the specified direction, false otherwise.</returns>
    public abstract bool CanConnect(Direction side);
    
    /// <summary>
    /// Called when a neighbor changes state.
    /// </summary>
    /// <param name="side">The direction of the neighbor that changed.</param>
    public abstract void OnNeighborChange(Direction side);
    
    /// <summary>
    /// Called when this component is added to a cell.
    /// </summary>
    /// <param name="cell">The cell that this component was added to.</param>
    public virtual void OnAddedToCell(Cell cell)
    {
        _currentCell = cell;
    }
    
    /// <summary>
    /// Called when this component is removed from a cell.
    /// </summary>
    /// <param name="cell">The cell that this component was removed from.</param>
    public virtual void OnRemovedFromCell(Cell cell)
    {
        if (_currentCell == cell)
        {
            _currentCell = null;
        }
    }
    
    /// <summary>
    /// Updates the component during a simulation tick.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update.</param>
    public virtual void Update(float deltaTime)
    {
        // Default implementation does nothing
    }
    
    /// <summary>
    /// Determines if this component can be placed in the specified cell.
    /// </summary>
    /// <param name="cell">The cell to check.</param>
    /// <returns>True if the component can be placed in the cell, false otherwise.</returns>
    public virtual bool CanBePlacedInCell(Cell cell)
    {
        // Default implementation allows placement in any cell
        return true;
    }
    
    /// <summary>
    /// Gets serialization data for this component.
    /// </summary>
    /// <returns>The serialization data for this component.</returns>
    public virtual ComponentData GetSerializationData()
    {
        return new ComponentData
        {
            Type = ComponentType.ToString(),
            Properties = new Dictionary<string, object>
            {
                ["PowerLevel"] = PowerLevel,
                ["Orientation"] = (int)Orientation
            }
        };
    }
    
    /// <summary>
    /// Determines if this component can connect to another component in the specified direction.
    /// </summary>
    /// <param name="direction">The direction to check.</param>
    /// <param name="otherComponent">The other component to check for connectivity.</param>
    /// <returns>True if this component can connect to the other component, false otherwise.</returns>
    public virtual bool CanConnectTo(Direction direction, IComponent otherComponent)
    {
        // Default implementation checks if both components can connect in the appropriate directions
        if (!CanConnect(direction))
        {
            return false;
        }
        
        // Calculate the opposite direction
        Direction oppositeDirection = GetOppositeDirection(direction);
        
        // Check if the other component can connect in the opposite direction
        return otherComponent.CanConnect(oppositeDirection);
    }
    
    /// <summary>
    /// Rotates the component clockwise by 90 degrees.
    /// </summary>
    public virtual void RotateClockwise()
    {
        Orientation = Orientation.RotateClockwise();
    }
    
    /// <summary>
    /// Rotates the component counter-clockwise by 90 degrees.
    /// </summary>
    public virtual void RotateCounterClockwise()
    {
        Orientation = Orientation.RotateCounterClockwise();
    }
    
    /// <summary>
    /// Sets the direction of this component.
    /// </summary>
    /// <param name="direction">The direction to set.</param>
    public virtual void SetDirection(Direction direction)
    {
        Orientation = direction;
    }
    
    /// <summary>
    /// Gets a value indicating whether the component is powered.
    /// </summary>
    /// <returns>True if the component has a power level greater than 0, false otherwise.</returns>
    public bool IsPowered()
    {
        return PowerLevel > 0;
    }
    
    /// <summary>
    /// Creates a deep copy of this component.
    /// </summary>
    /// <returns>A new instance of the component with the same state.</returns>
    public virtual object Clone()
    {
        throw new NotImplementedException("Clone method must be implemented by derived classes.");
    }
    
    /// <summary>
    /// Releases resources used by this component.
    /// </summary>
    public virtual void Dispose()
    {
        // Default implementation does nothing
        GC.SuppressFinalize(this);
    }
    
    /// <summary>
    /// Gets the opposite direction of the specified direction.
    /// </summary>
    /// <param name="direction">The direction to get the opposite of.</param>
    /// <returns>The opposite direction.</returns>
    protected static Direction GetOppositeDirection(Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.South,
            Direction.East => Direction.West,
            Direction.South => Direction.North,
            Direction.West => Direction.East,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Invalid direction")
        };
    }
    
    /// <summary>
    /// Marks the current cell for update in the next simulation tick.
    /// </summary>
    protected void MarkCellForUpdate()
    {
        _currentCell?.MarkForUpdate();
    }
    
    /// <summary>
    /// Gets the neighboring cells in all valid connection directions.
    /// </summary>
    /// <returns>A dictionary of neighboring cells keyed by direction.</returns>
    protected Dictionary<Direction, Cell?> GetNeighborCells()
    {
        if (_currentCell == null)
        {
            return new Dictionary<Direction, Cell?>();
        }
        
        // TODO: This would need to be implemented by accessing a grid service
        // For now, return an empty dictionary
        return new Dictionary<Direction, Cell?>();
    }
    
    /// <summary>
    /// Notifies neighboring components of a state change.
    /// </summary>
    protected void NotifyNeighbors()
    {
        if (_currentCell == null)
        {
            return;
        }
        
        // TODO: This would need to be implemented by accessing a grid service
        // For now, do nothing
    }
}