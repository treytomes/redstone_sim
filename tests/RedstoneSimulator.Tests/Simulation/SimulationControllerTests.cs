// tests\RedstoneSimulator.Core.Tests\Simulation\SimulationControllerTests.cs

using OpenTK.Mathematics;
using RedstoneSimulator.Core.Components;
using RedstoneSimulator.Core.Grid;
using RedstoneSimulator.Core.Simulation;
using Xunit;
using Moq;
using System.Collections.Generic;

namespace RedstoneSimulator.Core.Tests.Simulation;

public class SimulationControllerTests
{
    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Arrange
        var mockGrid = new Mock<IGrid>();
        
        // Act
        var controller = new SimulationController(mockGrid.Object);
        
        // Assert
        Assert.False(controller.IsRunning);
        Assert.Equal(10.0f, controller.TickRate);
    }
    
    [Fact]
    public void Start_SetsIsRunningToTrue()
    {
        // Arrange
        var mockGrid = new Mock<IGrid>();
        var controller = new SimulationController(mockGrid.Object);
        
        // Act
        controller.Start();
        
        // Assert
        Assert.True(controller.IsRunning);
    }
    
    [Fact]
    public void Pause_SetsIsRunningToFalse()
    {
        // Arrange
        var mockGrid = new Mock<IGrid>();
        var controller = new SimulationController(mockGrid.Object);
        controller.Start();
        
        // Act
        controller.Pause();
        
        // Assert
        Assert.False(controller.IsRunning);
    }
    
    [Fact]
    public void Update_WhenPaused_DoesNotProcessTicks()
    {
        // Arrange
        var mockGrid = new Mock<IGrid>();
        var mockComponent = new Mock<IComponent>();
        
        var cell = new Cell(new Vector2i(0, 0));
        cell.SetComponent(mockComponent.Object);
        
        var cells = new List<Cell> { cell };
        mockGrid.Setup(g => g.GetAllCells()).Returns(cells);
        mockGrid.Setup(g => g.GetMarkedCells()).Returns(new List<Vector2i>());
        
        var controller = new SimulationController(mockGrid.Object);
        controller.Pause(); // Ensure paused
        
        // Act
        controller.Update(1.0f); // Update with enough time for multiple ticks
        
        // Assert
        mockComponent.Verify(c => c.Update(It.IsAny<float>()), Times.Never);
    }
    
    [Fact]
    public void Update_WhenRunning_ProcessesTicksBasedOnTime()
    {
        // Arrange
        var mockGrid = new Mock<IGrid>();
        var mockComponent = new Mock<IComponent>();
        
        var cell = new Cell(new Vector2i(0, 0));
        cell.SetComponent(mockComponent.Object);
        
        var cells = new List<Cell> { cell };
        mockGrid.Setup(g => g.GetAllCells()).Returns(cells);
        mockGrid.Setup(g => g.GetMarkedCells()).Returns(new List<Vector2i>());
        
        var controller = new SimulationController(mockGrid.Object);
        controller.TickRate = 10.0f; // 10 ticks per second
        controller.Start();
        
        // Act
        controller.Update(0.5f); // Update with enough time for 5 ticks
        
        // Assert
        mockComponent.Verify(c => c.Update(0.1f), Times.Exactly(5));
    }
    
    [Fact]
    public void Step_ProcessesOneTick()
    {
        // Arrange
        var mockGrid = new Mock<IGrid>();
        var mockComponent = new Mock<IComponent>();
        
        var cell = new Cell(new Vector2i(0, 0));
        cell.SetComponent(mockComponent.Object);
        
        var cells = new List<Cell> { cell };
        mockGrid.Setup(g => g.GetAllCells()).Returns(cells);
        mockGrid.Setup(g => g.GetMarkedCells()).Returns(new List<Vector2i>());
        
        var controller = new SimulationController(mockGrid.Object);
        controller.TickRate = 10.0f; // 10 ticks per second
        
        // Act
        controller.Step();
        
        // Assert
        mockComponent.Verify(c => c.Update(0.1f), Times.Once);
    }
    
    [Fact]
    public void TickCompleted_EventRaisedAfterTick()
    {
        // Arrange
        var mockGrid = new Mock<IGrid>();
        mockGrid.Setup(g => g.GetAllCells()).Returns(new List<Cell>());
        mockGrid.Setup(g => g.GetMarkedCells()).Returns(new List<Vector2i>());
        
        var controller = new SimulationController(mockGrid.Object);
        
        bool eventRaised = false;
        controller.TickCompleted += (sender, args) => eventRaised = true;
        
        // Act
        controller.Step();
        
        // Assert
        Assert.True(eventRaised);
    }
    
    [Fact]
    public void ProcessMarkedCells_PropagatesSignalsFromMarkedCells()
    {
        // Arrange
        var mockGrid = new Mock<IGrid>();
        
        // Create a button at position (0,0)
        var buttonCell = new Cell(new Vector2i(0, 0));
        var button = new RedstoneButton();
        buttonCell.SetComponent(button);
        
        // Create a wire at position (1,0)
        var wireCell = new Cell(new Vector2i(1, 0));
        var wire = new RedstoneWire();
        wireCell.SetComponent(wire);
        
        // Set up grid to return our cells
        mockGrid.Setup(g => g.GetCell(new Vector2i(0, 0))).Returns(buttonCell);
        mockGrid.Setup(g => g.GetCell(new Vector2i(1, 0))).Returns(wireCell);
        mockGrid.Setup(g => g.GetAllCells()).Returns(new List<Cell> { buttonCell, wireCell });
        
        // Mark the button cell for update
        var markedCells = new List<Vector2i> { new Vector2i(0, 0) };
        mockGrid.Setup(g => g.GetMarkedCells()).Returns(markedCells);
        
        var controller = new SimulationController(mockGrid.Object);
        
        // Press the button to set its power level
        button.Press();
        
        // Act
        controller.Step();
        
        // Assert
        Assert.True(wire.PowerLevel > 0); // Wire should receive power from the button
        mockGrid.Verify(g => g.ClearMarkedCells(), Times.Once);
    }
    
    [Fact]
    public void TickRate_WhenSetBelowMinimum_UsesMininumValue()
    {
        // Arrange
        var mockGrid = new Mock<IGrid>();
        var controller = new SimulationController(mockGrid.Object);
        
        // Act
        controller.TickRate = 0.05f; // Try to set below minimum
        
        // Assert
        Assert.Equal(0.1f, controller.TickRate); // Should be clamped to minimum
    }
    
    [Fact]
    public void Update_WithMultipleComponents_UpdatesAllComponents()
    {
        // Arrange
        var mockGrid = new Mock<IGrid>();
        
        var component1 = new Mock<IComponent>();
        var component2 = new Mock<IComponent>();
        var component3 = new Mock<IComponent>();
        
        var cell1 = new Cell(new Vector2i(0, 0));
        cell1.SetComponent(component1.Object);
        
        var cell2 = new Cell(new Vector2i(1, 0));
        cell2.SetComponent(component2.Object);
        
        var cell3 = new Cell(new Vector2i(0, 1));
        cell3.SetComponent(component3.Object);
        
        var cells = new List<Cell> { cell1, cell2, cell3 };
        mockGrid.Setup(g => g.GetAllCells()).Returns(cells);
        mockGrid.Setup(g => g.GetMarkedCells()).Returns(new List<Vector2i>());
        
        var controller = new SimulationController(mockGrid.Object);
        controller.TickRate = 5.0f; // 5 ticks per second
        
        // Act
        controller.Step();
        
        // Assert
        component1.Verify(c => c.Update(0.2f), Times.Once);
        component2.Verify(c => c.Update(0.2f), Times.Once);
        component3.Verify(c => c.Update(0.2f), Times.Once);
    }
}