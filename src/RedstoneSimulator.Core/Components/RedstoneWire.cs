using RedstoneSimulator.Core.Common;

namespace RedstoneSimulator.Core.Components;

/// <summary>
/// Represents a redstone wire component that can transmit power signals
/// with signal decay of 1 per block traveled.
/// </summary>
public class RedstoneWire : ComponentBase
{
    #region Fields

    private readonly Dictionary<Direction, bool> _enabledDirections;
    private readonly ReaderWriterLockSlim _stateLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RedstoneWire"/> class.
    /// </summary>
    public RedstoneWire()
    {
        PowerLevel = 0;
        _enabledDirections = new Dictionary<Direction, bool>
        {
            { Direction.North, true },
            { Direction.East, true },
            { Direction.South, true },
            { Direction.West, true }
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the component type.
    /// </summary>
    public override ComponentType ComponentType => ComponentType.RedstoneWire;

#if DEBUG
    /// <summary>
    /// Gets or sets the power level of this component.
    /// This property is only available in debug builds for testing.
    /// </summary>
    public new int PowerLevel 
    { 
        get => base.PowerLevel; 
        set
        {
            // Clamp power level between 0 and 15
            base.PowerLevel = Math.Clamp(value, 0, 15);
        }
    }
#endif

    /// <summary>
    /// Gets the list of directions that this component can connect to.
    /// </summary>
    public override IReadOnlyList<Direction> ConnectedSides
    {
        get
        {
            _stateLock.EnterReadLock();
            try
            {
                return _enabledDirections.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
            }
            finally
            {
                _stateLock.ExitReadLock();
            }
        }
    }
    
    #endregion

    #region Methods

    /// <summary>
    /// Toggles the connection state for the specified direction.
    /// </summary>
    /// <param name="direction">The direction to toggle.</param>
    public void ToggleDirection(Direction direction)
    {
        _stateLock.EnterWriteLock();
        try
        {
            if (_enabledDirections.ContainsKey(direction))
            {
                _enabledDirections[direction] = !_enabledDirections[direction];
            }
        }
        finally
        {
            _stateLock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Checks if the specified direction is enabled for connection.
    /// </summary>
    /// <param name="direction">The direction to check.</param>
    /// <returns>True if the direction is enabled, false otherwise.</returns>
    public bool IsDirectionEnabled(Direction direction)
    {
        _stateLock.EnterReadLock();
        try
        {
            return _enabledDirections.TryGetValue(direction, out bool isEnabled) && isEnabled;
        }
        finally
        {
            _stateLock.ExitReadLock();
        }
    }

    /// <summary>
    /// Internal version of IsDirectionEnabled that doesn't acquire a lock.
    /// Used by methods that already have a lock.
    /// </summary>
    private bool IsDirectionEnabledInternal(Direction direction)
    {
        return _enabledDirections.TryGetValue(direction, out bool isEnabled) && isEnabled;
    }

    /// <summary>
    /// Updates the state of this component based on its neighbors.
    /// </summary>
    /// <param name="neighbors">Dictionary of neighboring components keyed by direction.</param>
    public override void UpdateState(IDictionary<Direction, IComponent>? neighbors)
    {
        if (neighbors == null)
        {
            PowerLevel = 0;
            return;
        }

        int highestPower = 0;
        
        _stateLock.EnterReadLock();
        try
        {
            foreach (var kvp in neighbors)
            {
                if (!IsDirectionEnabledInternal(kvp.Key)) continue;
                
                var neighborPower = kvp.Value.GetPowerOutput(kvp.Key.Opposite());
                if (neighborPower > highestPower)
                {
                    highestPower = neighborPower;
                }
            }
        }
        finally
        {
            _stateLock.ExitReadLock();
        }
        
        // Decrease power by 1, unless it's already 0
        PowerLevel = highestPower > 0 ? highestPower - 1 : 0;
    }

    /// <summary>
    /// Gets the power output of this component in the specified direction.
    /// </summary>
    /// <param name="side">The direction to get power output for.</param>
    /// <returns>The power level if the direction is enabled, 0 otherwise.</returns>
    public override int GetPowerOutput(Direction side)
    {
        return IsDirectionEnabled(side) ? PowerLevel : 0;
    }

    /// <summary>
    /// Determines if this component can connect to the specified direction.
    /// </summary>
    /// <param name="side">The direction to check.</param>
    /// <returns>True if the component can connect in the specified direction, false otherwise.</returns>
    public override bool CanConnect(Direction side)
    {
        // Only connect to cardinal directions
        if (!DirectionExtensions.CardinalDirections.Contains(side))
            return false;
            
        return IsDirectionEnabled(side);
    }

    /// <summary>
    /// Determines if this component can connect to another component in the specified direction.
    /// </summary>
    /// <param name="component">The component to check connection with.</param>
    /// <param name="side">The direction to check.</param>
    /// <returns>True if this component can connect to the other component, false otherwise.</returns>
    public bool CanConnectToComponent(IComponent component, Direction side)
    {
        if (!IsDirectionEnabled(side))
            return false;
            
        return component.CanConnect(side.Opposite());
    }

    /// <summary>
    /// Called when a neighbor changes state.
    /// </summary>
    /// <param name="side">The direction of the neighbor that changed.</param>
    public override void OnNeighborChange(Direction side)
    {
        // This will be handled by the signal manager to schedule an update
    }

    /// <summary>
    /// Called when this component is placed in the world.
    /// </summary>
    /// <param name="neighbors">The neighboring components.</param>
    public void OnPlacement(IDictionary<Direction, IComponent> neighbors)
    {
        if (neighbors == null)
            return;
            
        _stateLock.EnterWriteLock();
        try
        {
            foreach (var kvp in neighbors)
            {
                if (!kvp.Value.CanConnect(kvp.Key.Opposite()))
                {
                    _enabledDirections[kvp.Key] = false;
                }
            }
        }
        finally
        {
            _stateLock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Resets all connections to their default enabled state.
    /// </summary>
    public void ResetConnections()
    {
        _stateLock.EnterWriteLock();
        try
        {
            foreach (var direction in _enabledDirections.Keys.ToList())
            {
                _enabledDirections[direction] = true;
            }
        }
        finally
        {
            _stateLock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Gets the visual power level for rendering purposes.
    /// </summary>
    /// <returns>The current power level.</returns>
    public int GetVisualPowerLevel()
    {
        return PowerLevel;
    }

    /// <summary>
    /// Serializes this component's state to a dictionary.
    /// </summary>
    /// <returns>A dictionary containing the serialized state.</returns>
    public Dictionary<string, object> Serialize()
    {
        _stateLock.EnterReadLock();
        try
        {
            var data = new Dictionary<string, object>
            {
                { "PowerLevel", PowerLevel },
                { "EnabledDirections", _enabledDirections.ToDictionary(
                    kvp => kvp.Key.ToString(), 
                    kvp => kvp.Value) }
            };
            return data;
        }
        finally
        {
            _stateLock.ExitReadLock();
        }
    }

    /// <summary>
    /// Deserializes component state from a dictionary.
    /// </summary>
    /// <param name="data">The dictionary containing serialized state.</param>
    public void Deserialize(Dictionary<string, object> data)
    {
        if (data == null)
            return;

        _stateLock.EnterWriteLock();
        try
        {
            if (data.TryGetValue("PowerLevel", out var powerObj) && powerObj is int power)
            {
                PowerLevel = power;
            }

            if (data.TryGetValue("EnabledDirections", out var directionsObj) && 
                directionsObj is Dictionary<string, bool> directionDict)
            {
                foreach (var kvp in directionDict)
                {
                    if (Enum.TryParse<Direction>(kvp.Key, out var direction))
                    {
                        _enabledDirections[direction] = kvp.Value;
                    }
                }
            }
        }
        finally
        {
            _stateLock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Releases resources used by this component.
    /// </summary>
    public override void Dispose()
    {
        _stateLock.Dispose();
        base.Dispose();
    }

    #endregion
}