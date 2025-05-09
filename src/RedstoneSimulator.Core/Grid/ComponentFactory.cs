using RedstoneSimulator.Core.Common;

namespace RedstoneSimulator.Core.Components;

/// <summary>
/// Factory for creating components from serialized data.
/// </summary>
public class ComponentFactory
{
    private static readonly Lazy<ComponentFactory> _instance = new Lazy<ComponentFactory>(
        () => new ComponentFactory(), LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Private constructor to enforce singleton pattern
    /// </summary>
    private ComponentFactory() 
    {
        // Register default component factories
        RegisterComponentFactory("RedstoneWire", CreateWireComponent);
        RegisterComponentFactory("Button", CreateButtonComponent);
        RegisterComponentFactory("Switch", CreateSwitchComponent);
        RegisterComponentFactory("TestComponent", CreateTestComponent);
    }

    /// <summary>
    /// Gets the singleton instance of the ComponentFactory.
    /// </summary>
    public static ComponentFactory Instance => _instance.Value;
    
    /// <summary>
    /// Dictionary mapping component type names to factory methods
    /// </summary>
    private readonly Dictionary<string, Func<ComponentData, IComponent>> _componentFactories = new();
    
    /// <summary>
    /// Creates a component from serialized data.
    /// </summary>
    /// <param name="data">The serialized component data.</param>
    /// <returns>A new component instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the component type is unknown.</exception>
    public IComponent CreateFromData(ComponentData data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }
        
        if (string.IsNullOrEmpty(data.Type))
        {
            throw new ArgumentException("Component type cannot be null or empty", nameof(data));
        }
        
        // Check if we have a factory for this component type
        if (_componentFactories.TryGetValue(data.Type, out var factory))
        {
            return factory(data);
        }
        
        // If we don't have a specific factory, try to create a generic component
        // based on enum parsing
        if (Enum.TryParse<ComponentType>(data.Type, out var componentType))
        {
            return CreateGenericComponent(componentType, data);
        }
        
        throw new ArgumentException($"Unknown component type: {data.Type}", nameof(data));
    }
    
    /// <summary>
    /// Registers a custom component factory method.
    /// </summary>
    /// <param name="typeName">The name of the component type.</param>
    /// <param name="factory">The factory method to create components of this type.</param>
    public void RegisterComponentFactory(string typeName, Func<ComponentData, IComponent> factory)
    {
        _componentFactories[typeName] = factory ?? throw new ArgumentNullException(nameof(factory));
    }
    
    private static IComponent CreateGenericComponent(ComponentType type, ComponentData data)
    {
        // Create a component based on the type
        return type switch
        {
            ComponentType.RedstoneWire => CreateWireComponent(data),
            ComponentType.RedstoneTorch => CreateTorchComponent(data),
            ComponentType.Button => CreateButtonComponent(data),
            ComponentType.Switch => CreateSwitchComponent(data),
            ComponentType.Repeater => CreateRepeaterComponent(data),
            ComponentType.Transistor => CreateTransistorComponent(data),
            _ => throw new ArgumentException($"Unsupported component type: {type}")
        };
    }
    
    private static IComponent CreateWireComponent(ComponentData data)
    {
        var wire = new RedstoneWire();
        
        // Set power level if provided
        if (data.Properties.TryGetValue("PowerLevel", out var powerLevelObj) && powerLevelObj is int powerLevel)
        {
            wire.PowerLevel = powerLevel;
        }
        
        // Set enabled directions if provided
        if (data.Properties.TryGetValue("EnabledDirections", out var directionsObj))
        {
            if (directionsObj is Dictionary<string, bool> directionDict)
            {
                // Reset connections first to ensure clean state
                wire.ResetConnections();
                
                // Toggle off any disabled directions
                foreach (var kvp in directionDict)
                {
                    if (Enum.TryParse<Direction>(kvp.Key, out var direction) && !kvp.Value)
                    {
                        wire.ToggleDirection(direction);
                    }
                }
            }
            else if (directionsObj is Dictionary<Direction, bool> typedDirectionDict)
            {
                // Reset connections first to ensure clean state
                wire.ResetConnections();
                
                // Toggle off any disabled directions
                foreach (var kvp in typedDirectionDict)
                {
                    if (!kvp.Value)
                    {
                        wire.ToggleDirection(kvp.Key);
                    }
                }
            }
        }
        
        return wire;
    }
    
