// src\RedstoneSimulator.Core\Simulation\SimulationController.cs

using OpenTK.Mathematics;
using RedstoneSimulator.Core.Grid;

namespace RedstoneSimulator.Core.Simulation;

/// <summary>
/// Controls the simulation of a redstone circuit.
/// </summary>
public class SimulationController
{
    private readonly IGrid _grid;
    private readonly SignalPropagator _propagator;
    private bool _isRunning;
    private float _tickRate; // Ticks per second
    private float _accumulatedTime;
    
    /// <summary>
    /// Gets a value indicating whether the simulation is currently running.
    /// </summary>
    public bool IsRunning => _isRunning;
    
    /// <summary>
    /// Gets or sets the tick rate (ticks per second).
    /// </summary>
    public float TickRate
    {
        get => _tickRate;
        set => _tickRate = Math.Max(0.1f, value); // Ensure minimum tick rate
    }
    
    /// <summary>
    /// Event raised when a simulation tick is completed.
    /// </summary>
    public event EventHandler? TickCompleted;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SimulationController"/> class.
    /// </summary>
    /// <param name="grid">The grid to simulate.</param>
    public SimulationController(IGrid grid)
    {
        _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        _propagator = new SignalPropagator(grid);
        _tickRate = 10.0f; // Default to 10 ticks per second
        _isRunning = false;
        _accumulatedTime = 0;
    }
    
    /// <summary>
    /// Starts the simulation.
    /// </summary>
    public void Start()
    {
        _isRunning = true;
    }
    
    /// <summary>
    /// Pauses the simulation.
    /// </summary>
    public void Pause()
    {
        _isRunning = false;
    }
    
    /// <summary>
    /// Advances the simulation by one tick.
    /// </summary>
    public void Step()
    {
        ProcessTick();
    }
    
    /// <summary>
    /// Updates the simulation based on elapsed time.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update.</param>
    public void Update(float deltaTime)
    {
        if (!_isRunning)
        {
            return;
        }
        
        // Accumulate time
        _accumulatedTime += deltaTime;
        
        // Calculate tick interval
        float tickInterval = 1.0f / _tickRate;
        
        // Process ticks
        while (_accumulatedTime >= tickInterval)
        {
            ProcessTick();
            _accumulatedTime -= tickInterval;
        }
    }
    
    /// <summary>
    /// Processes a single simulation tick.
    /// </summary>
    private void ProcessTick()
    {
        // Update all components in the grid
        UpdateComponents();
        
        // Process updates for cells that were marked for update
        ProcessMarkedCells();
        
        // Raise tick completed event
        TickCompleted?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Updates all components in the grid.
    /// </summary>
    private void UpdateComponents()
    {
        float tickInterval = 1.0f / _tickRate;
        
        // Iterate through all cells in the grid
        foreach (var cell in _grid.GetAllCells())
        {
            if (cell.Component != null)
            {
                // Update the component with the tick interval
                cell.Component.Update(tickInterval);
            }
        }
    }
    
    /// <summary>
    /// Processes cells that were marked for update.
    /// </summary>
    private void ProcessMarkedCells()
    {
        // Get all cells that were marked for update
        var markedCells = _grid.GetMarkedCells();
        
        // Process each marked cell
        foreach (var position in markedCells)
        {
            // Propagate signals from this position
            _propagator.PropagateSignals(position);
        }
        
        // Clear marked cells
        _grid.ClearMarkedCells();
    }
}