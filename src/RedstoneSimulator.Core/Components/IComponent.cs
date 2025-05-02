using RedstoneSimulator.Core.Common;

namespace RedstoneSimulator.Core.Components
{
    public interface IComponent
    {
        int PowerLevel { get; }
        Direction Orientation { get; set; }
        IReadOnlyList<Direction> ConnectedSides { get; }
        
        void UpdateState(IDictionary<Direction, IComponent> neighbors);
        int GetPowerOutput(Direction side);
        bool CanConnect(Direction side);
        void OnNeighborChange(Direction side);
    }
}