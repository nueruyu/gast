using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.Lib.AI.Testing
{
    /// <summary>
    /// Provides functionality to run the AI decision-making process without executing side effects.
    /// </summary>
    public class AISimulator<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        readonly ITask<TActorContext, TWorldState> rootTask;
        TWorldState simulationState;

        public AISimulator(AIDomain<TActorContext, TWorldState> domain)
        {
            rootTask = domain.RootTask;
            if (rootTask == null)
            {
                throw new System.ArgumentException("The domain must have a root task.", nameof(domain));
            }
        }

        /// <summary>
        /// Executes a single planning and simulation cycle.
        /// </summary>
        /// <param name="initialState">The starting world state for the simulation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The result of the simulation, or null if no valid plan could be found.</returns>
        public async UniTask<SimulationResult<TWorldState>> SimulateAsync(
            TWorldState initialState,
            CancellationToken cancellationToken = default)
        {
            initialState.WriteTo(ref simulationState);

            var context = new SimulationContext<TWorldState>(simulationState);

            await rootTask.SimulateAsync(context, cancellationToken);

            if (!context.PlanFound)
            {
                return null;
            }

            return new SimulationResult<TWorldState>(
                context.RootMethodName,
                context.SimulatedPlan,
                context.WorldState
            );
        }
    }
}
