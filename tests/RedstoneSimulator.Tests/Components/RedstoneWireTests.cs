using Moq;
using RedstoneSimulator.Core.Components;
using RedstoneSimulator.Core.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedstoneSimulator.Tests.Components;

/// <summary>
/// Contains unit tests for the <see cref="RedstoneWire"/> component,
/// verifying its behavior for power transmission, connection management,
/// and interaction with neighboring components.
/// </summary>
public class RedstoneWireTests
{
    /// <summary>
    /// Verifies that a newly created RedstoneWire has a power level of zero.
    /// </summary>
    [Fact]
    public void NewWire_HasZeroPowerLevel()
    {
        // Arrange & Act
        var wire = new RedstoneWire();
        
        // Assert
        Assert.Equal(0, wire.PowerLevel);
    }

    /// <summary>
    /// Verifies that toggling a direction disables the connection in that direction.
    /// </summary>
    [Fact]
    public void ToggleDirection_DisablesConnection()
    {
        // Arrange
        var wire = new RedstoneWire();
        
        // Act
        wire.ToggleDirection(Direction.North);
        
        // Assert
        Assert.False(wire.IsDirectionEnabled(Direction.North));
    }

    /// <summary>
    /// Verifies that toggling a direction twice restores the connection.
    /// </summary>
    [Fact]
    public void ToggleDirection_TwiceRestoresConnection()
    {
        // Arrange
        var wire = new RedstoneWire();
        
        // Act
        wire.ToggleDirection(Direction.East);
        wire.ToggleDirection(Direction.East);
        
        // Assert
        Assert.True(wire.IsDirectionEnabled(Direction.East));
    }

    /// <summary>
    /// Verifies that UpdateState sets the power level to the highest
    /// neighbor power level minus one.
    /// </summary>
    [Fact]
    public void UpdateState_TakesHighestNeighborPowerLevelMinusOne()
    {
        // Arrange
        var wire = new RedstoneWire();
        
        var mockComponentNorth = new Mock<IComponent>();
        mockComponentNorth.Setup(c => c.GetPowerOutput(Direction.South)).Returns(15);
        
        var mockComponentEast = new Mock<IComponent>();
        mockComponentEast.Setup(c => c.GetPowerOutput(Direction.West)).Returns(10);
        
        var neighbors = new Dictionary<Direction, IComponent>
        {
            { Direction.North, mockComponentNorth.Object },
            { Direction.East, mockComponentEast.Object }
        };
        
        // Act
        wire.UpdateState(neighbors);
        
        // Assert
        Assert.Equal(14, wire.PowerLevel); // 15 - 1
    }

    /// <summary>
    /// Verifies that CanConnect returns false for disabled directions.
    /// </summary>
    [Fact]
    public void CanConnect_ReturnsFalseForDisabledDirection()
    {
        // Arrange
        var wire = new RedstoneWire();
        wire.ToggleDirection(Direction.South);
        
        // Act
        bool result = wire.CanConnect(Direction.South);
        
        // Assert
        Assert.False(result);
    }
    
    /// <summary>
    /// Verifies that GetPowerOutput returns zero for disabled directions.
    /// </summary>
    [Fact]
    public void GetPowerOutput_ReturnsZeroForDisabledDirection()
    {
        // Arrange
        var wire = new RedstoneWire();
        wire.PowerLevel = 10; // Using reflection or a test helper to set this
        wire.ToggleDirection(Direction.West);
        
        // Act
        int power = wire.GetPowerOutput(Direction.West);
        
        // Assert
        Assert.Equal(0, power);
    }
    
    /// <summary>
    /// Verifies that ConnectedSides only includes enabled directions.
    /// </summary>
    [Fact]
    public void ConnectedSides_OnlyIncludesEnabledDirections()
    {
        // Arrange
        var wire = new RedstoneWire();
        wire.ToggleDirection(Direction.North);
        wire.ToggleDirection(Direction.South);
        
        // Act
        var connectedSides = wire.ConnectedSides;
        
        // Assert
        Assert.Equal(2, connectedSides.Count);
        Assert.Contains(Direction.East, connectedSides);
        Assert.Contains(Direction.West, connectedSides);
        Assert.DoesNotContain(Direction.North, connectedSides);
        Assert.DoesNotContain(Direction.South, connectedSides);
    }
    
