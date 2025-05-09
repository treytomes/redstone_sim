using RedstoneSimulator.Core.Common;

namespace RedstoneSimulator.Core.Components;

/// <summary>
/// A test component implementation for unit testing.
/// </summary>
public class TestComponent : ComponentBase
{
    public override ComponentType ComponentType => ComponentType.RedstoneWire;
    
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

    public override IReadOnlyList<Direction> ConnectedSides => new List<Direction> 
    { 
        Direction.North, 
        Direction.East, 
        Direction.South, 
        Direction.West 
    };
    
    public override void UpdateState(IDictionary<Direction, IComponent>? neighbors) { }
    
    public override int GetPowerOutput(Direction side) => PowerLevel;
    
    public override bool CanConnect(Direction side) => true;
    
    public override void OnNeighborChange(Direction side) { }
    
    public override ComponentData GetSerializationData()
    {
        var data = base.GetSerializationData();
        data.Type = "TestComponent";
        return data;
    }
    
    public override object Clone()
    {
        var clone = new TestComponent();
        clone.PowerLevel = this.PowerLevel;
        clone.Orientation = this.Orientation;
        return clone;
    }
}