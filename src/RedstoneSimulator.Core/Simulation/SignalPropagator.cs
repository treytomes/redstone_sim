// src\RedstoneSimulator.Core\Simulation\SignalPropagator.cs

using OpenTK.Mathematics;
using RedstoneSimulator.Core.Common;
using RedstoneSimulator.Core.Components;
using RedstoneSimulator.Core.Grid;

namespace RedstoneSimulator.Core.Simulation;

/// <summary>
/// Handles the propagation of redstone signals through a circuit.
/// </summary>
public class SignalPropagator
{
    private readonly IGrid _grid;
    private readonly Queue<(Vector2i Position, Direction Direction)> _updateQueue = new();
    private readonly HashSet<Vector2i> _updatedPositions = new();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SignalPropagator"/> class.
    /// </summary>
    /// <param name="grid">The grid to propagate signals through.</param>
    public SignalPropagator(IGrid grid)
    {
        _grid = grid ?? throw new ArgumentNullException(nameof(grid));
    }
    
    /// <summary>
    /// Propagates signals from a source position.
    /// </summary>
    /// <param name="sourcePosition">The position of the signal source.</param>
    public void PropagateSignals(Vector2i sourcePosition)
    {
        // Clear state from previous propagation
        _updateQueue.Clear();
        _updatedPositions.Clear();
        
        // Get the source component
        var sourceCell = _grid.GetCell(sourcePosition);
        if (sourceCell?.Component == null)
        {
            return;
        }
        
        // Add the source position to the update queue for each direction it can connect
        foreach (var direction in DirectionExtensions.CardinalDirections)
        {
            if (sourceCell.Component.CanConnect(direction))
            {
                _updateQueue.Enqueue((sourcePosition, direction));
            }
        }
        
        // Process the update queue
        while (_updateQueue.Count > 0)
        {
            var (currentPos, direction) = _updateQueue.Dequeue();
            
            // Mark this position as updated
            _updatedPositions.Add(currentPos);
            
            // Get the current cell
            var currentCell = _grid.GetCell(currentPos);
            if (currentCell?.Component == null)
            {
                continue;
            }
            
            // Calculate the neighbor position
            var neighborPos = currentPos + direction.ToVector();
            
            // Get the neighbor cell
            var neighborCell = _grid.GetCell(neighborPos);
            if (neighborCell?.Component == null)
            {
                continue;
            }
            
            // Calculate the opposite direction
            var oppositeDirection = direction.Opposite();
            
            // Check if the current component can connect to the neighbor in this direction
            if (!currentCell.Component.CanConnect(direction) || !neighborCell.Component.CanConnect(oppositeDirection))
            {
                continue;
            }
            
            // Get the power output from the current component in this direction
            int powerOutput = currentCell.Component.GetPowerOutput(direction);
            
            // Update the neighbor's state
            UpdateComponentState(neighborCell, oppositeDirection, powerOutput);
            
            // If this neighbor hasn't been updated yet, add its connections to the queue
            if (!_updatedPositions.Contains(neighborPos))
            {
                foreach (var nextDirection in DirectionExtensions.CardinalDirections)
                {
                    if (nextDirection != oppositeDirection && neighborCell.Component.CanConnect(nextDirection))
                    {
                        _updateQueue.Enqueue((neighborPos, nextDirection));
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Updates a component's state based on input from a specific direction.
    /// </summary>
    /// <param name="cell">The cell containing the component to update.</param>
    /// <param name="inputDirection">The direction the input is coming from.</param>
    /// <param name="inputPower">The power level of the input.</param>
    private void UpdateComponentState(Cell cell, Direction inputDirection, int inputPower)
    {
        if (cell?.Component == null)
        {
            return;
        }
        
        // Create a dictionary of neighbors for the UpdateState method
        var neighbors = new Dictionary<Direction, IComponent>();
        
        // For now, we're only adding the input direction
        // In a full implementation, you would get all neighbors
        neighbors[inputDirection] = new MockComponent(ComponentType.None) 
        {
            // This is a simplified approach - in a real implementation you'd have actual components
            PowerLevel = inputPower
        };
        
        // Update the component's state
        cell.Component.UpdateState(neighbors);
        
        // Mark the cell for update in the grid
        cell.MarkForUpdate();
    }
    
    /// <summary>
    /// Temporary mock component for signal propagation.
    /// </summary>
    private class MockComponent : IComponent
    {
        public ComponentType ComponentType { get; }
        public int PowerLevel { get; set; }
        public Direction Orientation { get; set; }
        
        public MockComponent(ComponentType type)
        {
            ComponentType = type;
        }
        
        public IReadOnlyList<Direction> ConnectedSides => Array.Empty<Direction>();
        
        public void UpdateState(IDictionary<Direction, IComponent>? neighbors) { }
        public int GetPowerOutput(Direction side) => PowerLevel;
        public bool CanConnect(Direction side) => true;
        public void OnNeighborChange(Direction side) { }
        public void OnAddedToCell(Cell cell) { }
        public void OnRemovedFromCell(Cell cell) { }
        public void Update(float deltaTime) { }
        public bool CanBePlacedInCell(Cell cell) => true;
        public ComponentData GetSerializationData() => new ComponentData();
        public bool CanConnectTo(Direction direction, IComponent otherComponent) => true;
        public void RotateClockwise() { }
        public void RotateCounterClockwise() { }
        public void SetDirection(Direction direction) => Orientation = direction;
        public void Dispose() { }
    }
}