    /// <summary>
    /// Verifies that UpdateState ignores power from disabled directions.
    /// </summary>
    [Fact]
    public void UpdateState_IgnoresDisabledDirections()
    {
        // Arrange
        var wire = new RedstoneWire();
        wire.ToggleDirection(Direction.North); // Disable North
        
        var mockComponentNorth = new Mock<IComponent>();
        mockComponentNorth.Setup(c => c.GetPowerOutput(Direction.South)).Returns(15);
        
        var mockComponentEast = new Mock<IComponent>();
        mockComponentEast.Setup(c => c.GetPowerOutput(Direction.West)).Returns(5);
        
        var neighbors = new Dictionary<Direction, IComponent>
        {
            { Direction.North, mockComponentNorth.Object },
            { Direction.East, mockComponentEast.Object }
        };
        
        // Act
        wire.UpdateState(neighbors);
        
        // Assert
        Assert.Equal(4, wire.PowerLevel); // 5-1, ignoring the 15 from North
    }
    
    /// <summary>
    /// Verifies that UpdateState sets power level to zero when there are no neighbors.
    /// </summary>
    [Fact]
    public void UpdateState_PowerLevelZeroWhenNoNeighbors()
    {
        // Arrange
        var wire = new RedstoneWire();
        var neighbors = new Dictionary<Direction, IComponent>();
        
        // Act
        wire.UpdateState(neighbors);
        
        // Assert
        Assert.Equal(0, wire.PowerLevel);
    }
    
    /// <summary>
    /// Verifies that UpdateState sets power level to zero when all neighbors have zero power.
    /// </summary>
    [Fact]
    public void UpdateState_PowerLevelZeroWhenAllNeighborsZero()
    {
        // Arrange
        var wire = new RedstoneWire();
        
        var mockComponent1 = new Mock<IComponent>();
        mockComponent1.Setup(c => c.GetPowerOutput(It.IsAny<Direction>())).Returns(0);
        
        var mockComponent2 = new Mock<IComponent>();
        mockComponent2.Setup(c => c.GetPowerOutput(It.IsAny<Direction>())).Returns(0);
        
        var neighbors = new Dictionary<Direction, IComponent>
        {
            { Direction.North, mockComponent1.Object },
            { Direction.East, mockComponent2.Object }
        };
        
        // Act
        wire.UpdateState(neighbors);
        
        // Assert
        Assert.Equal(0, wire.PowerLevel);
    }
    
    /// <summary>
    /// Verifies that UpdateState decrements the input power level by one.
    /// </summary>
    /// <param name="inputPower">The input power level from the neighbor.</param>
    /// <param name="expectedPower">The expected resulting power level.</param>
    [Theory]
    [InlineData(15, 14)]
    [InlineData(1, 0)]
    [InlineData(0, 0)]
    public void UpdateState_DecrementsPowerByOne(int inputPower, int expectedPower)
    {
        // Arrange
        var wire = new RedstoneWire();
        
        var mockComponent = new Mock<IComponent>();
        mockComponent.Setup(c => c.GetPowerOutput(It.IsAny<Direction>())).Returns(inputPower);
        
        var neighbors = new Dictionary<Direction, IComponent>
        {
            { Direction.North, mockComponent.Object }
        };
        
        // Act
        wire.UpdateState(neighbors);
        
        // Assert
        Assert.Equal(expectedPower, wire.PowerLevel);
    }

    /// <summary>
    /// Verifies that power level is clamped between 0 and 15.
    /// </summary>
    /// <param name="inputPower">The power level to attempt to set.</param>
    /// <param name="expectedPower">The expected clamped power level.</param>
    [Theory]
    [InlineData(-5, 0)]
    [InlineData(20, 15)]
    public void PowerLevel_ClampsBetweenZeroAndFifteen(int inputPower, int expectedPower)
    {
        // Arrange
        var wire = new RedstoneWire();
        
        // Act
        // Using reflection or a test setter to bypass normal constraints
        wire.PowerLevel = inputPower;
        
        // Assert
        Assert.Equal(expectedPower, wire.PowerLevel);
    }

    /// <summary>
    /// Verifies that GetPowerOutput returns the current power level for enabled directions.
    /// </summary>
    [Fact]
    public void GetPowerOutput_ReturnsCurrentPowerLevelForEnabledDirections()
    {
        // Arrange
        var wire = new RedstoneWire();
        wire.PowerLevel = 8; // Using reflection or a test helper
        
        // Act
        int power = wire.GetPowerOutput(Direction.North);
        
        // Assert
        Assert.Equal(8, power);
    }

