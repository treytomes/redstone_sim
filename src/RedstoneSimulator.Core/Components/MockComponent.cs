using RedstoneSimulator.Core.Common;
using RedstoneSimulator.Core.Grid;

namespace RedstoneSimulator.Core.Components;

/// <summary>
/// Mock component implementation for testing purposes.
/// </summary>
public class MockComponent : IComponent
{
    private readonly ComponentType _componentType;
    private string _testName = "";
    private int _powerLevel;
    private Direction _orientation = Direction.North;
    
    public MockComponent(ComponentType componentType)
    {
        _componentType = componentType;
    }
    
    public ComponentType ComponentType => _componentType;
    
    public int PowerLevel => _powerLevel;
    
    public Direction Orientation 
    { 
        get => _orientation;
        set => _orientation = value;
    }
    
    public IReadOnlyList<Direction> ConnectedSides => new List<Direction> 
    { 
        Direction.North, 
        Direction.East, 
        Direction.South, 
        Direction.West 
    };
    
    public void SetPowerLevel(int powerLevel)
    {
        _powerLevel = Math.Clamp(powerLevel, 0, 15);
    }
    
    public void SetOrientation(Direction orientation)
    {
        _orientation = orientation;
    }
    
    public void SetTestName(string name)
    {
        _testName = name;
    }
    
    public string GetTypeName()
    {
        return string.IsNullOrEmpty(_testName) ? _componentType.ToString() : _testName;
    }
    
    public void UpdateState(IDictionary<Direction, IComponent>? neighbors) { }
    
    public int GetPowerOutput(Direction side) => PowerLevel;
    
    public bool CanConnect(Direction side) => true;
    
    public void OnNeighborChange(Direction side) { }
    
    public void OnAddedToCell(Cell cell) { }
    
    public void OnRemovedFromCell(Cell cell) { }
    
    public void Update(float deltaTime) { }
    
    public bool CanBePlacedInCell(Cell cell) => true;
    
    public ComponentData GetSerializationData()
    {
        return new ComponentData
        {
            Type = GetTypeName(),
            Properties = new Dictionary<string, object>
            {
                ["PowerLevel"] = PowerLevel,
                ["Orientation"] = (int)Orientation
            }
        };
    }
    
    public bool CanConnectTo(Direction direction, IComponent otherComponent) => true;
    
    public void Dispose() { }
}
