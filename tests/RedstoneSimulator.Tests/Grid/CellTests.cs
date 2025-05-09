using Moq;
using RedstoneSimulator.Core.Grid;
using RedstoneSimulator.Core.Components;
using OpenTK.Mathematics;
using RedstoneSimulator.Core.Common;

namespace RedstoneSimulator.Tests.Grid;

/// <summary>
/// Contains unit tests for the <see cref="Cell"/> class, which represents a single cell
/// in the redstone circuit grid that can contain a component.
/// </summary>
public class CellTests
{
    private Mock<TComponent> CreateMockComponent<TComponent>()
        where TComponent : class, IComponent
    {
        var mockComponent = new Mock<TComponent>();
        mockComponent.Setup(c => c.CanBePlacedInCell(It.IsAny<Cell>())).Returns(true);
        mockComponent.Setup(c => c.Update(It.IsAny<float>()));
        mockComponent.Setup(c => c.OnAddedToCell(It.IsAny<Cell>()));

        // Add any other common setups here
        return mockComponent;
    }

    /// <summary>
    /// Verifies that a newly created cell is empty.
    /// </summary>
    [Fact]
    public void NewCell_IsEmpty()
    {
        // Arrange & Act
        var cell = new Cell(5, 10);
        
        // Assert
        Assert.True(cell.IsEmpty);
    }

    /// <summary>
    /// Verifies that a cell is not empty after setting a component.
    /// </summary>
    [Fact]
    public void SetComponent_CellIsNotEmpty()
    {
        // Arrange
        var cell = new Cell(1, 2);
        var mockComponent = CreateMockComponent<IComponent>();
        
        // Act
        cell.SetComponent(mockComponent.Object);
        
        // Assert
        Assert.False(cell.IsEmpty);
    }

    /// <summary>
    /// Verifies that GetComponent returns the component that was set.
    /// </summary>
    [Fact]
    public void GetComponent_ReturnsSetComponent()
    {
        // Arrange
        var cell = new Cell(1, 2);
        var mockComponent = CreateMockComponent<IComponent>();
        
        // Act
        cell.SetComponent(mockComponent.Object);
        var result = cell.GetComponent();
        
        // Assert
        Assert.Same(mockComponent.Object, result);
    }

    /// <summary>
    /// Verifies that cell coordinates match the values provided in the constructor.
    /// </summary>
    [Fact]
    public void CellCoordinates_MatchConstructorValues()
    {
        // Arrange & Act
        var cell = new Cell(42, 24);
        
        // Assert
        Assert.Equal(42, cell.X);
        Assert.Equal(24, cell.Y);
    }

    /// <summary>
    /// Verifies that SetComponent can replace an existing component.
    /// </summary>
    [Fact]
    public void SetComponent_CanReplaceExistingComponent()
    {
        // Arrange
        var cell = new Cell(1, 2);
        var mockComponent1 = CreateMockComponent<IComponent>();
        var mockComponent2 = CreateMockComponent<IComponent>();
        cell.SetComponent(mockComponent1.Object);
        
        // Act
        cell.SetComponent(mockComponent2.Object);
        var result = cell.GetComponent();
        
        // Assert
        Assert.Same(mockComponent2.Object, result);
        Assert.NotSame(mockComponent1.Object, result);
    }
    
    /// <summary>
    /// Verifies that SetComponent with null clears the component from the cell.
    /// </summary>
    [Fact]
    public void SetComponent_CanClearComponent()
    {
        // Arrange
        var cell = new Cell(1, 2);
        var mockComponent = CreateMockComponent<IComponent>();
        cell.SetComponent(mockComponent.Object);
        
        // Act
        cell.SetComponent(null);
        
        // Assert
        Assert.True(cell.IsEmpty);
        Assert.Null(cell.GetComponent());
    }
    
    /// <summary>
    /// Verifies that GetComponent returns null when the cell is empty.
    /// </summary>
    [Fact]
    public void GetComponent_ReturnsNullWhenEmpty()
    {
        // Arrange
        var cell = new Cell(3, 4);
        
        // Act
        var result = cell.GetComponent();
        
        // Assert
        Assert.Null(result);
    }
    
    /// <summary>
    /// Verifies that cells correctly store coordinates, including negative values.
    /// </summary>
    /// <param name="x">The x-coordinate to test.</param>
    /// <param name="y">The y-coordinate to test.</param>
    [Theory]
    [InlineData(0, 0)]
    [InlineData(-5, 10)]
    [InlineData(100, -200)]
    public void Cell_StoresCoordinatesCorrectly(int x, int y)
    {
        // Arrange & Act
        var cell = new Cell(x, y);
        
        // Assert
        Assert.Equal(x, cell.X);
        Assert.Equal(y, cell.Y);
    }
    
