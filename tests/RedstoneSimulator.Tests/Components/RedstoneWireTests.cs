using System.Collections.Generic;
using Xunit;
using Moq;
using RedstoneSimulator.Core.Components;
using RedstoneSimulator.Core.Common;

namespace RedstoneSimulator.Tests.Components
{
    public class RedstoneWireTests
    {
        [Fact]
        public void NewWire_HasZeroPowerLevel()
        {
            // Arrange & Act
            var wire = new RedstoneWire();
            
            // Assert
            Assert.Equal(0, wire.PowerLevel);
        }

        [Fact]
        public void ToggleDirection_DisablesConnection()
        {
            // Arrange
            var wire = new RedstoneWire();
            
            // Act
            wire.ToggleDirection(Direction.North);
            
            // Assert
            Assert.False(wire.IsDirectionEnabled(Direction.North));
        }

        [Fact]
        public void ToggleDirection_TwiceRestoresConnection()
        {
            // Arrange
            var wire = new RedstoneWire();
            
            // Act
            wire.ToggleDirection(Direction.East);
            wire.ToggleDirection(Direction.East);
            
            // Assert
            Assert.True(wire.IsDirectionEnabled(Direction.East));
        }

        [Fact]
        public void UpdateState_TakesHighestNeighborPowerLevelMinusOne()
        {
            // Arrange
            var wire = new RedstoneWire();
            
            var mockComponentNorth = new Mock<IComponent>();
            mockComponentNorth.Setup(c => c.GetPowerOutput(Direction.South)).Returns(15);
            
            var mockComponentEast = new Mock<IComponent>();
            mockComponentEast.Setup(c => c.GetPowerOutput(Direction.West)).Returns(10);
            
            var neighbors = new Dictionary<Direction, IComponent>
            {
                { Direction.North, mockComponentNorth.Object },
                { Direction.East, mockComponentEast.Object }
            };
            
            // Act
            wire.UpdateState(neighbors);
            
            // Assert
            Assert.Equal(14, wire.PowerLevel); // 15 - 1
        }
    }
}