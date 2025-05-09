// src\RedstoneSimulator.Core\Components\RedstoneSwitch.cs

using RedstoneSimulator.Core.Common;
using RedstoneSimulator.Core.Grid;

namespace RedstoneSimulator.Core.Components;

/// <summary>
/// Represents a redstone switch (lever) that provides continuous power when toggled on.
/// </summary>
public class RedstoneSwitch : ComponentBase, IPowerableComponent
{
    private bool _isOn;

    /// <summary>
    /// Gets the type of this component.
    /// </summary>
    public override ComponentType ComponentType => ComponentType.Switch;

    /// <summary>
    /// Gets or sets the power level of this component.
    /// When on, a switch outputs power level 15.
    /// </summary>
    public new int PowerLevel
    {
        get => base.PowerLevel;
        set => base.PowerLevel = value;
    }

    /// <summary>
    /// Gets a value indicating whether the switch is currently on.
    /// </summary>
    public bool IsOn => _isOn;

    /// <summary>
    /// Gets the list of directions that this component can connect to.
    /// A switch can only connect in the direction it's facing.
    /// </summary>
    public override IReadOnlyList<Direction> ConnectedSides => new[] { Orientation };

    /// <summary>
    /// Initializes a new instance of the <see cref="RedstoneSwitch"/> class.
    /// </summary>
    /// <param name="initialState">The initial state of the switch (true for on, false for off).</param>
    public RedstoneSwitch(bool initialState = false)
    {
        _isOn = initialState;
        PowerLevel = initialState ? 15 : 0;
    }

    /// <summary>
    /// Toggles the switch between on and off states.
    /// </summary>
    public void Toggle()
    {
        _isOn = !_isOn;
        PowerLevel = _isOn ? 15 : 0;
        MarkCellForUpdate();
        NotifyNeighbors();
    }

    /// <summary>
    /// Sets the switch to a specific state.
    /// </summary>
    /// <param name="state">The state to set (true for on, false for off).</param>
    public void SetState(bool state)
    {
        if (_isOn != state)
        {
            _isOn = state;
            PowerLevel = _isOn ? 15 : 0;
            MarkCellForUpdate();
            NotifyNeighbors();
        }
    }

    /// <summary>
    /// Updates the state of this component based on its neighbors.
    /// Switches don't change state based on neighbors.
    /// </summary>
    /// <param name="neighbors">Dictionary of neighboring components keyed by direction.</param>
    public override void UpdateState(IDictionary<Direction, IComponent>? neighbors)
    {
        // Switches don't update based on neighbors
    }

    /// <summary>
    /// Gets the power output of this component in the specified direction.
    /// </summary>
    /// <param name="side">The direction to get power output for.</param>
    /// <returns>The power level (0-15) provided in the specified direction.</returns>
    public override int GetPowerOutput(Direction side)
    {
        // Switches only output power in the direction they're facing
        return side == Orientation ? PowerLevel : 0;
    }

    /// <summary>
    /// Determines if this component can connect to the specified direction.
    /// </summary>
    /// <param name="side">The direction to check.</param>
    /// <returns>True if the component can connect in the specified direction, false otherwise.</returns>
    public override bool CanConnect(Direction side)
    {
        // Switches can only connect in the direction they're facing
        return side == Orientation;
    }

    /// <summary>
    /// Called when a neighbor changes state.
    /// Switches don't respond to neighbor changes.
    /// </summary>
    /// <param name="side">The direction of the neighbor that changed.</param>
    public override void OnNeighborChange(Direction side)
    {
        // Switches don't respond to neighbor changes
    }

    /// <summary>
    /// Creates a deep copy of this component.
    /// </summary>
    /// <returns>A new instance of the RedstoneSwitch with the same state.</returns>
    public override object Clone()
    {
        return new RedstoneSwitch(_isOn)
        {
            Orientation = this.Orientation,
            PowerLevel = this.PowerLevel
        };
    }

    /// <summary>
    /// Gets serialization data for this component.
    /// </summary>
    /// <returns>The serialization data for this component.</returns>
    public override ComponentData GetSerializationData()
    {
        var data = base.GetSerializationData();
        data.Properties["IsOn"] = _isOn;
        return data;
    }
}