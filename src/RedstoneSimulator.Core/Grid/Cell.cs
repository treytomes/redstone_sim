using RedstoneSimulator.Core.Components;

namespace RedstoneSimulator.Core.Grid;

public class Cell
{
	#region Constructors

	public Cell(int x, int y)
	{
		X = x;
		Y = y;
	}

	#endregion

	#region Properties

	public int X { get; }
	public int Y { get; }

	public IComponent? Component { get; private set; }

	public bool IsEmpty => Component == null;
	
	#endregion
	
	#region Methods

	public IComponent? GetComponent() => Component;

	public void SetComponent(IComponent? component)
	{
		Component = component;
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as Cell);
	}
	
	public bool Equals(Cell? other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;
		return X == other.X && Y == other.Y;
	}
	
	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}
	
	public static bool operator ==(Cell? left, Cell? right)
	{
		return Equals(left, right);
	}
	
	public static bool operator !=(Cell? left, Cell? right)
	{
		return !Equals(left, right);
	}

	#endregion
}
