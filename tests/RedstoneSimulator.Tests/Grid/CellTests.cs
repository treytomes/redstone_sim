using Xunit;
using Moq;
using RedstoneSimulator.Core.Grid;
using RedstoneSimulator.Core.Components;

namespace RedstoneSimulator.Tests.Grid
{
    public class CellTests
    {
        [Fact]
        public void NewCell_IsEmpty()
        {
            // Arrange & Act
            var cell = new Cell(5, 10);
            
            // Assert
            Assert.True(cell.IsEmpty());
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
            Assert.False(cell.IsEmpty());
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
    }
}