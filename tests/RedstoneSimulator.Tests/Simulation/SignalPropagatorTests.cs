// tests\RedstoneSimulator.Core.Tests\Simulation\SignalPropagatorTests.cs

using OpenTK.Mathematics;
using RedstoneSimulator.Core.Common;
using RedstoneSimulator.Core.Components;
using RedstoneSimulator.Core.Grid;
using RedstoneSimulator.Core.Simulation;
using Xunit;
using Moq;

namespace RedstoneSimulator.Core.Tests.Simulation;

public class SignalPropagatorTests
{
    [Fact]
    public void PropagateSignals_FromButtonToWire_UpdatesWirePowerLevel()
    {
        // Arrange
        var mockGrid = new Mock<IGrid>();
        
        // Create a button at position (0,0) facing East
        var buttonCell = new Cell(new Vector2i(0, 0));
        var button = new RedstoneButton();
        button.SetDirection(Direction.East);
        button.Press(); // Power level 15
        buttonCell.SetComponent(button);
        
        // Create a wire at position (1,0)
        var wireCell = new Cell(new Vector2i(1, 0));
        var wire = new RedstoneWire();
        wireCell.SetComponent(wire);
        
        // Configure the mock grid to return our cells
        mockGrid.Setup(g => g.GetCell(new Vector2i(0, 0))).Returns(buttonCell);
        mockGrid.Setup(g => g.GetCell(new Vector2i(1, 0))).Returns(wireCell);
        
        var propagator = new SignalPropagator(mockGrid.Object);
        
        // Act
        propagator.PropagateSignals(new Vector2i(0, 0));
        
        // Assert
        Assert.Equal(14, wire.PowerLevel); // Power level decreases by 1 through wire
    }
    
    [Fact]
    public void PropagateSignals_FromSwitchToMultipleWires_UpdatesAllWires()
    {
        // Arrange
        var mockGrid = new Mock<IGrid>();
        
        // Create a switch at position (5,5) that can connect in all directions
        var switchCell = new Cell(new Vector2i(5, 5));
        var switch1 = new CustomSwitch(true); // On state, power level 15
        switchCell.SetComponent(switch1);
        
        // Create wires in all four directions around the switch
        var wireNorthCell = new Cell(new Vector2i(5, 4));
        var wireNorth = new RedstoneWire();
        wireNorthCell.SetComponent(wireNorth);
        
        var wireEastCell = new Cell(new Vector2i(6, 5));
        var wireEast = new RedstoneWire();
        wireEastCell.SetComponent(wireEast);
        
        var wireSouthCell = new Cell(new Vector2i(5, 6));
        var wireSouth = new RedstoneWire();
        wireSouthCell.SetComponent(wireSouth);
        
        var wireWestCell = new Cell(new Vector2i(4, 5));
        var wireWest = new RedstoneWire();
        wireWestCell.SetComponent(wireWest);
        
        // Configure the mock grid to return our cells
        mockGrid.Setup(g => g.GetCell(new Vector2i(5, 5))).Returns(switchCell);
        mockGrid.Setup(g => g.GetCell(new Vector2i(5, 4))).Returns(wireNorthCell);
        mockGrid.Setup(g => g.GetCell(new Vector2i(6, 5))).Returns(wireEastCell);
        mockGrid.Setup(g => g.GetCell(new Vector2i(5, 6))).Returns(wireSouthCell);
        mockGrid.Setup(g => g.GetCell(new Vector2i(4, 5))).Returns(wireWestCell);
        
        var propagator = new SignalPropagator(mockGrid.Object);
        
        // Act
        propagator.PropagateSignals(new Vector2i(5, 5));
        
        // Assert
        Assert.Equal(14, wireNorth.PowerLevel);
        Assert.Equal(14, wireEast.PowerLevel);
        Assert.Equal(14, wireSouth.PowerLevel);
        Assert.Equal(14, wireWest.PowerLevel);
    }
    
