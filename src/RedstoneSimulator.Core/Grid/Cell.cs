using RedstoneSimulator.Core.Components;

namespace RedstoneSimulator.Core.Grid
{
    public class Cell
    {
        public int X { get; }
        public int Y { get; }
        public IComponent? Component { get; private set; }

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool IsEmpty() => Component == null;

        public IComponent? GetComponent() => Component;

        public void SetComponent(IComponent? component)
        {
            Component = component;
        }
    }
}