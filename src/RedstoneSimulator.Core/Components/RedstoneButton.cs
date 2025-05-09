using RedstoneSimulator.Core.Common;

namespace RedstoneSimulator.Core.Components;

/// <summary>
/// Represents a redstone button that provides a momentary pulse of power when activated.
/// </summary>
public class RedstoneButton : ComponentBase, IPowerableComponent
{
    private bool _isPressed;
    private float _remainingActivationTime;
    private readonly float _activationDuration;

    /// <summary>
    /// Gets the type of this component.
    /// </summary>
    public override ComponentType ComponentType => ComponentType.Button;

    /// <summary>
    /// Gets or sets the power level of this component.
    /// When pressed, a button outputs power level 15.
    /// </summary>
    public new int PowerLevel
    {
        get => base.PowerLevel;
        set => base.PowerLevel = value;
    }

    /// <summary>
    /// Gets a value indicating whether the button is currently pressed.
    /// </summary>
    public bool IsPressed => _isPressed;

    /// <summary>
    /// Gets the list of directions that this component can connect to.
    /// A button can only connect in the direction it's facing.
    /// </summary>
    public override IReadOnlyList<Direction> ConnectedSides => new[] { Orientation };

    /// <summary>
    /// Initializes a new instance of the <see cref="RedstoneButton"/> class.
    /// </summary>
    /// <param name="activationDuration">The duration in seconds that the button stays pressed.</param>
    public RedstoneButton(float activationDuration = 1.0f)
    {
        _activationDuration = activationDuration;
        _remainingActivationTime = 0;
        _isPressed = false;
        PowerLevel = 0;
    }

    /// <summary>
    /// Presses the button, setting its power level to 15 and starting the activation timer.
    /// </summary>
    public void Press()
    {
        _isPressed = true;
        _remainingActivationTime = _activationDuration;
        PowerLevel = 15;
        MarkCellForUpdate();
        NotifyNeighbors();
    }

    /// <summary>
    /// Updates the button's state during a simulation tick.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update.</param>
    public override void Update(float deltaTime)
    {
        if (_isPressed)
        {
            _remainingActivationTime -= deltaTime;
            
            if (_remainingActivationTime <= 0)
            {
                _isPressed = false;
                PowerLevel = 0;
                _remainingActivationTime = 0;
                MarkCellForUpdate();
                NotifyNeighbors();
            }
        }
    }

    /// <summary>
    /// Updates the state of this component based on its neighbors.
    /// Buttons don't change state based on neighbors.
    /// </summary>
    /// <param name="neighbors">Dictionary of neighboring components keyed by direction.</param>
    public override void UpdateState(IDictionary<Direction, IComponent>? neighbors)
    {
        // Buttons don't update based on neighbors
    }

    /// <summary>
    /// Gets the power output of this component in the specified direction.
    /// </summary>
    /// <param name="side">The direction to get power output for.</param>
    /// <returns>The power level (0-15) provided in the specified direction.</returns>
    public override int GetPowerOutput(Direction side)
    {
        // Buttons only output power in the direction they're facing
        return side == Orientation ? PowerLevel : 0;
    }

    /// <summary>
    /// Determines if this component can connect to the specified direction.
    /// </summary>
    /// <param name="side">The direction to check.</param>
    /// <returns>True if the component can connect in the specified direction, false otherwise.</returns>
    public override bool CanConnect(Direction side)
    {
        // Buttons can only connect in the direction they're facing
        return side == Orientation;
    }

    /// <summary>
    /// Called when a neighbor changes state.
    /// Buttons don't respond to neighbor changes.
    /// </summary>
    /// <param name="side">The direction of the neighbor that changed.</param>
    public override void OnNeighborChange(Direction side)
    {
        // Buttons don't respond to neighbor changes
    }

    /// <summary>
    /// Creates a deep copy of this component.
    /// </summary>
    /// <returns>A new instance of the RedstoneButton with the same state.</returns>
    public override object Clone()
    {
        var button = new RedstoneButton(_activationDuration)
        {
            Orientation = this.Orientation,
            PowerLevel = this.PowerLevel
        };
        
        if (_isPressed)
        {
            button._isPressed = true;
            button._remainingActivationTime = _remainingActivationTime;
        }
        
        return button;
    }

    /// <summary>
    /// Gets serialization data for this component.
    /// </summary>
    /// <returns>The serialization data for this component.</returns>
    public override ComponentData GetSerializationData()
    {
        var data = base.GetSerializationData();
        data.Properties["ActivationDuration"] = _activationDuration;
        data.Properties["IsPressed"] = _isPressed;
        data.Properties["RemainingActivationTime"] = _remainingActivationTime;
        return data;
    }
}
