using System.Threading;
using Cryst.Domain.AI.Objectives;
using Cryst.Domain.Characters;
using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;

namespace Cryst.Features.CharacterAI.Humanoid.Objective.Actions
{
    public class SelectCombatObjectiveAction : SelectObjectiveAction<DefeatCharacterObjective>
    {
        public SelectCombatObjectiveAction(DefeatCharacterObjective objective) : base(objective) { }

        public override UniTask ExecuteAsync(ActorContext<ObjectiveState> context, CancellationToken cancellationToken)
        {
            var targetInfo = context.ObjectiveQueries.FindBestTargetFor(objective, context.WorldState);
            if (targetInfo == null)
                return UniTask.CompletedTask;

            var targetCharacter = context.CharacterRepository.Get(targetInfo.Id).As<BaseCharacter>();
            if (targetCharacter != null)
            {
                var memory = context.GetModule<CombatMemory>();
                memory.SetObjective(objective, targetCharacter);
            }

            return UniTask.CompletedTask;
        }
    }
}