    private static IComponent CreateTorchComponent(ComponentData data)
    {
        // In a real implementation, you would create a concrete RedstoneTorch here
        // For now, create a mock component
        var component = new MockComponent(ComponentType.RedstoneTorch);
        ApplyCommonProperties(component, data);
        return component;
    }
    
    private static IComponent CreateButtonComponent(ComponentData data)
    {
        // Extract the activation duration if provided, otherwise use default
        float activationDuration = 1.0f;
        if (data.Properties.TryGetValue("ActivationDuration", out var durationObj) && durationObj is float duration)
        {
            activationDuration = duration;
        }
        
        // Create a new button with the specified duration
        var button = new RedstoneButton(activationDuration);
        
        // Apply common properties
        ApplyCommonComponentProperties(button, data);
        
        // Check if the button is pressed
        if (data.Properties.TryGetValue("IsPressed", out var isPressedObj) && 
            isPressedObj is bool isPressed && isPressed)
        {
            button.Press();
            
            // Note: We can't directly set the remaining activation time as it's private
            // In a real implementation, you might want to add a method to set this for deserialization
        }
        
        return button;
    }
    
    private static IComponent CreateSwitchComponent(ComponentData data)
    {
        // Extract the initial state if provided, otherwise use default (off)
        bool initialState = false;
        if (data.Properties.TryGetValue("IsOn", out var isOnObj) && isOnObj is bool isOn)
        {
            initialState = isOn;
        }
        
        // Create a new switch with the specified initial state
        var switch1 = new RedstoneSwitch(initialState);
        
        // Apply common properties
        ApplyCommonComponentProperties(switch1, data);
        
        return switch1;
    }
    
    private static IComponent CreateRepeaterComponent(ComponentData data)
    {
        // In a real implementation, you would create a concrete RedstoneRepeater here
        // For now, create a mock component
        var component = new MockComponent(ComponentType.Repeater);
        ApplyCommonProperties(component, data);
        
        // Set repeater-specific properties
        if (data.Properties.TryGetValue("Delay", out var delayObj) && delayObj is int delay)
        {
            // In a real implementation, you would set the delay property
        }
        
        return component;
    }
    
    private static IComponent CreateTransistorComponent(ComponentData data)
    {
        // In a real implementation, you would create a concrete RedstoneTransistor here
        // For now, create a mock component
        var component = new MockComponent(ComponentType.Transistor);
        ApplyCommonProperties(component, data);
        return component;
    }
    
    private static IComponent CreateTestComponent(ComponentData data)
    {
        // Create a test component for unit testing
        return new TestComponent();
    }
    
    private static void ApplyCommonProperties(MockComponent component, ComponentData data)
    {
        // Set power level if provided
        if (data.Properties.TryGetValue("PowerLevel", out var powerLevelObj) && powerLevelObj is int powerLevel)
        {
            component.SetPowerLevel(powerLevel);
        }
        
        // Set orientation if provided
        if (data.Properties.TryGetValue("Orientation", out var orientationObj))
        {
            if (orientationObj is int orientationInt)
            {
                component.SetOrientation((Direction)orientationInt);
            }
            else if (orientationObj is string orientationStr && Enum.TryParse<Direction>(orientationStr, out var direction))
            {
                component.SetOrientation(direction);
            }
        }
    }
    
    /// <summary>
    /// Applies common properties to any component that inherits from ComponentBase
    /// </summary>
    private static void ApplyCommonComponentProperties(ComponentBase component, ComponentData data)
    {
        // Set power level if provided
        if (data.Properties.TryGetValue("PowerLevel", out var powerLevelObj) && powerLevelObj is int powerLevel)
        {
            if (component is IPowerableComponent powerableComponent)
            {
                powerableComponent.PowerLevel = powerLevel;
            }
        }
        
        // Set orientation if provided
        if (data.Properties.TryGetValue("Orientation", out var orientationObj))
        {
            if (orientationObj is int orientationInt)
            {
                component.SetDirection((Direction)orientationInt);
            }
            else if (orientationObj is string orientationStr && Enum.TryParse<Direction>(orientationStr, out var direction))
            {
                component.SetDirection(direction);
            }
        }
    }
}