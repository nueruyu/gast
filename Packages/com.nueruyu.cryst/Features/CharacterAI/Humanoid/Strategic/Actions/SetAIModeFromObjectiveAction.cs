using System.Threading;
using Cysharp.Threading.Tasks;
using Cryst.Domain.AI.Objectives;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Strategic.Actions
{
    /// <summary>
    /// Sets the AI mode based on the type of the current objective in memory.
    /// </summary>
    public class SetAIModeFromObjectiveAction : IAction<ActorContext<StrategicState>, StrategicState>
    {
        public bool IsAvailable(StrategicState worldState) => true;

        public void Simulate(StrategicState worldState) { }

        public UniTask ExecuteAsync(ActorContext<StrategicState> context, CancellationToken cancellationToken)
        {
            var memory = context.GetModule<HumanoidMemory>();

            memory.CurrentMode = memory.CurrentObjective switch
            {
                DefeatCharacterObjective => AIMode.Combat,
                AcquireItemObjective => AIMode.Gathering,
                _ => AIMode.Idle
            };

            return UniTask.CompletedTask;
        }
    }
}
