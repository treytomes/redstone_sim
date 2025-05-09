// src\RedstoneSimulator.Core\Grid\Cell.cs

using OpenTK.Mathematics;
using RedstoneSimulator.Core.Common;
using RedstoneSimulator.Core.Components;

namespace RedstoneSimulator.Core.Grid;

/// <summary>
/// Represents a single cell in the redstone circuit grid that can contain a component.
/// Cells are identified by their X and Y coordinates and can hold at most one component.
/// </summary>
public class Cell : IEquatable<Cell>
{
    #region Events

    /// <summary>
    /// Event raised when the component in this cell changes.
    /// </summary>
    public event EventHandler<ComponentChangedEventArgs>? ComponentChanged;

    /// <summary>
    /// Event raised when this cell is marked for update in the simulation.
    /// </summary>
    public event EventHandler? MarkedForUpdate;

    #endregion

    #region Fields

    /// <summary>
    /// Dictionary for storing additional cell state information.
    /// </summary>
    private readonly Dictionary<string, object> _stateData = new();

    /// <summary>
    /// The current component contained in this cell.
    /// </summary>
    private IComponent? _component;

    /// <summary>
    /// Flag indicating whether this cell needs to be updated in the next simulation tick.
    /// </summary>
    private bool _needsUpdate;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Cell"/> class with the specified coordinates.
    /// </summary>
    /// <param name="x">The X coordinate of the cell.</param>
    /// <param name="y">The Y coordinate of the cell.</param>
    public Cell(int x, int y)
    {
        X = x;
        Y = y;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the X coordinate of the cell.
    /// </summary>
    public int X { get; }

    /// <summary>
    /// Gets the Y coordinate of the cell.
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Gets the component contained in this cell.
    /// </summary>
    /// <remarks>
    /// Use <see cref="SetComponent"/> to modify this property to ensure proper notifications.
    /// </remarks>
    public IComponent? Component => _component;

    /// <summary>
    /// Gets a value indicating whether this cell is empty (contains no component).
    /// </summary>
    public bool IsEmpty => Component == null;

    /// <summary>
    /// Gets a value indicating whether this cell needs to be updated in the next simulation tick.
    /// </summary>
    public bool NeedsUpdate => _needsUpdate;

    #endregion
    
    #region Methods

    /// <summary>
    /// Gets the component contained in this cell.
    /// </summary>
    /// <returns>The component in this cell, or null if the cell is empty.</returns>
    public IComponent? GetComponent() => Component;

    /// <summary>
    /// Sets the component contained in this cell, replacing any existing component.
    /// </summary>
    /// <param name="component">The component to place in this cell, or null to clear the cell.</param>
    /// <exception cref="InvalidOperationException">Thrown when the component cannot be placed in this cell.</exception>
    public void SetComponent(IComponent? component)
    {
        // Check if the component can be placed in this cell
        if (component != null && component is IComponent validComponent)
        {
            if (!validComponent.CanBePlacedInCell(this))
            {
                throw new InvalidOperationException($"Component {component.GetType().Name} cannot be placed in cell at ({X}, {Y}).");
            }
        }

        var oldComponent = _component;
        
        // Notify the old component it's being removed
        if (oldComponent != null)
        {
            oldComponent.OnRemovedFromCell(this);
        }
        
        _component = component;
        
        // Notify the new component it's being added
        if (_component != null)
        {
            _component.OnAddedToCell(this);
        }
        
        // Raise the component changed event
        OnComponentChanged(new ComponentChangedEventArgs(oldComponent, _component));
    }

    /// <summary>
    /// Gets the type of the component in this cell.
    /// </summary>
    /// <returns>The component type, or null if the cell is empty.</returns>
    public ComponentType? GetComponentType()
    {
        if (Component is IWireComponent wireComponent)
        {
            return wireComponent.GetComponentType();
        }
        
        return null;
    }

    /// <summary>
    /// Gets the power level of the component in this cell.
    /// </summary>
    /// <returns>The power level (0-15), or 0 if the cell is empty or the component is not powerable.</returns>
    public int GetPowerLevel()
    {
        if (Component is IPowerableComponent powerableComponent)
        {
            return powerableComponent.PowerLevel;
        }
        
        return 0;
    }

    /// <summary>
    /// Sets the direction of a directional component in this cell.
    /// </summary>
    /// <param name="direction">The direction to set.</param>
    /// <exception cref="InvalidOperationException">Thrown when the component is not directional.</exception>
    public void RotateComponent(Direction direction)
    {
        if (Component is IDirectionalComponent directionalComponent)
        {
            directionalComponent.SetDirection(direction);
        }
        else
        {
            throw new InvalidOperationException("Cannot rotate a non-directional component.");
        }
    }

    /// <summary>
    /// Gets the positions of neighboring cells.
    /// </summary>
    /// <returns>A collection of Vector2i positions representing neighboring cells.</returns>
    public ICollection<Vector2i> GetNeighborPositions()
    {
        return new List<Vector2i>
        {
            new Vector2i(X - 1, Y),  // West
            new Vector2i(X + 1, Y),  // East
            new Vector2i(X, Y - 1),  // North
            new Vector2i(X, Y + 1)   // South
        };
    }

    /// <summary>
    /// Determines whether the component in this cell can connect to another cell in the specified direction.
    /// </summary>
    /// <param name="direction">The direction to check.</param>
    /// <param name="otherCell">The other cell to check for connectivity.</param>
    /// <returns>True if the components can connect, false otherwise.</returns>
    public bool CanConnectTo(Direction direction, Cell otherCell)
    {
        if (Component is IConnectableComponent connectableComponent && otherCell?.Component != null)
        {
            return connectableComponent.CanConnectTo(direction, otherCell.Component);
        }
        
        return false;
    }

    /// <summary>
    /// Activates the component in this cell if it supports activation.
    /// </summary>
    public void Activate()
    {
        if (Component is IActivatableComponent activatableComponent)
        {
            activatableComponent.Activate();
        }
    }

    /// <summary>
    /// Updates the component in this cell during a simulation tick.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update.</param>
    public void Update(float deltaTime)
    {
        Component?.Update(deltaTime);
        _needsUpdate = false;
    }

    /// <summary>
    /// Marks this cell for update in the next simulation tick.
    /// </summary>
    public void MarkForUpdate()
    {
        _needsUpdate = true;
        OnMarkedForUpdate(EventArgs.Empty);
    }

    /// <summary>
    /// Sets additional state data for this cell.
    /// </summary>
    /// <param name="key">The key for the state data.</param>
    /// <param name="value">The value to store.</param>
    public void SetState(string key, object value)
    {
        _stateData[key] = value;
    }

    /// <summary>
    /// Gets state data from this cell.
    /// </summary>
    /// <typeparam name="T">The type of the state data.</typeparam>
    /// <param name="key">The key for the state data.</param>
    /// <returns>The state data value.</returns>
    public T GetState<T>(string key)
    {
        if (_stateData.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        
        return default!;
    }

    /// <summary>
    /// Performs a special action on this cell.
    /// </summary>
    /// <param name="actionName">The name of the action to perform.</param>
    /// <exception cref="InvalidOperationException">Thrown when the action is not supported.</exception>
    public void PerformSpecialAction(string actionName)
    {
        throw new InvalidOperationException($"The action '{actionName}' is an unsupported action for this cell.");
    }

    /// <summary>
    /// Creates a clone of this cell.
    /// </summary>
    /// <returns>A new cell with the same coordinates and component (if supported).</returns>
    public Cell Clone()
    {
        var clonedCell = new Cell(X, Y);
        
        // Clone component if it supports cloning
        if (Component is ICloneable cloneableComponent)
        {
            clonedCell.SetComponent((IComponent)cloneableComponent.Clone());
        }
        
        // Copy state data
        foreach (var kvp in _stateData)
        {
            clonedCell._stateData[kvp.Key] = kvp.Value;
        }
        
        return clonedCell;
    }

    /// <summary>
    /// Serializes this cell to a format that can be saved or transmitted.
    /// </summary>
    /// <returns>The serialized representation of the cell.</returns>
    public object Serialize()
    {
        var serializedData = new Dictionary<string, object>
        {
            ["X"] = X,
            ["Y"] = Y
        };
        
        if (Component != null)
        {
            serializedData["Component"] = Component.GetSerializationData();
        }
        
        return serializedData;
    }

    /// <summary>
    /// Deserializes a cell from its serialized representation.
    /// </summary>
    /// <param name="serialized">The serialized cell data.</param>
    /// <returns>A new cell constructed from the serialized data.</returns>
    public static Cell Deserialize(object serialized)
    {
        // Implementation would depend on the serialization format
        // This is a placeholder that assumes serialized is a Dictionary
        if (serialized is Dictionary<string, object> data)
        {
            var x = (int)data["X"];
            var y = (int)data["Y"];
            var cell = new Cell(x, y);
            
            // Component deserialization would be handled by a factory
            if (data.TryGetValue("Component", out var componentData) && componentData is ComponentData typedComponentData)
            {
                var componentFactory = ComponentFactory.Instance;
                var component = componentFactory.CreateFromData(typedComponentData);
                cell.SetComponent(component);
            }
            
            return cell;
        }
        
        throw new ArgumentException("Invalid serialized data format", nameof(serialized));
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current cell.
    /// </summary>
    /// <param name="obj">The object to compare with the current cell.</param>
    /// <returns>true if the specified object is equal to the current cell; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Cell);
    }
    
    /// <summary>
    /// Determines whether the specified cell is equal to the current cell.
    /// </summary>
    /// <param name="other">The cell to compare with the current cell.</param>
    /// <returns>true if the specified cell is equal to the current cell; otherwise, false.</returns>
    public bool Equals(Cell? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return X == other.X && Y == other.Y;
    }
    
    /// <summary>
    /// Returns a hash code for this cell.
    /// </summary>
    /// <returns>A hash code for the current cell.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
    
    /// <summary>
    /// Returns a string that represents the current cell.
    /// </summary>
    /// <returns>A string that represents the current cell.</returns>
    public override string ToString()
    {
        return $"Cell({X}, {Y}){(IsEmpty ? " Empty" : $" {Component?.GetType().Name}")}";
    }
    
    /// <summary>
    /// Determines whether two cells are equal.
    /// </summary>
    /// <param name="left">The first cell to compare.</param>
    /// <param name="right">The second cell to compare.</param>
    /// <returns>true if the cells are equal; otherwise, false.</returns>
    public static bool operator ==(Cell? left, Cell? right)
    {
        return Equals(left, right);
    }
    
    /// <summary>
    /// Determines whether two cells are not equal.
    /// </summary>
    /// <param name="left">The first cell to compare.</param>
    /// <param name="right">The second cell to compare.</param>
    /// <returns>true if the cells are not equal; otherwise, false.</returns>
    public static bool operator !=(Cell? left, Cell? right)
    {
        return !Equals(left, right);
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Raises the ComponentChanged event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnComponentChanged(ComponentChangedEventArgs e)
    {
        ComponentChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the MarkedForUpdate event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnMarkedForUpdate(EventArgs e)
    {
        MarkedForUpdate?.Invoke(this, e);
    }

    #endregion
}