    /// <summary>
    /// Verifies that cells with the same coordinates are considered equal.
    /// </summary>
    [Fact]
    public void Equals_ReturnsTrueForSameCoordinates()
    {
        // Arrange
        var cell1 = new Cell(5, 5);
        var cell2 = new Cell(5, 5);
        
        // Act & Assert
        Assert.Equal(cell1, cell2);
    }
    
    /// <summary>
    /// Verifies that cells with different coordinates are not considered equal.
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseForDifferentCoordinates()
    {
        // Arrange
        var cell1 = new Cell(5, 5);
        var cell2 = new Cell(5, 6);
        
        // Act & Assert
        Assert.NotEqual(cell1, cell2);
    }
    
    /// <summary>
    /// Verifies that cells with the same coordinates have the same hash code.
    /// </summary>
    [Fact]
    public void GetHashCode_SameForEqualCells()
    {
        // Arrange
        var cell1 = new Cell(3, 7);
        var cell2 = new Cell(3, 7);
        
        // Act & Assert
        Assert.Equal(cell1.GetHashCode(), cell2.GetHashCode());
    }
    
    /// <summary>
    /// Verifies that components are notified when added to a cell.
    /// </summary>
    [Fact]
    public void SetComponent_NotifiesComponentWhenAdded()
    {
        // Arrange
        var cell = new Cell(5, 8);
        var mockComponent = CreateMockComponent<IComponent>();
        
        // Act
        cell.SetComponent(mockComponent.Object);
        
        // Assert
        mockComponent.Verify(c => c.OnAddedToCell(cell), Times.Once);
    }
    
    /// <summary>
    /// Verifies that components are notified when removed from a cell.
    /// </summary>
    [Fact]
    public void SetComponent_NotifiesComponentWhenRemoved()
    {
        // Arrange
        var cell = new Cell(5, 8);
        var mockComponent = CreateMockComponent<IComponent>();
        cell.SetComponent(mockComponent.Object);
        
        // Act
        cell.SetComponent(null);
        
        // Assert
        mockComponent.Verify(c => c.OnRemovedFromCell(cell), Times.Once);
    }
    
    /// <summary>
    /// Verifies that components are notified when replaced by another component.
    /// </summary>
    [Fact]
    public void SetComponent_NotifiesOldComponentWhenReplaced()
    {
        // Arrange
        var cell = new Cell(5, 8);
        var mockComponent1 = CreateMockComponent<IComponent>();
        var mockComponent2 = CreateMockComponent<IComponent>();
        cell.SetComponent(mockComponent1.Object);
        
        // Act
        cell.SetComponent(mockComponent2.Object);
        
        // Assert
        mockComponent1.Verify(c => c.OnRemovedFromCell(cell), Times.Once);
        mockComponent2.Verify(c => c.OnAddedToCell(cell), Times.Once);
    }
    
    /// <summary>
    /// Verifies that comparing a cell with a non-Cell object returns false.
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseForDifferentTypes()
    {
        // Arrange
        var cell = new Cell(1, 2);
        var notACell = new object();
        
        // Act & Assert
        Assert.False(cell.Equals(notACell));
    }
    
    /// <summary>
    /// Verifies that comparing a cell with null returns false.
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseForNull()
    {
        // Arrange
        var cell = new Cell(1, 2);
        
        // Act & Assert
        Assert.False(cell.Equals(null));
    }
    
    /// <summary>
    /// Verifies that a cell equals itself (reference equality).
    /// </summary>
    [Fact]
    public void Equals_ReturnsTrueForSameReference()
    {
        // Arrange
        var cell = new Cell(1, 2);
        
        // Act & Assert
        Assert.True(cell.Equals(cell));
    }
    
    /// <summary>
    /// Verifies that ToString returns a meaningful representation of the cell.
    /// </summary>
    [Fact]
    public void ToString_ReturnsCoordinateRepresentation()
    {
        // Arrange
        var cell = new Cell(3, 4);
        
        // Act
        var result = cell.ToString();
        
        // Assert
        Assert.Contains("3", result);
        Assert.Contains("4", result);
    }
    
