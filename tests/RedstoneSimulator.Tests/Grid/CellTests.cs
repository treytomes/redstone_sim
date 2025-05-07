using Moq;
using RedstoneSimulator.Core.Grid;
using RedstoneSimulator.Core.Components;

namespace RedstoneSimulator.Tests.Grid;

public class CellTests
{
	[Fact]
	public void NewCell_IsEmpty()
	{
		// Arrange & Act
		var cell = new Cell(5, 10);
		
		// Assert
		Assert.True(cell.IsEmpty);
	}

	[Fact]
	public void SetComponent_CellIsNotEmpty()
	{
		// Arrange
		var cell = new Cell(1, 2);
		var mockComponent = new Mock<IComponent>();
		
		// Act
		cell.SetComponent(mockComponent.Object);
		
		// Assert
		Assert.False(cell.IsEmpty);
	}

	[Fact]
	public void GetComponent_ReturnsSetComponent()
	{
		// Arrange
		var cell = new Cell(1, 2);
		var mockComponent = new Mock<IComponent>();
		
		// Act
		cell.SetComponent(mockComponent.Object);
		var result = cell.GetComponent();
		
		// Assert
		Assert.Same(mockComponent.Object, result);
	}

	[Fact]
	public void CellCoordinates_MatchConstructorValues()
	{
		// Arrange & Act
		var cell = new Cell(42, 24);
		
		// Assert
		Assert.Equal(42, cell.X);
		Assert.Equal(24, cell.Y);
	}

    [Fact]
    public void SetComponent_CanReplaceExistingComponent()
    {
        // Arrange
        var cell = new Cell(1, 2);
        var mockComponent1 = new Mock<IComponent>();
        var mockComponent2 = new Mock<IComponent>();
        cell.SetComponent(mockComponent1.Object);
        
        // Act
        cell.SetComponent(mockComponent2.Object);
        var result = cell.GetComponent();
        
        // Assert
        Assert.Same(mockComponent2.Object, result);
        Assert.NotSame(mockComponent1.Object, result);
    }
    
    [Fact]
    public void SetComponent_CanClearComponent()
    {
        // Arrange
        var cell = new Cell(1, 2);
        var mockComponent = new Mock<IComponent>();
        cell.SetComponent(mockComponent.Object);
        
        // Act
        cell.SetComponent(null);
        
        // Assert
        Assert.True(cell.IsEmpty);
        Assert.Null(cell.GetComponent());
    }
    
    [Fact]
    public void GetComponent_ReturnsNullWhenEmpty()
    {
        // Arrange
        var cell = new Cell(3, 4);
        
        // Act
        var result = cell.GetComponent();
        
        // Assert
        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(0, 0)]
    [InlineData(-5, 10)]
    [InlineData(100, -200)]
    public void Cell_StoresCoordinatesCorrectly(int x, int y)
    {
        // Arrange & Act
        var cell = new Cell(x, y);
        
        // Assert
        Assert.Equal(x, cell.X);
        Assert.Equal(y, cell.Y);
    }
    
    [Fact]
    public void Equals_ReturnsTrueForSameCoordinates()
    {
        // Arrange
        var cell1 = new Cell(5, 5);
        var cell2 = new Cell(5, 5);
        
        // Act & Assert
        Assert.Equal(cell1, cell2); // This will require implementing IEquatable<Cell> or overriding Equals
    }
    
    [Fact]
    public void Equals_ReturnsFalseForDifferentCoordinates()
    {
        // Arrange
        var cell1 = new Cell(5, 5);
        var cell2 = new Cell(5, 6);
        
        // Act & Assert
        Assert.NotEqual(cell1, cell2);
    }
    
    [Fact]
    public void GetHashCode_SameForEqualCells()
    {
        // Arrange
        var cell1 = new Cell(3, 7);
        var cell2 = new Cell(3, 7);
        
        // Act & Assert
        Assert.Equal(cell1.GetHashCode(), cell2.GetHashCode());
    }
}