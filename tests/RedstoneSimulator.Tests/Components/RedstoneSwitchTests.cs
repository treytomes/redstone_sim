// tests\RedstoneSimulator.Core.Tests\Components\RedstoneSwitchTests.cs

using RedstoneSimulator.Core.Common;
using RedstoneSimulator.Core.Components;
using Xunit;

namespace RedstoneSimulator.Core.Tests.Components;

public class RedstoneSwitchTests
{
    [Fact]
    public void Switch_InitialState_DefaultOff()
    {
        // Arrange & Act
        var lever = new RedstoneSwitch();
        
        // Assert
        Assert.False(lever.IsOn);
        Assert.Equal(0, lever.PowerLevel);
    }
    
    [Fact]
    public void Switch_InitialState_CanBeOn()
    {
        // Arrange & Act
        var lever = new RedstoneSwitch(true);
        
        // Assert
        Assert.True(lever.IsOn);
        Assert.Equal(15, lever.PowerLevel);
    }
    
    [Fact]
    public void Switch_Toggle_ChangesState()
    {
        // Arrange
        var lever = new RedstoneSwitch();
        
        // Act
        lever.Toggle();
        
        // Assert
        Assert.True(lever.IsOn);
        Assert.Equal(15, lever.PowerLevel);
        
        // Act again
        lever.Toggle();
        
        // Assert again
        Assert.False(lever.IsOn);
        Assert.Equal(0, lever.PowerLevel);
    }
    
    [Fact]
    public void Switch_SetState_ChangesStateDirectly()
    {
        // Arrange
        var lever = new RedstoneSwitch();
        
        // Act
        lever.SetState(true);
        
        // Assert
        Assert.True(lever.IsOn);
        Assert.Equal(15, lever.PowerLevel);
        
        // Act again
        lever.SetState(false);
        
        // Assert again
        Assert.False(lever.IsOn);
        Assert.Equal(0, lever.PowerLevel);
    }
    
    [Fact]
    public void Switch_OutputsPowerOnlyInFacingDirection()
    {
        // Arrange
        var lever = new RedstoneSwitch(true);
        lever.SetDirection(Direction.North);
        
        // Act & Assert
        Assert.Equal(15, lever.GetPowerOutput(Direction.North));
        Assert.Equal(0, lever.GetPowerOutput(Direction.East));
        Assert.Equal(0, lever.GetPowerOutput(Direction.South));
        Assert.Equal(0, lever.GetPowerOutput(Direction.West));
    }
    
    [Fact]
    public void Switch_CanConnectOnlyInFacingDirection()
    {
        // Arrange
        var lever = new RedstoneSwitch();
        lever.SetDirection(Direction.East);
        
        // Act & Assert
        Assert.True(lever.CanConnect(Direction.East));
        Assert.False(lever.CanConnect(Direction.North));
        Assert.False(lever.CanConnect(Direction.South));
        Assert.False(lever.CanConnect(Direction.West));
    }
    
    [Fact]
    public void Switch_Clone_CreatesDeepCopy()
    {
        // Arrange
        var lever = new RedstoneSwitch(true);
        lever.SetDirection(Direction.South);
        
        // Act
        var clone = (RedstoneSwitch)lever.Clone();
        
        // Assert
        Assert.NotSame(lever, clone);
        Assert.Equal(lever.PowerLevel, clone.PowerLevel);
        Assert.Equal(lever.Orientation, clone.Orientation);
        Assert.Equal(lever.IsOn, clone.IsOn);
    }
    
    [Fact]
    public void Switch_MaintainsStateAcrossUpdates()
    {
        // Arrange
        var lever = new RedstoneSwitch(true);
        
        // Act - Simulate multiple update cycles
        lever.Update(0.1f);
        lever.Update(0.1f);
        lever.Update(0.1f);
        
        // Assert - State should remain unchanged
        Assert.True(lever.IsOn);
        Assert.Equal(15, lever.PowerLevel);
    }
    
    [Fact]
    public void Switch_SerializationData_ContainsAllRequiredProperties()
    {
        // Arrange
        var lever = new RedstoneSwitch(true);
        lever.SetDirection(Direction.West);
        
        // Act
        var data = lever.GetSerializationData();
        
        // Assert
        Assert.Equal("Switch", data.Type);
        Assert.Equal(15, data.Properties["PowerLevel"]);
        Assert.Equal((int)Direction.West, data.Properties["Orientation"]);
        Assert.True((bool)data.Properties["IsOn"]);
    }
}