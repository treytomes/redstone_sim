using RedstoneSimulator.Core.Common;
using System.Collections.Generic;
using System.Linq;

namespace RedstoneSimulator.Core.Components
{
    public class RedstoneWire : ComponentBase
    {
        private readonly Dictionary<Direction, bool> _enabledDirections;

        public RedstoneWire()
        {
            PowerLevel = 0;
            _enabledDirections = new Dictionary<Direction, bool>
            {
                { Direction.North, true },
                { Direction.East, true },
                { Direction.South, true },
                { Direction.West, true }
            };
        }

        public override IReadOnlyList<Direction> ConnectedSides => 
            _enabledDirections.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();

        public void ToggleDirection(Direction direction)
        {
            if (_enabledDirections.ContainsKey(direction))
            {
                _enabledDirections[direction] = !_enabledDirections[direction];
            }
        }

        public bool IsDirectionEnabled(Direction direction)
        {
            return _enabledDirections.TryGetValue(direction, out bool isEnabled) && isEnabled;
        }

        public override void UpdateState(IDictionary<Direction, IComponent> neighbors)
        {
            int highestPower = 0;
            
            foreach (var kvp in neighbors)
            {
                if (!IsDirectionEnabled(kvp.Key))
                    continue;
                    
                var neighborPower = kvp.Value.GetPowerOutput(kvp.Key.Opposite());
                if (neighborPower > highestPower)
                {
                    highestPower = neighborPower;
                }
            }
            
            // Decrease power by 1, unless it's already 0
            PowerLevel = highestPower > 0 ? highestPower - 1 : 0;
        }

        public override int GetPowerOutput(Direction side)
        {
            return IsDirectionEnabled(side) ? PowerLevel : 0;
        }

        public override bool CanConnect(Direction side)
        {
            return IsDirectionEnabled(side);
        }

        public override void OnNeighborChange(Direction side)
        {
            // This will be handled by the signal manager to schedule an update
        }
    }
}