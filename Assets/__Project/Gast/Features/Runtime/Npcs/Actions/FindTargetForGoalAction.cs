using Cysharp.Threading.Tasks;
using Gast.Api.AI.Goals;
using Gast.Domain.Characters;
using Gast.Lib.AI.Tasks;
using System;
using System.Linq;
using UnityEngine;

namespace Gast.Features.Npcs.Actions
{
    [Serializable]
    public class FindTargetForGoalAction : PrimitiveTask<StrategicWorldState, AIContext<StrategicWorldState>>
    {
        readonly ICharacterRepository characterRepository;
        readonly SharedAIState sharedState;

        public FindTargetForGoalAction(
            ICharacterRepository characterRepository,
            SharedAIState sharedState) : base("FindTargetForGoalAction")
        {
            this.characterRepository = characterRepository;
            this.sharedState = sharedState;
        }

        protected override bool CanExecute(StrategicWorldState worldState) => worldState.HasGoal;

        protected override void Simulate(StrategicWorldState worldState)
        {
        }

        protected override UniTask ExecuteAsync(AIContext<StrategicWorldState> ctx)
        {
            var goal = ctx.WorldState.CurrentGoal;
            ICharacter foundTarget = null;

            if (goal is DefeatCharacterGoal defeatGoal)
            {
                foundTarget = FindClosestCharacterOfType(ctx.Actor, defeatGoal.TargetTypeId);
            }

            sharedState.CombatTarget = foundTarget;
            return UniTask.CompletedTask;
        }

        private ICharacter FindClosestCharacterOfType(ICharacter self, CharacterTypeId typeId)
        {
            return characterRepository.GetAll()
                .Where(c => c.TypeId == typeId && c.IsAlive)
                .Where(c => c.Status.Faction != self.Status.Faction)
                .OrderBy(c => Vector3.Distance(self.Body.Position, c.Body.Position))
                .FirstOrDefault();
        }
    }
}
