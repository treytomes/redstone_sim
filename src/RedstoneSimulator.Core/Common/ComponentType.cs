namespace RedstoneSimulator.Core.Common;

/// <summary>
/// Defines the different types of components available in the redstone simulator.
/// </summary>
public enum ComponentType
{
    /// <summary>
    /// Represents an empty or undefined component.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// A basic redstone wire that transmits power with signal decay.
    /// </summary>
    RedstoneWire,
    
    /// <summary>
    /// A solid block that doesn't conduct redstone power.
    /// </summary>
    SolidBlock,
    
    /// <summary>
    /// A momentary button that generates a pulse of power.
    /// </summary>
    Button,
    
    /// <summary>
    /// A lever that can be toggled to provide continuous power.
    /// </summary>
    Switch,
    
    /// <summary>
    /// A redstone torch that provides power or inverts input signals.
    /// </summary>
    RedstoneTorch,
    
    /// <summary>
    /// A repeater that restores signal strength and adds configurable delay.
    /// </summary>
    Repeater,
    
    /// <summary>
    /// A comparator that compares signal strengths or performs subtraction.
    /// </summary>
    Comparator,
    
    /// <summary>
    /// A transistor that allows power to pass when controlled by another input.
    /// </summary>
    Transistor,
    
    /// <summary>
    /// An observer that detects changes in adjacent blocks.
    /// </summary>
    Observer,
    
    /// <summary>
    /// A redstone lamp that lights up when powered.
    /// </summary>
    RedstoneLamp,
    
    /// <summary>
    /// A piston that can push blocks when powered.
    /// </summary>
    Piston,
    
    /// <summary>
    /// A sticky piston that can push and pull blocks when powered.
    /// </summary>
    StickyPiston,
    
    /// <summary>
    /// A redstone dust cross that allows signals to pass without connecting.
    /// </summary>
    RedstoneCross,
    
    /// <summary>
    /// A target block that conducts power in all directions when hit.
    /// </summary>
    TargetBlock,
    
    /// <summary>
    /// A hopper that can transfer items when not powered.
    /// </summary>
    Hopper,
    
    /// <summary>
    /// A dropper that can eject items when powered.
    /// </summary>
    Dropper,
    
    /// <summary>
    /// A dispenser that can use items when powered.
    /// </summary>
    Dispenser,
    
    /// <summary>
    /// A note block that plays a note when powered.
    /// </summary>
    NoteBlock,
    
    /// <summary>
    /// A door that opens and closes when powered.
    /// </summary>
    Door,
    
    /// <summary>
    /// A trapdoor that opens and closes when powered.
    /// </summary>
    Trapdoor,
    
    /// <summary>
    /// A fence gate that opens and closes when powered.
    /// </summary>
    FenceGate,
    
    /// <summary>
    /// A rail that can change direction when powered.
    /// </summary>
    PoweredRail,
    
    /// <summary>
    /// A rail that can activate powered minecarts.
    /// </summary>
    DetectorRail,
    
    /// <summary>
    /// A rail that can change direction when powered.
    /// </summary>
    ActivatorRail,
    
    /// <summary>
    /// A command block that executes commands when powered.
    /// </summary>
    CommandBlock,
    
    /// <summary>
    /// A daylight sensor that outputs power based on time of day.
    /// </summary>
    DaylightSensor
}
