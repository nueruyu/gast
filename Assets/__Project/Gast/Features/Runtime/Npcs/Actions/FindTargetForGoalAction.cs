using Cysharp.Threading.Tasks;
using Gast.Api.AI.Goals;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using System;
using System.Linq;
using UnityEngine;

namespace Gast.Features.Npcs.Actions
{
    [Serializable]
    public class FindTargetForGoalAction : PrimitiveTask<StrategicWorldState>
    {
        readonly SharedAIState sharedState;

        public FindTargetForGoalAction(SharedAIState sharedState) : base("FindTargetForGoalAction")
        {
            this.sharedState = sharedState;
        }

        protected override bool CheckCondition(StrategicWorldState state) => state.HasGoal;

        protected override void ApplyEffect(ref StrategicWorldState state, ISimulationContext context) { }

        protected override UniTask ExecuteAsync(Context<StrategicWorldState> ctx)
        {
            var goal = ctx.CurrentState.CurrentGoal;
            ICharacter foundTarget = null;

            if (goal is DefeatCharacterGoal defeatGoal)
            {
                foundTarget = FindClosestCharacterOfType(ctx.Character, defeatGoal.TargetTypeId);
            }

            sharedState.StrategicTarget = foundTarget;
            return UniTask.CompletedTask;
        }

        private ICharacter FindClosestCharacterOfType(ICharacter self, CharacterTypeId typeId)
        {
            return self.VisionSensor.VisibleCharacters
                .Where(c => c.TypeId == typeId && c.IsAlive)
                .OrderBy(c => Vector3.Distance(self.Body.Position, c.Body.Position))
                .FirstOrDefault();
        }
    }
}
