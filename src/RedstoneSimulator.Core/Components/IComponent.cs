using System;
using System.Collections.Generic;
using RedstoneSimulator.Core.Common;

namespace RedstoneSimulator.Core.Components
{
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
        void UpdateState(IDictionary<Direction, IComponent> neighbors);
        
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
    }
}