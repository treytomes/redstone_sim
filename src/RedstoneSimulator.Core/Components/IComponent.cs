// src\RedstoneSimulator.Core\Components\IComponent.cs

using RedstoneSimulator.Core.Common;
using RedstoneSimulator.Core.Grid;

namespace RedstoneSimulator.Core.Components;

/// <summary>
/// Defines the interface for all redstone circuit components.
/// </summary>
public interface IComponent : IDisposable
{
    /// <summary>
    /// Gets the type of this component.
    /// </summary>
    ComponentType ComponentType { get; }
    
    /// <summary>
    /// Gets the current power level of this component (0-15).
    /// </summary>
    int PowerLevel { get; }
    
    /// <summary>
    /// Gets or sets the orientation of this component.
    /// </summary>
    Direction Orientation { get; set; }
    
    /// <summary>
    /// Gets the list of directions that this component can connect to.
    /// </summary>
    IReadOnlyList<Direction> ConnectedSides { get; }
    
    /// <summary>
    /// Updates the state of this component based on its neighbors.
    /// </summary>
    /// <param name="neighbors">Dictionary of neighboring components keyed by direction.</param>
    void UpdateState(IDictionary<Direction, IComponent>? neighbors);
    
    /// <summary>
    /// Gets the power output of this component in the specified direction.
    /// </summary>
    /// <param name="side">The direction to get power output for.</param>
    /// <returns>The power level (0-15) provided in the specified direction.</returns>
    int GetPowerOutput(Direction side);
    
    /// <summary>
    /// Determines if this component can connect to the specified direction.
    /// </summary>
    /// <param name="side">The direction to check.</param>
    /// <returns>True if the component can connect in the specified direction, false otherwise.</returns>
    bool CanConnect(Direction side);
    
    /// <summary>
    /// Called when a neighbor changes state.
    /// </summary>
    /// <param name="side">The direction of the neighbor that changed.</param>
    void OnNeighborChange(Direction side);
    
    /// <summary>
    /// Called when this component is added to a cell.
    /// </summary>
    /// <param name="cell">The cell that this component was added to.</param>
    void OnAddedToCell(Cell cell);
    
    /// <summary>
    /// Called when this component is removed from a cell.
    /// </summary>
    /// <param name="cell">The cell that this component was removed from.</param>
    void OnRemovedFromCell(Cell cell);
    
    /// <summary>
    /// Updates the component during a simulation tick.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update.</param>
    void Update(float deltaTime);
    
    /// <summary>
    /// Determines if this component can be placed in the specified cell.
    /// </summary>
    /// <param name="cell">The cell to check.</param>
    /// <returns>True if the component can be placed in the cell, false otherwise.</returns>
    bool CanBePlacedInCell(Cell cell);
    
    /// <summary>
    /// Gets serialization data for this component.
    /// </summary>
    /// <returns>The serialization data for this component.</returns>
    ComponentData GetSerializationData();
    
    /// <summary>
    /// Determines if this component can connect to another component in the specified direction.
    /// </summary>
    /// <param name="direction">The direction to check.</param>
    /// <param name="otherComponent">The other component to check for connectivity.</param>
    /// <returns>True if this component can connect to the other component, false otherwise.</returns>
    bool CanConnectTo(Direction direction, IComponent otherComponent);
}