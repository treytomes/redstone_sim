// tests\RedstoneSimulator.Core.Tests\Components\ComponentFactoryTests.cs

using RedstoneSimulator.Core.Common;
using RedstoneSimulator.Core.Components;
using Xunit;

namespace RedstoneSimulator.Core.Tests.Components;

public class ComponentFactoryTests
{
    [Fact]
    public void CreateFromData_Button_CreatesCorrectType()
    {
        // Arrange
        var data = new ComponentData
        {
            Type = "Button",
            Properties = new Dictionary<string, object>
            {
                ["PowerLevel"] = 0,
                ["Orientation"] = (int)Direction.East,
                ["ActivationDuration"] = 2.5f
            }
        };
        
        // Act
        var component = ComponentFactory.Instance.CreateFromData(data);
        
        // Assert
        Assert.IsType<RedstoneButton>(component);
        var button = (RedstoneButton)component;
        Assert.Equal(Direction.East, button.Orientation);
        Assert.Equal(0, button.PowerLevel);
        Assert.False(button.IsPressed);
    }
    
    [Fact]
    public void CreateFromData_Button_WithPressedState_CreatesCorrectState()
    {
        // Arrange
        var data = new ComponentData
        {
            Type = "Button",
            Properties = new Dictionary<string, object>
            {
                ["PowerLevel"] = 15,
                ["Orientation"] = (int)Direction.North,
                ["ActivationDuration"] = 1.0f,
                ["IsPressed"] = true
            }
        };
        
        // Act
        // Act
        var component = ComponentFactory.Instance.CreateFromData(data);
        
        // Assert
        Assert.IsType<RedstoneButton>(component);
        var button = (RedstoneButton)component;
        Assert.Equal(Direction.North, button.Orientation);
        Assert.Equal(15, button.PowerLevel);
        Assert.True(button.IsPressed);
    }
    
    [Fact]
    public void CreateFromData_Switch_CreatesCorrectType()
    {
        // Arrange
        var data = new ComponentData
        {
            Type = "Switch",
            Properties = new Dictionary<string, object>
            {
                ["PowerLevel"] = 0,
                ["Orientation"] = (int)Direction.West,
                ["IsOn"] = false
            }
        };
        
        // Act
        var component = ComponentFactory.Instance.CreateFromData(data);
        
        // Assert
        Assert.IsType<RedstoneSwitch>(component);
        var switch1 = (RedstoneSwitch)component;
        Assert.Equal(Direction.West, switch1.Orientation);
        Assert.Equal(0, switch1.PowerLevel);
        Assert.False(switch1.IsOn);
    }
    
    [Fact]
    public void CreateFromData_Switch_WithOnState_CreatesCorrectState()
    {
        // Arrange
        var data = new ComponentData
        {
            Type = "Switch",
            Properties = new Dictionary<string, object>
            {
                ["PowerLevel"] = 15,
                ["Orientation"] = (int)Direction.South,
                ["IsOn"] = true
            }
        };
        
        // Act
        var component = ComponentFactory.Instance.CreateFromData(data);
        
        // Assert
        Assert.IsType<RedstoneSwitch>(component);
        var switch1 = (RedstoneSwitch)component;
        Assert.Equal(Direction.South, switch1.Orientation);
        Assert.Equal(15, switch1.PowerLevel);
        Assert.True(switch1.IsOn);
    }
    
    [Fact]
    public void CreateFromData_WithStringOrientation_SetsCorrectOrientation()
    {
        // Arrange
        var data = new ComponentData
        {
            Type = "Button",
            Properties = new Dictionary<string, object>
            {
                ["Orientation"] = "East"
            }
        };
        
        // Act
        var component = ComponentFactory.Instance.CreateFromData(data);
        
        // Assert
        Assert.Equal(Direction.East, component.Orientation);
    }
    
    [Fact]
    public void RegisterComponentFactory_OverridesExistingFactory()
    {
        // Arrange
        var customButton = new RedstoneButton(5.0f);
        customButton.SetDirection(Direction.West);
        
        ComponentFactory.Instance.RegisterComponentFactory("CustomButton", _ => customButton);
        
        var data = new ComponentData
        {
            Type = "CustomButton",
            Properties = new Dictionary<string, object>()
        };
        
        // Act
        var component = ComponentFactory.Instance.CreateFromData(data);
        
        // Assert
        Assert.Same(customButton, component);
    }
    
    [Fact]
    public void CreateFromData_ThrowsArgumentNullException_WhenDataIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ComponentFactory.Instance.CreateFromData(null!));
    }
    
    [Fact]
    public void CreateFromData_ThrowsArgumentException_WhenTypeIsEmpty()
    {
        // Arrange
        var data = new ComponentData
        {
            Type = "",
            Properties = new Dictionary<string, object>()
        };
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => ComponentFactory.Instance.CreateFromData(data));
    }
    
    [Fact]
    public void CreateFromData_ThrowsArgumentException_WhenTypeIsUnknown()
    {
        // Arrange
        var data = new ComponentData
        {
            Type = "NonExistentComponent",
            Properties = new Dictionary<string, object>()
        };
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => ComponentFactory.Instance.CreateFromData(data));
    }
}