using RedstoneSimulator.Core.Common;
using RedstoneSimulator.Core.Grid;

namespace RedstoneSimulator.Core.Components;

/// <summary>
/// Mock component implementation for testing purposes.
/// </summary>
public class MockComponent : IComponent
{
    #region Fields

    private readonly ComponentType _componentType;
    private string _testName = "";
    private int _powerLevel;
    private Direction _orientation = Direction.North;
    private readonly List<Direction> _connectedSides;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MockComponent"/> class.
    /// </summary>
    /// <param name="componentType">The type of component to mock.</param>
    /// <param name="powerLevel">The initial power level.</param>
    public MockComponent(ComponentType componentType = Common.ComponentType.RedstoneWire, int powerLevel = 0)
    {
        _componentType = componentType;
        _powerLevel = Math.Clamp(powerLevel, 0, 15);
        _connectedSides = new List<Direction>(DirectionExtensions.CardinalDirections);
        _orientation = Direction.North;
        UpdateCount = 0;
    }

    #endregion

    #region Properties
    
    /// <summary>
    /// Gets the list of directions that this component can connect to.
    /// </summary>
    public IReadOnlyList<Direction> ConnectedSides => _connectedSides;

    /// <summary>
    /// Gets the type of this component.
    /// </summary>
    public ComponentType ComponentType => _componentType;

    /// <summary>
    /// Gets the current power level of this component.
    /// </summary>
    public int PowerLevel => _powerLevel;

    /// <summary>
    /// Gets or sets the orientation of this component.
    /// </summary>
    public Direction Orientation 
    { 
        get => _orientation;
        set => _orientation = value;
    }

    /// <summary>
    /// Gets the number of times the component has been updated.
    /// </summary>
    public int UpdateCount { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether this component can be placed in a cell.
    /// </summary>
    public bool CanBePlaced { get; set; } = true;

    #endregion

    #region Methods

    /// <summary>
    /// Sets the power level of this mock component.
    /// </summary>
    /// <param name="powerLevel">The power level to set.</param>
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
    
    /// <summary>
    /// Updates the state of this component based on its neighbors.
    /// </summary>
    /// <param name="neighbors">Dictionary of neighboring components keyed by direction.</param>
    public void UpdateState(IDictionary<Direction, IComponent>? neighbors)
    {
        UpdateCount++;
    }

    /// <summary>
    /// Gets the power output of this component in the specified direction.
    /// </summary>
    /// <param name="side">The direction to get power output for.</param>
    /// <returns>The power level provided in the specified direction.</returns>
    public int GetPowerOutput(Direction side)
    {
        return _powerLevel;
    }

    /// <summary>
    /// Determines if this component can connect to the specified direction.
    /// </summary>
    /// <param name="side">The direction to check.</param>
    /// <returns>True if the component can connect in the specified direction.</returns>
    public bool CanConnect(Direction side)
    {
        return _connectedSides.Contains(side);
    }

    /// <summary>
    /// Called when a neighbor changes state.
    /// </summary>
    /// <param name="side">The direction of the neighbor that changed.</param>
    public void OnNeighborChange(Direction side)
    {
        // Do nothing for mock
    }

    /// <summary>
    /// Called when this component is added to a cell.
    /// </summary>
    /// <param name="cell">The cell that this component was added to.</param>
    public void OnAddedToCell(Cell cell)
    {
        // Do nothing for mock
    }

    /// <summary>
    /// Called when this component is removed from a cell.
    /// </summary>
    /// <param name="cell">The cell that this component was removed from.</param>
    public void OnRemovedFromCell(Cell cell)
    {
        // Do nothing for mock
    }

    /// <summary>
    /// Updates the component during a simulation tick.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update.</param>
    public void Update(float deltaTime)
    {
        UpdateCount++;
    }

    /// <summary>
    /// Determines if this component can be placed in the specified cell.
    /// </summary>
    /// <param name="cell">The cell to check.</param>
    /// <returns>True if the component can be placed in the cell.</returns>
    public bool CanBePlacedInCell(Cell cell)
    {
        return CanBePlaced;
    }

    /// <summary>
    /// Gets serialization data for this component.
    /// </summary>
    /// <returns>The serialization data for this component.</returns>
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
    
    /// <summary>
    /// Determines if this component can connect to another component in the specified direction.
    /// </summary>
    /// <param name="direction">The direction to check.</param>
    /// <param name="otherComponent">The other component to check for connectivity.</param>
    /// <returns>True if this component can connect to the other component.</returns>
    public bool CanConnectTo(Direction direction, IComponent otherComponent)
    {
        return CanConnect(direction) && otherComponent.CanConnect(direction.Opposite());
    }
    
    public void Dispose() { }

    #endregion
}
