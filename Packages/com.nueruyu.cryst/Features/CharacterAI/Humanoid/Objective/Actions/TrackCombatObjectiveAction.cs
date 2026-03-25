using System.Threading;
using Cryst.Domain.AI.Objectives;
using Cryst.Domain.Characters;
using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;

namespace Cryst.Features.CharacterAI.Humanoid.Objective.Actions
{
    public class TrackCombatObjectiveAction : TrackObjectiveAction<DefeatCharacterObjective>
    {
        public TrackCombatObjectiveAction(DefeatCharacterObjective objective) : base(objective) { }

        public override async UniTask ExecuteAsync(ActorContext<ObjectiveState> context, CancellationToken cancellationToken)
        {
            var memory = context.GetModule<CombatMemory>();

            while (!cancellationToken.IsCancellationRequested)
            {
                if (!memory.HasObjectiveCombatTarget)
                {
                    var targetInfo = context.ObjectiveQueries.FindBestTargetFor(objective, context.WorldState);
                    if (targetInfo.HasValue)
                    {
                        var targetCharacter = context.CharacterRepository.Get(targetInfo.Value.Id).As<BaseCharacter>();
                        if (targetCharacter != null)
                            memory.SetObjective(objective, targetCharacter);
                    }
                }

                await UniTask.Yield(cancellationToken);
            }
        }
    }
}