    [Fact]
    public void PropagateSignals_ThroughMultipleWires_DecaysPowerCorrectly()
    {
        // Arrange
        var mockGrid = new Mock<IGrid>();
        
        // Create a button at position (0,0) facing East
        var buttonCell = new Cell(new Vector2i(0, 0));
        var button = new RedstoneButton();
        button.SetDirection(Direction.East);
        button.Press(); // Power level 15
        buttonCell.SetComponent(button);
        
        // Create a chain of wires
        var wire1Cell = new Cell(new Vector2i(1, 0));
        var wire1 = new RedstoneWire();
        wire1Cell.SetComponent(wire1);
        
        var wire2Cell = new Cell(new Vector2i(2, 0));
        var wire2 = new RedstoneWire();
        wire2Cell.SetComponent(wire2);
        
        var wire3Cell = new Cell(new Vector2i(3, 0));
        var wire3 = new RedstoneWire();
        wire3Cell.SetComponent(wire3);
        
        // Configure the mock grid to return our cells
        mockGrid.Setup(g => g.GetCell(new Vector2i(0, 0))).Returns(buttonCell);
        mockGrid.Setup(g => g.GetCell(new Vector2i(1, 0))).Returns(wire1Cell);
        mockGrid.Setup(g => g.GetCell(new Vector2i(2, 0))).Returns(wire2Cell);
        mockGrid.Setup(g => g.GetCell(new Vector2i(3, 0))).Returns(wire3Cell);
        
        var propagator = new SignalPropagator(mockGrid.Object);
        
        // Act
        propagator.PropagateSignals(new Vector2i(0, 0));
        
        // Assert
        Assert.Equal(14, wire1.PowerLevel); // 15 - 1 = 14
        Assert.Equal(13, wire2.PowerLevel); // 14 - 1 = 13
        Assert.Equal(12, wire3.PowerLevel); // 13 - 1 = 12
    }
    
    [Fact]
    public void PropagateSignals_WithNoComponent_DoesNothing()
    {
        // Arrange
        var mockGrid = new Mock<IGrid>();
        
        // Empty cell at position (0,0)
        var emptyCell = new Cell(new Vector2i(0, 0));
        
        mockGrid.Setup(g => g.GetCell(new Vector2i(0, 0))).Returns(emptyCell);
        
        var propagator = new SignalPropagator(mockGrid.Object);
        
        // Act - this should not throw an exception
        propagator.PropagateSignals(new Vector2i(0, 0));
        
        // Assert - nothing to assert, just verifying no exception is thrown
    }
    
    [Fact]
    public void PropagateSignals_WithDisconnectedComponents_DoesNotPropagate()
    {
        // Arrange
        var mockGrid = new Mock<IGrid>();
        
        // Create a button at position (0,0) facing East
        var buttonCell = new Cell(new Vector2i(0, 0));
        var button = new RedstoneButton();
        button.SetDirection(Direction.East);
        button.Press(); // Power level 15
        buttonCell.SetComponent(button);
        
        // Create a wire at position (1,0) that can't connect to the West
        var wireCell = new Cell(new Vector2i(1, 0));
        var wire = new DisconnectedWire(); // Custom wire that doesn't connect to the West
        wireCell.SetComponent(wire);
        
        // Configure the mock grid to return our cells
        mockGrid.Setup(g => g.GetCell(new Vector2i(0, 0))).Returns(buttonCell);
        mockGrid.Setup(g => g.GetCell(new Vector2i(1, 0))).Returns(wireCell);
        
        var propagator = new SignalPropagator(mockGrid.Object);
        
        // Act
        propagator.PropagateSignals(new Vector2i(0, 0));
        
        // Assert
        Assert.Equal(0, wire.PowerLevel); // Power should not propagate
    }
    
    // Custom switch that can connect in all directions
    private class CustomSwitch : RedstoneSwitch
    {
        public CustomSwitch(bool initialState) : base(initialState) { }
        
        public override IReadOnlyList<Direction> ConnectedSides => DirectionExtensions.CardinalDirections;
        
        public override bool CanConnect(Direction side) => true;
        
        public override int GetPowerOutput(Direction side) => PowerLevel;
    }
    
    // Custom wire that doesn't connect to the West
    private class DisconnectedWire : RedstoneWire
    {
        public override bool CanConnect(Direction side) => side != Direction.West;
    }
}