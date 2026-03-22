using System.Threading;
using Cryst.Domain.Characters;
using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Objective.Actions
{
    public class SelectBestCombatObjectiveAction : IAction<ActorContext<ObjectiveState>, ObjectiveState>
    {
        public bool IsAvailable(ObjectiveState worldState)
        {
            return true;
        }

        public void Simulate(ObjectiveState worldState)
        {
            // Cognitive action, no direct world state change for planning.
        }

        public UniTask ExecuteAsync(ActorContext<ObjectiveState> context, CancellationToken cancellationToken)
        {
            var (objective, targetInfo) = context.ObjectiveQueries.FindBestCombatObjective(context.WorldState);

            if (objective == null || targetInfo == null)
                return UniTask.CompletedTask;

            var memory = context.GetModule<HumanoidMemory>();
            var targetCharacter = context.CharacterRepository.Get(targetInfo.Id).As<BaseCharacter>();

            if (targetCharacter != null)
            {
                memory.SetObjective(objective);
                memory.SetCombatTarget(targetCharacter);
            }

            return UniTask.CompletedTask;
        }
    }
}