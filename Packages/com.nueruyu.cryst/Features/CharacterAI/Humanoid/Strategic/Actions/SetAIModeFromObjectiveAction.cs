using System.Threading;
using Cryst.Domain.AI.Objectives;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Strategic.Actions
{
    /// <summary>
    ///     Sets the AI mode based on the type of the current objective in memory.
    /// </summary>
    public class SetAIModeFromObjectiveAction : IAction<ActorContext<StrategicState>, StrategicState>
    {
        public bool IsAvailable(StrategicState worldState)
        {
            return true;
        }

        public void Simulate(StrategicState worldState)
        {
        }

        public UniTask ExecuteAsync(ActorContext<StrategicState> context, CancellationToken cancellationToken)
        {
            var memory = context.GetModule<HumanoidMemory>();

            memory.SetMode(memory.CurrentObjective switch
            {
                DefeatCharacterObjective => AIMode.Combat,
                AcquireItemObjective => AIMode.Gathering,
                _ => AIMode.Idle
            });

            return UniTask.CompletedTask;
        }
    }
}