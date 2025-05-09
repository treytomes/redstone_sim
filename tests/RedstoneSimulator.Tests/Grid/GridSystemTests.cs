// tests\RedstoneSimulator.Core.Tests\Grid\GridSystemTests.cs

using OpenTK.Mathematics;
using RedstoneSimulator.Core.Common;
using RedstoneSimulator.Core.Grid;
using RedstoneSimulator.Core.Components;

namespace RedstoneSimulator.Core.Tests.Grid;

public class GridSystemTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_Default_CreatesUnboundedGrid()
    {
        // Arrange & Act
        var grid = new GridSystem();

        // Assert
        Assert.False(grid.HasBounds);
        Assert.Equal(0, grid.CellCount);
    }

    [Fact]
    public void Constructor_WithDimensions_CreatesBoundedGrid()
    {
        // Arrange & Act
        var grid = new GridSystem(10, 15);

        // Assert
        Assert.True(grid.HasBounds);
        Assert.Equal(10, grid.Width);
        Assert.Equal(15, grid.Height);
        Assert.Equal(10 * 15, grid.CellCount);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(10, 0)]
    [InlineData(-5, 10)]
    [InlineData(10, -5)]
    public void Constructor_WithInvalidDimensions_ThrowsArgumentOutOfRangeException(int width, int height)
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new GridSystem(width, height));
    }

    #endregion

    #region Component Placement Tests

    [Fact]
    public void PlaceComponent_ValidPosition_ReturnsTrue()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var component = new MockComponent();
        var position = new Vector2i(5, 5);

        // Act
        bool result = grid.PlaceComponent(component, position);

        // Assert
        Assert.True(result);
        Assert.Same(component, grid.GetComponentAt(position));
    }

    [Fact]
    public void PlaceComponent_OccupiedPosition_ReturnsFalse()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var component1 = new MockComponent();
        var component2 = new MockComponent();
        var position = new Vector2i(5, 5);
        grid.PlaceComponent(component1, position);

        // Act
        bool result = grid.PlaceComponent(component2, position);

        // Assert
        Assert.False(result);
        Assert.Same(component1, grid.GetComponentAt(position));
    }

    [Fact]
    public void PlaceComponent_OutOfBounds_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var component = new MockComponent();
        var position = new Vector2i(15, 15);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => grid.PlaceComponent(component, position));
    }

    [Fact]
    public void PlaceComponent_UnboundedGrid_CreatesNewCell()
    {
        // Arrange
        var grid = new GridSystem();
        var component = new MockComponent();
        var position = new Vector2i(100, 100);

        // Act
        bool result = grid.PlaceComponent(component, position);

        // Assert
        Assert.True(result);
        Assert.Same(component, grid.GetComponentAt(position));
        Assert.Equal(1, grid.CellCount);
    }

    [Fact]
    public void PlaceComponent_ComponentCannotBePlaced_ReturnsFalse()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var component = new MockComponent { CanBePlaced = false };
        var position = new Vector2i(5, 5);

        // Act
        bool result = grid.PlaceComponent(component, position);

        // Assert
        Assert.False(result);
        Assert.Null(grid.GetComponentAt(position));
    }

    [Fact]
    public void PlaceComponent_RaisesComponentPlacedEvent()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var component = new MockComponent();
        var position = new Vector2i(5, 5);
        
        GridComponentEventArgs? eventArgs = null;
        grid.ComponentPlaced += (sender, e) => eventArgs = e;

        // Act
        grid.PlaceComponent(component, position);

        // Assert
        Assert.NotNull(eventArgs);
        Assert.Equal(position, eventArgs.Position);
        Assert.Same(component, eventArgs.Component);
    }

    [Fact]
    public void PlaceComponent_MarksNeighborsForUpdate()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var component = new MockComponent();
        var position = new Vector2i(5, 5);
        
        // Act
        grid.PlaceComponent(component, position);
        
        // Assert
        var cellsToUpdate = grid.GetCellsToUpdate().ToList();
        Assert.Contains(position, cellsToUpdate);
        Assert.Contains(new Vector2i(4, 5), cellsToUpdate); // West
        Assert.Contains(new Vector2i(6, 5), cellsToUpdate); // East
        Assert.Contains(new Vector2i(5, 4), cellsToUpdate); // North
        Assert.Contains(new Vector2i(5, 6), cellsToUpdate); // South
    }

    #endregion

    #region Component Removal Tests

    [Fact]
    public void RemoveComponent_ExistingComponent_ReturnsComponent()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var component = new MockComponent();
        var position = new Vector2i(5, 5);
        grid.PlaceComponent(component, position);

        // Act
        var removedComponent = grid.RemoveComponent(position);

        // Assert
        Assert.Same(component, removedComponent);
        Assert.Null(grid.GetComponentAt(position));
    }

    [Fact]
    public void RemoveComponent_NoComponent_ReturnsNull()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var position = new Vector2i(5, 5);

        // Act
        var removedComponent = grid.RemoveComponent(position);

        // Assert
        Assert.Null(removedComponent);
    }

    [Fact]
    public void RemoveComponent_OutOfBounds_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var position = new Vector2i(15, 15);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => grid.RemoveComponent(position));
    }

    [Fact]
    public void RemoveComponent_RaisesComponentRemovedEvent()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var component = new MockComponent();
        var position = new Vector2i(5, 5);
        grid.PlaceComponent(component, position);
        
        GridComponentEventArgs? eventArgs = null;
        grid.ComponentRemoved += (sender, e) => eventArgs = e;

        // Act
        grid.RemoveComponent(position);

        // Assert
        Assert.NotNull(eventArgs);
        Assert.Equal(position, eventArgs.Position);
        Assert.Same(component, eventArgs.Component);
    }

    [Fact]
    public void RemoveComponent_MarksNeighborsForUpdate()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var component = new MockComponent();
        var position = new Vector2i(5, 5);
        grid.PlaceComponent(component, position);
        grid.ClearUpdateQueue(); // Clear initial update marks
        
        // Act
        grid.RemoveComponent(position);
        
        // Assert
        var cellsToUpdate = grid.GetCellsToUpdate().ToList();
        Assert.Contains(new Vector2i(4, 5), cellsToUpdate); // West
        Assert.Contains(new Vector2i(6, 5), cellsToUpdate); // East
        Assert.Contains(new Vector2i(5, 4), cellsToUpdate); // North
        Assert.Contains(new Vector2i(5, 6), cellsToUpdate); // South
    }

    #endregion

    #region Cell and Component Retrieval Tests

    [Fact]
    public void GetComponentAt_ExistingComponent_ReturnsComponent()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var component = new MockComponent();
        var position = new Vector2i(5, 5);
        grid.PlaceComponent(component, position);

        // Act
        var retrievedComponent = grid.GetComponentAt(position);

        // Assert
        Assert.Same(component, retrievedComponent);
    }

    [Fact]
    public void GetComponentAt_NoComponent_ReturnsNull()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var position = new Vector2i(5, 5);

        // Act
        var retrievedComponent = grid.GetComponentAt(position);

        // Assert
        Assert.Null(retrievedComponent);
    }

    [Fact]
    public void GetCellAt_ExistingCell_ReturnsCell()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var position = new Vector2i(5, 5);

        // Act
        var cell = grid.GetCellAt(position);

        // Assert
        Assert.NotNull(cell);
        Assert.Equal(position.X, cell.X);
        Assert.Equal(position.Y, cell.Y);
    }

    [Fact]
    public void GetCellAt_OutOfBounds_ReturnsNull()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var position = new Vector2i(15, 15);

        // Act
        var cell = grid.GetCellAt(position);

        // Assert
        Assert.Null(cell);
    }

    [Fact]
    public void GetCellAt_UnboundedGrid_CreatesNewCell()
    {
        // Arrange
        var grid = new GridSystem();
        var position = new Vector2i(100, 100);

        // Act
        var cell = grid.GetCellAt(position);

        // Assert
        Assert.NotNull(cell);
        Assert.Equal(position.X, cell.X);
        Assert.Equal(position.Y, cell.Y);
        Assert.Equal(1, grid.CellCount);
    }

    #endregion

    #region Grid Resizing Tests

    [Fact]
    public void ResizeGrid_Larger_PreservesExistingCells()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var component = new MockComponent();
        var position = new Vector2i(5, 5);
        grid.PlaceComponent(component, position);

        // Act
        grid.ResizeGrid(15, 15);

        // Assert
        Assert.Equal(15, grid.Width);
        Assert.Equal(15, grid.Height);
        Assert.Equal(15 * 15, grid.CellCount);
        Assert.Same(component, grid.GetComponentAt(position));
    }

    [Fact]
    public void ResizeGrid_Smaller_RemovesOutOfBoundsCells()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var component1 = new MockComponent();
        var component2 = new MockComponent();
        grid.PlaceComponent(component1, new Vector2i(5, 5));
        grid.PlaceComponent(component2, new Vector2i(9, 9));

        // Act
        grid.ResizeGrid(8, 8);

        // Assert
        Assert.Equal(8, grid.Width);
        Assert.Equal(8, grid.Height);
        Assert.Equal(8 * 8, grid.CellCount);
        Assert.Same(component1, grid.GetComponentAt(new Vector2i(5, 5)));
        Assert.Null(grid.GetComponentAt(new Vector2i(9, 9)));
    }

    [Fact]
    public void ResizeGrid_FromUnbounded_RemovesOutOfBoundsCells()
    {
        // Arrange
        var grid = new GridSystem();
        var component1 = new MockComponent();
        var component2 = new MockComponent();
        grid.PlaceComponent(component1, new Vector2i(5, 5));
        grid.PlaceComponent(component2, new Vector2i(15, 15));

        // Act
        grid.ResizeGrid(10, 10);

        // Assert
        Assert.Equal(10, grid.Width);
        Assert.Equal(10, grid.Height);
        Assert.Equal(10 * 10, grid.CellCount);
        Assert.Same(component1, grid.GetComponentAt(new Vector2i(5, 5)));
        Assert.Null(grid.GetComponentAt(new Vector2i(15, 15)));
    }

    [Fact]
    public void ResizeGrid_InvalidDimensions_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var grid = new GridSystem(10, 10);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => grid.ResizeGrid(0, 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => grid.ResizeGrid(10, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => grid.ResizeGrid(-5, 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => grid.ResizeGrid(10, -5));
    }

    [Fact]
    public void ResizeGrid_RaisesGridResizedEvent()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        GridResizeEventArgs? eventArgs = null;
        grid.GridResized += (sender, e) => eventArgs = e;

        // Act
        grid.ResizeGrid(15, 20);

        // Assert
        Assert.NotNull(eventArgs);
        Assert.Equal(10, eventArgs.OldWidth);
        Assert.Equal(10, eventArgs.OldHeight);
        Assert.Equal(15, eventArgs.NewWidth);
        Assert.Equal(20, eventArgs.NewHeight);
    }

    [Fact]
    public void MakeUnbounded_FromBounded_PreservesExistingCells()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var component = new MockComponent();
        var position = new Vector2i(5, 5);
        grid.PlaceComponent(component, position);
        int initialCellCount = grid.CellCount;

        // Act
        grid.MakeUnbounded();

        // Assert
        Assert.False(grid.HasBounds);
        Assert.Equal(initialCellCount, grid.CellCount);
        Assert.Same(component, grid.GetComponentAt(position));
    }

    [Fact]
    public void MakeUnbounded_RaisesGridResizedEvent()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        GridResizeEventArgs? eventArgs = null;
        grid.GridResized += (sender, e) => eventArgs = e;

        // Act
        grid.MakeUnbounded();

        // Assert
        Assert.NotNull(eventArgs);
        Assert.Equal(10, eventArgs.OldWidth);
        Assert.Equal(10, eventArgs.OldHeight);
        Assert.Equal(0, eventArgs.NewWidth);
        Assert.Equal(0, eventArgs.NewHeight);
    }

    #endregion

    #region Grid Clearing Tests

    [Fact]
    public void Clear_BoundedGrid_RemovesAllComponents()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        grid.PlaceComponent(new MockComponent(), new Vector2i(3, 3));
        grid.PlaceComponent(new MockComponent(), new Vector2i(5, 5));
        grid.PlaceComponent(new MockComponent(), new Vector2i(7, 7));

        // Act
        grid.Clear();

        // Assert
        Assert.Equal(10 * 10, grid.CellCount); // Cells remain, but components are removed
        Assert.Null(grid.GetComponentAt(new Vector2i(3, 3)));
        Assert.Null(grid.GetComponentAt(new Vector2i(5, 5)));
        Assert.Null(grid.GetComponentAt(new Vector2i(7, 7)));
        Assert.Empty(grid.GetCellsToUpdate());
    }

    [Fact]
    public void Clear_UnboundedGrid_RemovesAllCells()
    {
        // Arrange
        var grid = new GridSystem();
        grid.PlaceComponent(new MockComponent(), new Vector2i(3, 3));
        grid.PlaceComponent(new MockComponent(), new Vector2i(5, 5));
        grid.PlaceComponent(new MockComponent(), new Vector2i(7, 7));

        // Act
        grid.Clear();

        // Assert
        Assert.Equal(0, grid.CellCount);
        Assert.Null(grid.GetComponentAt(new Vector2i(3, 3)));
        Assert.Null(grid.GetComponentAt(new Vector2i(5, 5)));
        Assert.Null(grid.GetComponentAt(new Vector2i(7, 7)));
        Assert.Empty(grid.GetCellsToUpdate());
    }

    [Fact]
    public void Clear_RaisesGridClearedEvent()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        bool eventRaised = false;
        grid.GridCleared += (sender, e) => eventRaised = true;

        // Act
        grid.Clear();

        // Assert
        Assert.True(eventRaised);
    }

    #endregion

    #region Update Queue Tests

    [Fact]
    public void MarkCellForUpdate_ValidCell_AddsToCellsToUpdate()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var position = new Vector2i(5, 5);

        // Act
        bool result = grid.MarkCellForUpdate(position);

        // Assert
        Assert.True(result);
        Assert.Contains(position, grid.GetCellsToUpdate());
    }

    [Fact]
    public void MarkCellForUpdate_InvalidCell_ReturnsFalse()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var position = new Vector2i(15, 15);

        // Act
        bool result = grid.MarkCellForUpdate(position);

        // Assert
        Assert.False(result);
        Assert.DoesNotContain(position, grid.GetCellsToUpdate());
    }

    [Fact]
    public void MarkCellForUpdate_RaisesCellMarkedForUpdateEvent()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var position = new Vector2i(5, 5);
        GridCellEventArgs? eventArgs = null;
        grid.CellMarkedForUpdate += (sender, e) => eventArgs = e;

        // Act
        grid.MarkCellForUpdate(position);

        // Assert
        Assert.NotNull(eventArgs);
        Assert.Equal(position, eventArgs.Position);
    }

    [Fact]
    public void UpdateMarkedCells_UpdatesAllMarkedCells()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var component1 = new MockComponent();
        var component2 = new MockComponent();
        grid.PlaceComponent(component1, new Vector2i(3, 3));
        grid.PlaceComponent(component2, new Vector2i(5, 5));
        grid.ClearUpdateQueue(); // Clear initial update marks
        grid.MarkCellForUpdate(new Vector2i(3, 3));
        grid.MarkCellForUpdate(new Vector2i(5, 5));

        // Act
        int updatedCount = grid.UpdateMarkedCells(0.1f);

        // Assert
        Assert.Equal(2, updatedCount);
        Assert.Equal(1, component1.UpdateCount);
        Assert.Equal(1, component2.UpdateCount);
        Assert.Empty(grid.GetCellsToUpdate());
    }

    [Fact]
    public void ClearUpdateQueue_RemovesAllMarkedCells()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        grid.MarkCellForUpdate(new Vector2i(3, 3));
        grid.MarkCellForUpdate(new Vector2i(5, 5));

        // Act
        grid.ClearUpdateQueue();

        // Assert
        Assert.Empty(grid.GetCellsToUpdate());
    }

    #endregion

    #region Neighbor Tests

    [Fact]
    public void GetNeighborCells_ReturnsCorrectCells()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var position = new Vector2i(5, 5);

        // Act
        var neighbors = grid.GetNeighborCells(position);

        // Assert
        Assert.Equal(4, neighbors.Count);
        Assert.NotNull(neighbors[Direction.North]);
        Assert.NotNull(neighbors[Direction.East]);
        Assert.NotNull(neighbors[Direction.South]);
        Assert.NotNull(neighbors[Direction.West]);
        Assert.Equal(position.X, neighbors[Direction.North]!.X);
        Assert.Equal(position.Y - 1, neighbors[Direction.North]!.Y);
        Assert.Equal(position.X + 1, neighbors[Direction.East]!.X);
        Assert.Equal(position.Y, neighbors[Direction.East]!.Y);
        Assert.Equal(position.X, neighbors[Direction.South]!.X);
        Assert.Equal(position.Y + 1, neighbors[Direction.South]!.Y);
        Assert.Equal(position.X - 1, neighbors[Direction.West]!.X);
        Assert.Equal(position.Y, neighbors[Direction.West]!.Y);
    }

    [Fact]
    public void GetNeighborCells_AtEdge_ReturnsNullForOutOfBounds()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var position = new Vector2i(0, 0); // Top-left corner

        // Act
        var neighbors = grid.GetNeighborCells(position);

        // Assert
        Assert.Equal(4, neighbors.Count);
        Assert.Null(neighbors[Direction.North]); // Out of bounds
        Assert.NotNull(neighbors[Direction.East]);
        Assert.NotNull(neighbors[Direction.South]);
        Assert.Null(neighbors[Direction.West]); // Out of bounds
    }

    [Fact]
    public void GetNeighborCells_UnboundedGrid_CreatesNewCells()
    {
        // Arrange
        var grid = new GridSystem();
        var position = new Vector2i(0, 0);
        
        // First, make sure the cell at the position exists
        var cell = grid.GetCellAt(position);
        
        // Act
        var neighbors = grid.GetNeighborCells(position);
        
        // Assert
        Assert.Equal(4, neighbors.Count);
        Assert.NotNull(neighbors[Direction.North]);
        Assert.NotNull(neighbors[Direction.East]);
        Assert.NotNull(neighbors[Direction.South]);
        Assert.NotNull(neighbors[Direction.West]);
        Assert.Equal(5, grid.CellCount); // Original cell + 4 neighbors
    }

    [Fact]
    public void GetNeighborComponents_ReturnsCorrectComponents()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var position = new Vector2i(5, 5);
        var eastComponent = new MockComponent();
        grid.PlaceComponent(eastComponent, new Vector2i(6, 5)); // East

        // Act
        var neighbors = grid.GetNeighborComponents(position);

        // Assert
        Assert.Equal(4, neighbors.Count);
        Assert.Null(neighbors[Direction.North]);
        Assert.Same(eastComponent, neighbors[Direction.East]);
        Assert.Null(neighbors[Direction.South]);
        Assert.Null(neighbors[Direction.West]);
    }

    #endregion

    #region Utility Method Tests

    [Fact]
    public void IsWithinBounds_BoundedGrid_ReturnsCorrectResult()
    {
        // Arrange
        var grid = new GridSystem(10, 10);

        // Act & Assert
        Assert.True(grid.IsWithinBounds(new Vector2i(0, 0)));
        Assert.True(grid.IsWithinBounds(new Vector2i(9, 9)));
        Assert.False(grid.IsWithinBounds(new Vector2i(10, 5)));
        Assert.False(grid.IsWithinBounds(new Vector2i(5, 10)));
        Assert.False(grid.IsWithinBounds(new Vector2i(-1, 5)));
        Assert.False(grid.IsWithinBounds(new Vector2i(5, -1)));
    }

    [Fact]
    public void IsWithinBounds_UnboundedGrid_AlwaysReturnsTrue()
    {
        // Arrange
        var grid = new GridSystem();

        // Act & Assert
        Assert.True(grid.IsWithinBounds(new Vector2i(0, 0)));
        Assert.True(grid.IsWithinBounds(new Vector2i(1000, 1000)));
        Assert.True(grid.IsWithinBounds(new Vector2i(-1000, -1000)));
    }

    [Fact]
    public void CanPlaceComponentAt_ValidPosition_ReturnsTrue()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var component = new MockComponent { CanBePlaced = true };

        // Act & Assert
        Assert.True(grid.CanPlaceComponentAt(component, new Vector2i(5, 5)));
    }

    [Fact]
    public void CanPlaceComponentAt_OccupiedPosition_ReturnsFalse()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        grid.PlaceComponent(new MockComponent(), new Vector2i(5, 5));
        var component = new MockComponent();

        // Act & Assert
        Assert.False(grid.CanPlaceComponentAt(component, new Vector2i(5, 5)));
    }

    [Fact]
    public void CanPlaceComponentAt_ComponentCannotBePlaced_ReturnsFalse()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var component = new MockComponent { CanBePlaced = false };

        // Act & Assert
        Assert.False(grid.CanPlaceComponentAt(component, new Vector2i(5, 5)));
    }

    [Fact]
    public void CanPlaceComponentAt_OutOfBounds_ReturnsFalse()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        var component = new MockComponent();

        // Act & Assert
        Assert.False(grid.CanPlaceComponentAt(component, new Vector2i(15, 15)));
    }

    [Fact]
    public void FindComponentsByType_ReturnsCorrectPositions()
    {
        // Arrange
        var grid = new GridSystem(10, 10);
        grid.PlaceComponent(new MockComponent(ComponentType.RedstoneWire), new Vector2i(3, 3));
        grid.PlaceComponent(new MockComponent(ComponentType.RedstoneTorch), new Vector2i(5, 5));
        grid.PlaceComponent(new MockComponent(ComponentType.RedstoneWire), new Vector2i(7, 7));

        // Act
        var wirePositions = grid.FindComponentsByType(ComponentType.RedstoneWire).ToList();
        var torchPositions = grid.FindComponentsByType(ComponentType.RedstoneTorch).ToList();
        var buttonPositions = grid.FindComponentsByType(ComponentType.Button).ToList();

        // Assert
        Assert.Equal(2, wirePositions.Count);
        Assert.Contains(new Vector2i(3, 3), wirePositions);
        Assert.Contains(new Vector2i(7, 7), wirePositions);
        
        Assert.Single(torchPositions);
        Assert.Contains(new Vector2i(5, 5), torchPositions);
        
        Assert.Empty(buttonPositions);
    }

    #endregion
}