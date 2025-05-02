using RedstoneSimulator.Core.Common;

namespace RedstoneSimulator.Core.Components
{
    public abstract class ComponentBase : IComponent
    {
        public int PowerLevel { get; protected set; }
        public Direction Orientation { get; set; }
        public abstract IReadOnlyList<Direction> ConnectedSides { get; }

        public abstract void UpdateState(IDictionary<Direction, IComponent> neighbors);
        public abstract int GetPowerOutput(Direction side);
        public abstract bool CanConnect(Direction side);
        public abstract void OnNeighborChange(Direction side);
    }
}