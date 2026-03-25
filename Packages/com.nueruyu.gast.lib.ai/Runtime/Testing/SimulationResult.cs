using System.Collections.Generic;

namespace Gast.Lib.AI.Testing
{
    /// <summary>
    /// Represents the result of a single simulation cycle of the AI.
    /// </summary>
    public class SimulationResult<TWorldState> where TWorldState : class
    {
        /// <summary>
        /// The name of the method selected by the AI's root task.
        /// </summary>
        public string SelectedMethodName { get; }

        /// <summary>
        /// The sequence of primitive tasks that would have been executed.
        /// </summary>
        public IReadOnlyList<ITask> SimulatedTaskSequence { get; }

        /// <summary>
        /// The final state of the world after simulating the entire method.
        /// </summary>
        public TWorldState FinalWorldState { get; }

        public SimulationResult(
            string selectedMethodName,
            IReadOnlyList<ITask> simulatedTaskSequence,
            TWorldState finalWorldState)
        {
            SelectedMethodName = selectedMethodName;
            SimulatedTaskSequence = simulatedTaskSequence;
            FinalWorldState = finalWorldState;
        }
    }
}
