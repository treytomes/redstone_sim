using OpenTK.Mathematics;

namespace RedstoneSimulator.Core.Common;

/// <summary>
/// Represents cardinal and diagonal directions in the 2D grid system.
/// </summary>
public enum Direction
{
    /// <summary>
    /// Represents the north (upward) direction.
    /// </summary>
    North,
    
    /// <summary>
    /// Represents the east (rightward) direction.
    /// </summary>
    East,
    
    /// <summary>
    /// Represents the south (downward) direction.
    /// </summary>
    South,
    
    /// <summary>
    /// Represents the west (leftward) direction.
    /// </summary>
    West,
    
    /// <summary>
    /// Represents the northeast (up-right) diagonal direction.
    /// </summary>
    NorthEast,
    
    /// <summary>
    /// Represents the southeast (down-right) diagonal direction.
    /// </summary>
    SouthEast,
    
    /// <summary>
    /// Represents the southwest (down-left) diagonal direction.
    /// </summary>
    SouthWest,
    
    /// <summary>
    /// Represents the northwest (up-left) diagonal direction.
    /// </summary>
    NorthWest
}

/// <summary>
/// Provides extension methods for the <see cref="Direction"/> enum.
/// </summary>
public static class DirectionExtensions
{
    /// <summary>
    /// A read-only collection of all cardinal directions.
    /// </summary>
    public static readonly IReadOnlyList<Direction> CardinalDirections = new[]
    {
        Direction.North,
        Direction.East,
        Direction.South,
        Direction.West
    };

    /// <summary>
    /// A read-only collection of all diagonal directions.
    /// </summary>
    public static readonly IReadOnlyList<Direction> DiagonalDirections = new[]
    {
        Direction.NorthEast,
        Direction.SouthEast,
        Direction.SouthWest,
        Direction.NorthWest
    };

    /// <summary>
    /// A read-only collection of all directions.
    /// </summary>
    public static readonly IReadOnlyList<Direction> AllDirections = new[]
    {
        Direction.North,
        Direction.East,
        Direction.South,
        Direction.West,
        Direction.NorthEast,
        Direction.SouthEast,
        Direction.SouthWest,
        Direction.NorthWest
    };

    /// <summary>
    /// Returns the opposite direction of the specified direction.
    /// </summary>
    /// <param name="direction">The direction to find the opposite of.</param>
    /// <returns>The opposite direction.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the direction is not recognized.</exception>
    public static Direction Opposite(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.South,
            Direction.East => Direction.West,
            Direction.South => Direction.North,
            Direction.West => Direction.East,
            Direction.NorthEast => Direction.SouthWest,
            Direction.SouthEast => Direction.NorthWest,
            Direction.SouthWest => Direction.NorthEast,
            Direction.NorthWest => Direction.SouthEast,
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
    }
    
    /// <summary>
    /// Determines whether the specified direction is a cardinal direction (North, East, South, West).
    /// </summary>
    /// <param name="direction">The direction to check.</param>
    /// <returns>True if the direction is cardinal; otherwise, false.</returns>
    public static bool IsCardinal(this Direction direction)
    {
        return CardinalDirections.Contains(direction);
    }
    
    /// <summary>
    /// Determines whether the specified direction is a diagonal direction (NorthEast, SouthEast, SouthWest, NorthWest).
    /// </summary>
    /// <param name="direction">The direction to check.</param>
    /// <returns>True if the direction is diagonal; otherwise, false.</returns>
    public static bool IsDiagonal(this Direction direction)
    {
        return DiagonalDirections.Contains(direction);
    }
    
    /// <summary>
    /// Gets all cardinal directions.
    /// </summary>
    /// <returns>An array containing all cardinal directions.</returns>
    [Obsolete("Use the CardinalDirections property instead.")]
    public static Direction[] GetCardinalDirections()
    {
        return CardinalDirections.ToArray();
    }
    
    /// <summary>
    /// Gets all diagonal directions.
    /// </summary>
    /// <returns>An array containing all diagonal directions.</returns>
    [Obsolete("Use the DiagonalDirections property instead.")]
    public static Direction[] GetDiagonalDirections()
    {
        return DiagonalDirections.ToArray();
    }
    
    /// <summary>
    /// Gets all directions (cardinal and diagonal).
    /// </summary>
    /// <returns>An array containing all directions.</returns>
    [Obsolete("Use the AllDirections property instead.")]
    public static Direction[] GetAllDirections()
    {
        return AllDirections.ToArray();
    }
    
    /// <summary>
    /// Gets the X component of the direction vector.
    /// </summary>
    /// <param name="direction">The direction.</param>
    /// <returns>-1 for west, 1 for east, 0 for north/south.</returns>
    public static int GetXComponent(this Direction direction)
    {
        return direction switch
        {
            Direction.East or Direction.NorthEast or Direction.SouthEast => 1,
            Direction.West or Direction.NorthWest or Direction.SouthWest => -1,
            _ => 0
        };
    }
    
    /// <summary>
    /// Gets the Y component of the direction vector.
    /// </summary>
    /// <param name="direction">The direction.</param>
    /// <returns>-1 for north, 1 for south, 0 for east/west.</returns>
    public static int GetYComponent(this Direction direction)
    {
        return direction switch
        {
            Direction.North or Direction.NorthEast or Direction.NorthWest => -1,
            Direction.South or Direction.SouthEast or Direction.SouthWest => 1,
            _ => 0
        };
    }
    
    /// <summary>
    /// Converts a direction to a Vector2i representation.
    /// </summary>
    /// <param name="direction">The direction to convert.</param>
    /// <returns>A Vector2i representing the direction.</returns>
    public static Vector2i ToVector(this Direction direction)
    {
        return new Vector2i(direction.GetXComponent(), direction.GetYComponent());
    }
    
    /// <summary>
    /// Rotates a direction clockwise by 90 degrees.
    /// </summary>
    /// <param name="direction">The direction to rotate.</param>
    /// <returns>The rotated direction.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the direction is not recognized.</exception>
    public static Direction RotateClockwise(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.East,
            Direction.East => Direction.South,
            Direction.South => Direction.West,
            Direction.West => Direction.North,
            Direction.NorthEast => Direction.SouthEast,
            Direction.SouthEast => Direction.SouthWest,
            Direction.SouthWest => Direction.NorthWest,
            Direction.NorthWest => Direction.NorthEast,
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
    }
    
    /// <summary>
    /// Rotates a direction counter-clockwise by 90 degrees.
    /// </summary>
    /// <param name="direction">The direction to rotate.</param>
    /// <returns>The rotated direction.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the direction is not recognized.</exception>
    public static Direction RotateCounterClockwise(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.West,
            Direction.East => Direction.North,
            Direction.South => Direction.East,
            Direction.West => Direction.South,
            Direction.NorthEast => Direction.NorthWest,
            Direction.SouthEast => Direction.NorthEast,
            Direction.SouthWest => Direction.SouthEast,
            Direction.NorthWest => Direction.SouthWest,
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
    }
}