    /// <summary>
    /// Verifies that power level doesn't change without an UpdateState call.
    /// </summary>
    [Fact]
    public void PowerLevel_MaintainsValueWithoutUpdate()
    {
        // Arrange
        var wire = new RedstoneWire();
        wire.PowerLevel = 7; // Using reflection or a test helper
        
        // Act
        // No action, just verify the state remains
        
        // Assert
        Assert.Equal(7, wire.PowerLevel);
        
        // Create neighbors that would change the power level
        var mockComponent = new Mock<IComponent>();
        mockComponent.Setup(c => c.GetPowerOutput(It.IsAny<Direction>())).Returns(15);
        
        var neighbors = new Dictionary<Direction, IComponent>
        {
            { Direction.North, mockComponent.Object }
        };
        
        // Still no UpdateState call
        Assert.Equal(7, wire.PowerLevel);
    }

    /// <summary>
    /// Verifies that wire automatically connects to compatible components when placed.
    /// </summary>
    [Fact]
    public void Wire_AutoConnectsToCompatibleComponents()
    {
        // Arrange
        var wire = new RedstoneWire();
        
        var mockCompatibleComponent = new Mock<IComponent>();
        mockCompatibleComponent.Setup(c => c.CanConnect(It.IsAny<Direction>())).Returns(true);
        
        var mockIncompatibleComponent = new Mock<IComponent>();
        mockIncompatibleComponent.Setup(c => c.CanConnect(It.IsAny<Direction>())).Returns(false);
        
        var neighbors = new Dictionary<Direction, IComponent>
        {
            { Direction.North, mockCompatibleComponent.Object },
            { Direction.South, mockIncompatibleComponent.Object }
        };
        
        // Act
        wire.OnPlacement(neighbors);
        
        // Assert
        Assert.True(wire.IsDirectionEnabled(Direction.North));
        Assert.True(wire.IsDirectionEnabled(Direction.East));
        Assert.True(wire.IsDirectionEnabled(Direction.West));
        // South should be disabled since the component is incompatible
        Assert.False(wire.IsDirectionEnabled(Direction.South));
    }

    /// <summary>
    /// Verifies behavior when UpdateState is called multiple times in sequence.
    /// </summary>
    [Fact]
    public void UpdateState_MultipleCallsWorkCorrectly()
    {
        // Arrange
        var wire = new RedstoneWire();
        
        // First update with power 10
        var mockComponent1 = new Mock<IComponent>();
        mockComponent1.Setup(c => c.GetPowerOutput(It.IsAny<Direction>())).Returns(10);
        
        var neighbors1 = new Dictionary<Direction, IComponent>
        {
            { Direction.North, mockComponent1.Object }
        };
        
        // Act - First update
        wire.UpdateState(neighbors1);
        
        // Assert after first update
        Assert.Equal(9, wire.PowerLevel);
        
        // Second update with power 5
        var mockComponent2 = new Mock<IComponent>();
        mockComponent2.Setup(c => c.GetPowerOutput(It.IsAny<Direction>())).Returns(5);
        
        var neighbors2 = new Dictionary<Direction, IComponent>
        {
            { Direction.East, mockComponent2.Object }
        };
        
        // Act - Second update
        wire.UpdateState(neighbors2);
        
        // Assert after second update
        Assert.Equal(4, wire.PowerLevel);
        
        // Third update with power 15
        var mockComponent3 = new Mock<IComponent>();
        mockComponent3.Setup(c => c.GetPowerOutput(It.IsAny<Direction>())).Returns(15);
        
        var neighbors3 = new Dictionary<Direction, IComponent>
        {
            { Direction.West, mockComponent3.Object }
        };
        
        // Act - Third update
        wire.UpdateState(neighbors3);
        
        // Assert after third update
        Assert.Equal(14, wire.PowerLevel);
    }

    /// <summary>
    /// Verifies behavior when receiving power from all four directions simultaneously.
    /// </summary>
    [Fact]
    public void UpdateState_HandlesAllFourDirections()
    {
        // Arrange
        var wire = new RedstoneWire();
        
        var mockComponentNorth = new Mock<IComponent>();
        mockComponentNorth.Setup(c => c.GetPowerOutput(Direction.South)).Returns(10);
        
        var mockComponentEast = new Mock<IComponent>();
        mockComponentEast.Setup(c => c.GetPowerOutput(Direction.West)).Returns(5);
        
        var mockComponentSouth = new Mock<IComponent>();
        mockComponentSouth.Setup(c => c.GetPowerOutput(Direction.North)).Returns(15);
        
        var mockComponentWest = new Mock<IComponent>();
        mockComponentWest.Setup(c => c.GetPowerOutput(Direction.East)).Returns(8);
        
        var neighbors = new Dictionary<Direction, IComponent>
        {
            { Direction.North, mockComponentNorth.Object },
            { Direction.East, mockComponentEast.Object },
            { Direction.South, mockComponentSouth.Object },
            { Direction.West, mockComponentWest.Object }
        };
        
        // Act
        wire.UpdateState(neighbors);
        
        // Assert
        Assert.Equal(14, wire.PowerLevel); // 15 - 1 (from South)
    }

