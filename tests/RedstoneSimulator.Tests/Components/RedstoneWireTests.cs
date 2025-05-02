using System.Collections.Generic;
using Xunit;
using Moq;
using RedstoneSimulator.Core.Components;
using RedstoneSimulator.Core.Common;

namespace RedstoneSimulator.Tests.Components;

public class RedstoneWireTests
{
	[Fact]
	public void NewWire_HasZeroPowerLevel()
	{
		// Arrange & Act
		var wire = new RedstoneWire();
		
		// Assert
		Assert.Equal(0, wire.PowerLevel);
	}

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
}