    /// <summary>
    /// Verifies that the cell rejects invalid components if validation is implemented.
    /// </summary>
    [Fact]
    public void SetComponent_RejectsInvalidComponents()
    {
        // Arrange
        var cell = new Cell(1, 2);
        var mockInvalidComponent = CreateMockComponent<IComponent>();
        mockInvalidComponent.Setup(c => c.CanBePlacedInCell(It.IsAny<Cell>())).Returns(false);
        
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            cell.SetComponent(mockInvalidComponent.Object));
        
        Assert.Contains("cannot be placed", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Verifies that cell state tracking works correctly if implemented.
    /// </summary>
    [Fact]
    public void Cell_TracksAdditionalState()
    {
        // Arrange
        var cell = new Cell(1, 2);
        
        // Act
        cell.SetState("PowerLevel", 15);
        var result = cell.GetState<int>("PowerLevel");
        
        // Assert
        Assert.Equal(15, result);
    }
    
    /// <summary>
    /// Verifies that cell events are raised when components change.
    /// </summary>
    [Fact]
    public void Cell_RaisesEventsOnComponentChange()
    {
        // Arrange
        var cell = new Cell(1, 2);
        var mockComponent = CreateMockComponent<IComponent>();
        
        bool eventRaised = false;
        cell.ComponentChanged += (sender, args) => eventRaised = true;
        
        // Act
        cell.SetComponent(mockComponent.Object);
        
        // Assert
        Assert.True(eventRaised);
    }
    
    /// <summary>
    /// Verifies that cells can be cloned or copied correctly.
    /// </summary>
    [Fact]
    public void Cell_CanBeCloned()
    {
        // Arrange
        var originalCell = new Cell(1, 2);
        var mockComponent = CreateMockComponent<IComponent>();
        originalCell.SetComponent(mockComponent.Object);
        
        // Act
        var clonedCell = originalCell.Clone();
        
        // Assert
        Assert.Equal(originalCell.X, clonedCell.X);
        Assert.Equal(originalCell.Y, clonedCell.Y);
        Assert.NotSame(originalCell, clonedCell);
        
        // Note: Component cloning behavior depends on implementation details
        // This test assumes components are cloned or referenced based on design
    }
    
    /// <summary>
    /// Verifies that cells behave correctly with concurrent access if thread safety is implemented.
    /// </summary>
    [Fact]
    public void Cell_HandlesConcurrentAccess()
    {
        // Arrange
        var cell = new Cell(1, 2);
        var mockComponent1 = CreateMockComponent<IComponent>();
        var mockComponent2 = CreateMockComponent<IComponent>();
        bool exceptionThrown = false;
        
        // Act
        try
        {
            // Simulate concurrent access by running multiple operations
            Parallel.Invoke(
                () => cell.SetComponent(mockComponent1.Object),
                () => cell.SetComponent(mockComponent2.Object),
                () => cell.GetComponent(),
                () => {
                    var isEmpty = cell.IsEmpty;
                }
            );
        }
        catch
        {
            exceptionThrown = true;
        }
        
        // Assert
        Assert.False(exceptionThrown, "Cell should handle concurrent access without exceptions");
        
        // Note: This test primarily verifies that no exceptions are thrown during concurrent access
        // More detailed thread safety tests would depend on specific threading guarantees
    }
    
    /// <summary>
    /// Verifies that cells behave correctly with components that have specific requirements.
    /// </summary>
    [Fact]
    public void Cell_HandlesComponentSpecificBehavior()
    {
        // Arrange
        var cell = new Cell(1, 2);
        var mockWireComponent = CreateMockComponent<IWireComponent>();
        mockWireComponent.Setup(w => w.GetComponentType()).Returns(ComponentType.RedstoneWire);
        
        // Act
        cell.SetComponent(mockWireComponent.Object);
        var componentType = cell.GetComponentType();
        
        // Assert
        Assert.Equal(ComponentType.RedstoneWire, componentType);
    }
    
    /// <summary>
    /// Verifies that cell operations perform within acceptable time limits.
    /// </summary>
    [Fact]
    public void Cell_OperationsPerformEfficiently()
    {
        // Arrange
        const int operationCount = 10000;
        var cell = new Cell(1, 2);
        var mockComponent = CreateMockComponent<IComponent>();
        var stopwatch = new System.Diagnostics.Stopwatch();
        
        // Act
        stopwatch.Start();
        for (int i = 0; i < operationCount; i++)
        {
            cell.SetComponent(mockComponent.Object);
            var component = cell.GetComponent();
            var isEmpty = cell.IsEmpty;
        }
        stopwatch.Stop();
        
        // Assert
        // This is a performance guideline - adjust threshold as needed
        Assert.True(stopwatch.ElapsedMilliseconds < 500, 
            $"Cell operations should complete quickly. Took {stopwatch.ElapsedMilliseconds}ms for {operationCount} operations");
    }
    
    /// <summary>
    /// Verifies that cells can be serialized and deserialized correctly.
    /// </summary>
    [Fact]
    public void Cell_CanBeSerializedAndDeserialized()
    {
        // Arrange
        var originalCell = new Cell(5, 10);
        var mockComponent = CreateMockComponent<IComponent>();
        mockComponent.Setup(c => c.GetSerializationData()).Returns(new ComponentData { Type = "TestComponent" });
        originalCell.SetComponent(mockComponent.Object);
        
        // Act
        var serialized = originalCell.Serialize();
        var deserializedCell = Cell.Deserialize(serialized);
        
        // Assert
        Assert.Equal(originalCell.X, deserializedCell.X);
        Assert.Equal(originalCell.Y, deserializedCell.Y);
        
        // Instead of checking the type name, check the component type or another property
        var component = deserializedCell.GetComponent();
        Assert.NotNull(component);
        
        // Check if the component was created from TestComponent data
        var componentData = component.GetSerializationData();
        Assert.Equal("TestComponent", componentData.Type);
    }
    
    /// <summary>
    /// Verifies that cells properly handle component power level changes.
    /// </summary>
    [Fact]
    public void Cell_TracksComponentPowerLevel()
    {
        // Arrange
        var cell = new Cell(3, 4);
        var mockPowerableComponent = CreateMockComponent<IPowerableComponent>();
        mockPowerableComponent.Setup(c => c.PowerLevel).Returns(15);
        
        // Act
        cell.SetComponent(mockPowerableComponent.Object);
        var powerLevel = cell.GetPowerLevel();
        
        // Assert
        Assert.Equal(15, powerLevel);
    }
    
    /// <summary>
    /// Verifies that cells correctly handle component rotation.
    /// </summary>
    [Fact]
    public void Cell_HandlesComponentRotation()
    {
        // Arrange
        var cell = new Cell(3, 4);
        var mockDirectionalComponent = CreateMockComponent<IDirectionalComponent>();
        
        // Act
        cell.SetComponent(mockDirectionalComponent.Object);
        cell.RotateComponent(Direction.North);
        
        // Assert
        mockDirectionalComponent.Verify(c => c.SetDirection(Direction.North), Times.Once);
    }
    
    /// <summary>
    /// Verifies that cells correctly report their neighbors based on grid position.
    /// </summary>
    [Fact]
    public void Cell_IdentifiesNeighborPositions()
    {
        // Arrange
        var cell = new Cell(5, 5);
        
        // Act
        var neighborPositions = cell.GetNeighborPositions();
        
        // Assert
        Assert.Contains(new Vector2i(4, 5), neighborPositions);  // West
        Assert.Contains(new Vector2i(6, 5), neighborPositions);  // East
        Assert.Contains(new Vector2i(5, 4), neighborPositions);  // North
        Assert.Contains(new Vector2i(5, 6), neighborPositions);  // South
        Assert.Equal(4, neighborPositions.Count);
    }
    
    /// <summary>
    /// Verifies that cells can correctly determine if they can connect to another cell.
    /// </summary>
    [Fact]
    public void Cell_DeterminesConnectivity()
    {
        // Arrange
        var cell1 = new Cell(1, 1);
        var cell2 = new Cell(1, 2);
        
        var mockConnectableComponent1 = CreateMockComponent<IConnectableComponent>();
        var mockConnectableComponent2 = CreateMockComponent<IConnectableComponent>();
        
        mockConnectableComponent1.Setup(c => c.CanConnectTo(It.IsAny<Direction>(), It.IsAny<IComponent>()))
            .Returns(true);
            
        cell1.SetComponent(mockConnectableComponent1.Object);
        cell2.SetComponent(mockConnectableComponent2.Object);
        
        // Act
        var canConnect = cell1.CanConnectTo(Direction.South, cell2);
        
        // Assert
        Assert.True(canConnect);
        mockConnectableComponent1.Verify(c => c.CanConnectTo(Direction.South, mockConnectableComponent2.Object), Times.Once);
    }
    
    /// <summary>
    /// Verifies that cells correctly handle component activation.
    /// </summary>
    [Fact]
    public void Cell_ActivatesComponent()
    {
        // Arrange
        var cell = new Cell(3, 4);
        var mockActivatableComponent = CreateMockComponent<IActivatableComponent>();
        cell.SetComponent(mockActivatableComponent.Object);
        
        // Act
        cell.Activate();
        
        // Assert
        mockActivatableComponent.Verify(c => c.Activate(), Times.Once);
    }
    
    /// <summary>
    /// Verifies that cells correctly handle component updates during simulation ticks.
    /// </summary>
    [Fact]
    public void Cell_UpdatesComponentDuringSimulationTick()
    {
        // Arrange
        var cell = new Cell(3, 4);
        
        // Create mock with explicit setup for this test
        var mockComponent = new Mock<IComponent>(MockBehavior.Strict);
        mockComponent.Setup(c => c.CanBePlacedInCell(It.IsAny<Cell>())).Returns(true);
        mockComponent.Setup(c => c.Update(It.IsAny<float>()));
        mockComponent.Setup(c => c.OnAddedToCell(It.IsAny<Cell>()));
        
        cell.SetComponent(mockComponent.Object);
        
        // Act
        cell.Update(1.0f);  // Update with delta time of 1.0
        
        // Assert
        mockComponent.Verify(c => c.Update(1.0f), Times.Once);
    }
    
    /// <summary>
    /// Verifies that cells correctly handle invalid operations.
    /// </summary>
    [Fact]
    public void Cell_ThrowsExceptionForInvalidOperations()
    {
        // Arrange
        var cell = new Cell(1, 2);
        
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            cell.PerformSpecialAction("InvalidAction"));
            
        Assert.Contains("unsupported action", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Verifies that cells can be marked for update in the simulation.
    /// </summary>
    [Fact]
    public void Cell_CanBeMarkedForUpdate()
    {
        // Arrange
        var cell = new Cell(1, 2);
        bool updateHandlerCalled = false;
        
        cell.MarkedForUpdate += (sender, args) => updateHandlerCalled = true;
        
        // Act
        cell.MarkForUpdate();
        
        // Assert
        Assert.True(updateHandlerCalled);
        Assert.True(cell.NeedsUpdate);
    }
    
    /// <summary>
    /// Verifies that cells correctly implement value equality.
    /// </summary>
    [Fact]
    public void Cell_ImplementsValueEquality()
    {
        // Arrange
        var cell1 = new Cell(5, 5);
        var cell2 = new Cell(5, 5);
        var cell3 = new Cell(6, 5);
        
        // Act & Assert
        Assert.True(cell1 == cell2);
        Assert.False(cell1 == cell3);
        Assert.False(cell1 != cell2);
        Assert.True(cell1 != cell3);
    }
    
    /// <summary>
    /// Verifies that cells can be used as dictionary keys correctly.
    /// </summary>
    [Fact]
    public void Cell_WorksAsADictionaryKey()
    {
        // Arrange
        var dictionary = new Dictionary<Cell, string>();
        var cell1 = new Cell(1, 2);
        var cell2 = new Cell(1, 2);  // Same coordinates as cell1
        var cell3 = new Cell(3, 4);
        
        // Act
        dictionary[cell1] = "Value1";
        dictionary[cell3] = "Value3";
        
        // Assert
        Assert.Equal("Value1", dictionary[cell2]);  // Should retrieve using equivalent cell
        Assert.Equal(2, dictionary.Count);          // Should not duplicate for equivalent cells
    }

    [Fact]
    public void Constructor_SetsCoordinatesCorrectly()
    {
        // Arrange & Act
        var cell = new Cell(5, 10);
        
        // Assert
        Assert.Equal(5, cell.X);
        Assert.Equal(10, cell.Y);
        Assert.True(cell.IsEmpty);
        Assert.Null(cell.Component);
    }
    
    [Fact]
    public void SetComponent_ValidComponent_SetsComponentCorrectly()
    {
        // Arrange
        var cell = new Cell(5, 10);
        var component = new MockComponent();
        
        // Act
        cell.SetComponent(component);
        
        // Assert
        Assert.Same(component, cell.Component);
        Assert.False(cell.IsEmpty);
    }
    
    [Fact]
    public void SetComponent_RaisesComponentChangedEvent()
    {
        // Arrange
        var cell = new Cell(5, 10);
        var component = new MockComponent();
        ComponentChangedEventArgs? eventArgs = null;
        cell.ComponentChanged += (sender, e) => eventArgs = e;
        
        // Act
        cell.SetComponent(component);
        
        // Assert
        Assert.NotNull(eventArgs);
        Assert.Null(eventArgs.OldComponent);
        Assert.Same(component, eventArgs.NewComponent);
    }
    
    [Fact]
    public void SetComponent_ReplacingExistingComponent_RaisesEventWithBothComponents()
    {
        // Arrange
        var cell = new Cell(5, 10);
        var component1 = new MockComponent();
        var component2 = new MockComponent();
        cell.SetComponent(component1);
        
        ComponentChangedEventArgs? eventArgs = null;
        cell.ComponentChanged += (sender, e) => eventArgs = e;
        
        // Act
        cell.SetComponent(component2);
        
        // Assert
        Assert.NotNull(eventArgs);
        Assert.Same(component1, eventArgs.OldComponent);
        Assert.Same(component2, eventArgs.NewComponent);
    }
    
    [Fact]
    public void SetComponent_Null_ClearsComponent()
    {
        // Arrange
        var cell = new Cell(5, 10);
        var component = new MockComponent();
        cell.SetComponent(component);
        
        // Act
        cell.SetComponent(null);
        
        // Assert
        Assert.Null(cell.Component);
        Assert.True(cell.IsEmpty);
    }
    
    [Fact]
    public void SetComponent_ComponentCannotBePlaced_ThrowsInvalidOperationException()
    {
        // Arrange
        var cell = new Cell(5, 10);
        var component = new MockComponent { CanBePlaced = false };
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => cell.SetComponent(component));
    }
    
    [Fact]
    public void MarkForUpdate_RaisesMarkedForUpdateEvent()
    {
        // Arrange
        var cell = new Cell(5, 10);
        bool eventRaised = false;
        cell.MarkedForUpdate += (sender, e) => eventRaised = true;
        
        // Act
        cell.MarkForUpdate();
        
        // Assert
        Assert.True(eventRaised);
        Assert.True(cell.NeedsUpdate);
    }
    
    [Fact]
    public void Update_ResetsNeedsUpdateFlag()
    {
        // Arrange
        var cell = new Cell(5, 10);
        var component = new MockComponent();
        cell.SetComponent(component);
        cell.MarkForUpdate();
        
        // Act
        cell.Update(0.1f);
        
        // Assert
        Assert.False(cell.NeedsUpdate);
        Assert.Equal(1, component.UpdateCount);
    }
    
    [Fact]
    public void GetNeighborPositions_ReturnsCorrectPositions()
    {
        // Arrange
        var cell = new Cell(5, 10);
        
        // Act
        var neighborPositions = cell.GetNeighborPositions();
        
        // Assert
        Assert.Equal(4, neighborPositions.Count);
        Assert.Contains(new Vector2i(4, 10), neighborPositions); // West
        Assert.Contains(new Vector2i(6, 10), neighborPositions); // East
        Assert.Contains(new Vector2i(5, 9), neighborPositions);  // North
        Assert.Contains(new Vector2i(5, 11), neighborPositions); // South
    }
    
    [Fact]
    public void Equals_SameCoordinates_ReturnsTrue()
    {
        // Arrange
        var cell1 = new Cell(5, 10);
        var cell2 = new Cell(5, 10);
        
        // Act & Assert
        Assert.True(cell1.Equals(cell2));
        Assert.True(cell1 == cell2);
        Assert.False(cell1 != cell2);
        Assert.Equal(cell1.GetHashCode(), cell2.GetHashCode());
    }
    
    [Fact]
    public void Equals_DifferentCoordinates_ReturnsFalse()
    {
        // Arrange
        var cell1 = new Cell(5, 10);
        var cell2 = new Cell(6, 10);
        var cell3 = new Cell(5, 11);
        
        // Act & Assert
        Assert.False(cell1.Equals(cell2));
        Assert.False(cell1 == cell2);
        Assert.True(cell1 != cell2);
        
        Assert.False(cell1.Equals(cell3));
        Assert.False(cell1 == cell3);
        Assert.True(cell1 != cell3);
    }
    
    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var emptyCell = new Cell(5, 10);
        var componentCell = new Cell(7, 12);
        componentCell.SetComponent(new MockComponent());
        
        // Act
        string emptyString = emptyCell.ToString();
        string componentString = componentCell.ToString();
        
        // Assert
        Assert.Contains("Cell(5, 10)", emptyString);
        Assert.Contains("Empty", emptyString);
        Assert.Contains("Cell(7, 12)", componentString);
        Assert.Contains("MockComponent", componentString);
    }
}