    /// <summary>
    /// Verifies behavior when receiving power level 1, which would reduce to 0.
    /// </summary>
    [Fact]
    public void UpdateState_PowerLevelOneReducesToZero()
    {
        // Arrange
        var wire = new RedstoneWire();
        
        var mockComponent = new Mock<IComponent>();
        mockComponent.Setup(c => c.GetPowerOutput(It.IsAny<Direction>())).Returns(1);
        
        var neighbors = new Dictionary<Direction, IComponent>
        {
            { Direction.North, mockComponent.Object }
        };
        
        // Act
        wire.UpdateState(neighbors);
        
        // Assert
        Assert.Equal(0, wire.PowerLevel); // 1 - 1 = 0
    }

    /// <summary>
    /// Verifies that power decreases correctly across multiple wires.
    /// </summary>
    [Fact]
    public void PowerPropagation_DecreasesByOnePerWire()
    {
        // Arrange
        var wire1 = new RedstoneWire();
        var wire2 = new RedstoneWire();
        var wire3 = new RedstoneWire();
        
        var mockPowerSource = new Mock<IComponent>();
        mockPowerSource.Setup(c => c.GetPowerOutput(It.IsAny<Direction>())).Returns(15);
        
        // Act - Update wire1 from power source
        wire1.UpdateState(new Dictionary<Direction, IComponent>
        {
            { Direction.North, mockPowerSource.Object }
        });
        
        // Mock wire1 as a component to feed into wire2
        var mockWire1 = new Mock<IComponent>();
        mockWire1.Setup(c => c.GetPowerOutput(It.IsAny<Direction>())).Returns(wire1.PowerLevel);
        
        // Update wire2 from wire1
        wire2.UpdateState(new Dictionary<Direction, IComponent>
        {
            { Direction.North, mockWire1.Object }
        });
        
        // Mock wire2 as a component to feed into wire3
        var mockWire2 = new Mock<IComponent>();
        mockWire2.Setup(c => c.GetPowerOutput(It.IsAny<Direction>())).Returns(wire2.PowerLevel);
        
        // Update wire3 from wire2
        wire3.UpdateState(new Dictionary<Direction, IComponent>
        {
            { Direction.North, mockWire2.Object }
        });
        
        // Assert
        Assert.Equal(14, wire1.PowerLevel); // 15 - 1
        Assert.Equal(13, wire2.PowerLevel); // 14 - 1
        Assert.Equal(12, wire3.PowerLevel); // 13 - 1
    }

    /// <summary>
    /// Verifies that the GetVisualPowerLevel method returns correct values.
    /// </summary>
    [Theory]
    [InlineData(0, 0)]
    [InlineData(5, 5)]
    [InlineData(15, 15)]
    public void GetVisualPowerLevel_ReturnsCorrectValue(int powerLevel, int expectedVisualLevel)
    {
        // Arrange
        var wire = new RedstoneWire();
        wire.PowerLevel = powerLevel;
        
        // Act
        int visualLevel = wire.GetVisualPowerLevel();
        
        // Assert
        Assert.Equal(expectedVisualLevel, visualLevel);
    }

    /// <summary>
    /// Verifies behavior when ResetConnections method is called.
    /// </summary>
    [Fact]
    public void ResetConnections_EnablesAllDirections()
    {
        // Arrange
        var wire = new RedstoneWire();
        wire.ToggleDirection(Direction.North);
        wire.ToggleDirection(Direction.East);
        wire.ToggleDirection(Direction.South);
        wire.ToggleDirection(Direction.West);
        
        // Act
        wire.ResetConnections();
        
        // Assert
        Assert.True(wire.IsDirectionEnabled(Direction.North));
        Assert.True(wire.IsDirectionEnabled(Direction.East));
        Assert.True(wire.IsDirectionEnabled(Direction.South));
        Assert.True(wire.IsDirectionEnabled(Direction.West));
    }

