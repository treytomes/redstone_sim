using RedstoneSimulator.Core.Common;

namespace RedstoneSimulator.Core.Components;

/// <summary>
/// Base class for all redstone circuit components.
/// Provides common implementation for the IComponent interface.
/// </summary>
public abstract class ComponentBase : IComponent
{
    private int _powerLevel;
    
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
    /// Updates the state of this component based on its neighbors.
    /// </summary>
    /// <param name="neighbors">Dictionary of neighboring components keyed by direction.</param>
    public abstract void UpdateState(IDictionary<Direction, IComponent> neighbors);
    
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
    public virtual IComponent Clone()
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
}
