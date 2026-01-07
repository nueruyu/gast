using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using System;
using System.Linq;
using UnityEngine;

namespace Gast.Features.Npcs.Actions
{
    [Serializable]
    public class FindThreatAction : PrimitiveTask<StrategicWorldState>
    {
        readonly SharedAIState sharedState;

        public FindThreatAction(SharedAIState sharedState) : base("FindThreatAction")
        {
            this.sharedState = sharedState;
        }

        protected override bool CheckCondition(StrategicWorldState state) => !state.HasGoal;

        protected override void ApplyEffect(ref StrategicWorldState state, ISimulationContext context) { }

        protected override UniTask ExecuteAsync(Context<StrategicWorldState> ctx)
        {
            var self = ctx.Character;
            var closestThreat = self.VisionSensor.VisibleCharacters
                .Where(c => c != self && c.IsAlive)
                .OrderBy(e => Vector3.Distance(self.VisionSensor.EyePosition, e.Body.Position))
                .FirstOrDefault();

            sharedState.StrategicTarget = closestThreat;
            return UniTask.CompletedTask;
        }
    }
}