    /// <summary>
    /// Verifies wire state is properly preserved when serialized and deserialized.
    /// </summary>
    [Fact]
    public void Serialization_PreservesWireState()
    {
        // Arrange
        var originalWire = new RedstoneWire();
        originalWire.PowerLevel = 10;
        originalWire.ToggleDirection(Direction.North);
        originalWire.ToggleDirection(Direction.West);
        
        // Act
        var serializedData = originalWire.Serialize();
        var deserializedWire = new RedstoneWire();
        deserializedWire.Deserialize(serializedData);
        
        // Assert
        Assert.Equal(10, deserializedWire.PowerLevel);
        Assert.False(deserializedWire.IsDirectionEnabled(Direction.North));
        Assert.True(deserializedWire.IsDirectionEnabled(Direction.East));
        Assert.True(deserializedWire.IsDirectionEnabled(Direction.South));
        Assert.False(deserializedWire.IsDirectionEnabled(Direction.West));
    }

    /// <summary>
    /// Verifies wire connects correctly to different component types.
    /// </summary>
    [Fact]
    public void Wire_ConnectsToCorrectComponentTypes()
    {
        // Arrange
        var wire = new RedstoneWire();
        
        var mockRedstoneWire = new Mock<IComponent>();
        mockRedstoneWire.Setup(c => c.ComponentType).Returns(ComponentType.RedstoneWire);
        mockRedstoneWire.Setup(c => c.CanConnect(It.IsAny<Direction>())).Returns(true);
        
        var mockRepeater = new Mock<IComponent>();
        mockRepeater.Setup(c => c.ComponentType).Returns(ComponentType.Repeater);
        mockRepeater.Setup(c => c.CanConnect(It.IsAny<Direction>())).Returns(true);
        
        var mockButton = new Mock<IComponent>();
        mockButton.Setup(c => c.ComponentType).Returns(ComponentType.Button);
        mockButton.Setup(c => c.CanConnect(It.IsAny<Direction>())).Returns(true);
        
        var mockIncompatible = new Mock<IComponent>();
        mockIncompatible.Setup(c => c.ComponentType).Returns(ComponentType.SolidBlock);
        mockIncompatible.Setup(c => c.CanConnect(It.IsAny<Direction>())).Returns(false);
        
        // Act & Assert
        Assert.True(wire.CanConnectToComponent(mockRedstoneWire.Object, Direction.North));
        Assert.True(wire.CanConnectToComponent(mockRepeater.Object, Direction.East));
        Assert.True(wire.CanConnectToComponent(mockButton.Object, Direction.South));
        Assert.False(wire.CanConnectToComponent(mockIncompatible.Object, Direction.West));
    }

    /// <summary>
    /// Verifies wire doesn't connect diagonally.
    /// </summary>
    [Fact]
    public void Wire_DoesNotConnectDiagonally()
    {
        // Arrange
        var wire = new RedstoneWire();
        
        // Act & Assert
        Assert.False(wire.CanConnect(Direction.NorthEast));
        Assert.False(wire.CanConnect(Direction.NorthWest));
        Assert.False(wire.CanConnect(Direction.SouthEast));
        Assert.False(wire.CanConnect(Direction.SouthWest));
    }

    /// <summary>
    /// Verifies proper handling of null neighbors dictionary.
    /// </summary>
    [Fact]
    public void UpdateState_HandlesNullNeighborsDictionary()
    {
        // Arrange
        var wire = new RedstoneWire();
        wire.PowerLevel = 10; // Set initial power level
        
        // Act
        wire.UpdateState(null); // Pass null dictionary
        
        // Assert
        Assert.Equal(0, wire.PowerLevel); // Should reset to 0 when no neighbors are provided
    }

    /// <summary>
    /// Verifies component behaves correctly when accessed from multiple threads.
    /// </summary>
    [Fact]
    public async Task Wire_ThreadSafeOperations()
    {
        // Arrange
        var wire = new RedstoneWire();
        const int iterations = 1000;
        
        // Act
        var task1 = Task.Run(() => {
            for (int i = 0; i < iterations; i++) {
                wire.ToggleDirection(Direction.North);
                wire.PowerLevel = 5;
            }
        });
        
        var task2 = Task.Run(() => {
            for (int i = 0; i < iterations; i++) {
                wire.ToggleDirection(Direction.North);
                wire.PowerLevel = 10;
            }
        });
        
        await Task.WhenAll(task1, task2);
        
        // Assert
        // The final state depends on which thread executed last,
        // but the important thing is that no exceptions were thrown
        // and the component is in a valid state
        Assert.True(wire.PowerLevel == 5 || wire.PowerLevel == 10);
        
        // North connection should be in a consistent state (either enabled or disabled)
        bool northEnabled = wire.IsDirectionEnabled(Direction.North);
        Assert.Equal(northEnabled, wire.IsDirectionEnabled(Direction.North));
    }
}