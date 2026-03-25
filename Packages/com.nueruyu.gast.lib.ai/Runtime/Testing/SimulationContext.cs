using System.Collections.Generic;

namespace Gast.Lib.AI.Testing
{
    /// <summary>
    /// Collects information during a simulation execution.
    /// </summary>
    public class SimulationContext<TWorldState> where TWorldState : class
    {
        /// <summary>
        /// The world state being simulated. This instance is mutated during the simulation.
        /// </summary>
        public TWorldState WorldState { get; }

        /// <summary>
        /// The sequence of primitive tasks that would have been executed.
        /// </summary>
        public List<ITask> SimulatedPlan { get; } = new List<ITask>();

        /// <summary>
        /// The name of the method chosen by the top-level CompoundTask.
        /// </summary>
        public string RootMethodName { get; set; }

        /// <summary>
        /// Indicates whether a valid plan was found.
        /// </summary>
        public bool PlanFound => RootMethodName != null;

        public SimulationContext(TWorldState worldState)
        {
            WorldState = worldState;
        }
    }
}
