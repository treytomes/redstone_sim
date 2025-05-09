// tests\RedstoneSimulator.Core.Tests\Components\RedstoneButtonTests.cs

using RedstoneSimulator.Core.Common;
using RedstoneSimulator.Core.Components;
using Xunit;

namespace RedstoneSimulator.Core.Tests.Components;

public class RedstoneButtonTests
{
    [Fact]
    public void Button_InitialState_NotPressed()
    {
        // Arrange & Act
        var button = new RedstoneButton();
        
        // Assert
        Assert.False(button.IsPressed);
        Assert.Equal(0, button.PowerLevel);
    }
    
    [Fact]
    public void Button_WhenPressed_OutputsPowerLevel15()
    {
        // Arrange
        var button = new RedstoneButton();
        
        // Act
        button.Press();
        
        // Assert
        Assert.True(button.IsPressed);
        Assert.Equal(15, button.PowerLevel);
    }
    
    [Fact]
    public void Button_AfterDuration_AutomaticallyDeactivates()
    {
        // Arrange
        float duration = 0.5f;
        var button = new RedstoneButton(duration);
        button.Press();
        
        // Act - Simulate time passing
        button.Update(duration + 0.1f);
        
        // Assert
        Assert.False(button.IsPressed);
        Assert.Equal(0, button.PowerLevel);
    }
    
    [Fact]
    public void Button_OutputsPowerOnlyInFacingDirection()
    {
        // Arrange
        var button = new RedstoneButton();
        button.SetDirection(Direction.East);
        button.Press();
        
        // Act & Assert
        Assert.Equal(15, button.GetPowerOutput(Direction.East));
        Assert.Equal(0, button.GetPowerOutput(Direction.North));
        Assert.Equal(0, button.GetPowerOutput(Direction.South));
        Assert.Equal(0, button.GetPowerOutput(Direction.West));
    }
    
    [Fact]
    public void Button_CanConnectOnlyInFacingDirection()
    {
        // Arrange
        var button = new RedstoneButton();
        button.SetDirection(Direction.South);
        
        // Act & Assert
        Assert.True(button.CanConnect(Direction.South));
        Assert.False(button.CanConnect(Direction.North));
        Assert.False(button.CanConnect(Direction.East));
        Assert.False(button.CanConnect(Direction.West));
    }
    
    [Fact]
    public void Button_Clone_CreatesDeepCopy()
    {
        // Arrange
        var button = new RedstoneButton(2.0f);
        button.SetDirection(Direction.West);
        button.Press();
        
        // Act
        var clone = (RedstoneButton)button.Clone();
        
        // Assert
        Assert.NotSame(button, clone);
        Assert.Equal(button.PowerLevel, clone.PowerLevel);
        Assert.Equal(button.Orientation, clone.Orientation);
        Assert.Equal(button.IsPressed, clone.IsPressed);
    }
}