using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using System;
using System.Linq;
using UnityEngine;

namespace Gast.Features.Npcs.Actions
{
    [Serializable]
    public class SelectThreatAction : PrimitiveTask<StrategicWorldState>
    {
        readonly SharedAIState sharedState;

        public SelectThreatAction(SharedAIState sharedState) : base("SelectThreatAction")
        {
            this.sharedState = sharedState;
        }

        protected override bool CheckCondition(StrategicWorldState state) => state.IsThreatened;

        protected override void ApplyEffect(ref StrategicWorldState state, ISimulationContext context)
        {
        }

        protected override UniTask ExecuteAsync(Context<StrategicWorldState> ctx)
        {
            var self = ctx.Character;
            var closestThreat = self.VisionSensor.VisibleCharacters
                .Where(c => c.IsAlive)
                .Where(c => c.Status.Faction != self.Status.Faction)
                .OrderBy(e => Vector3.Distance(self.VisionSensor.EyePosition, e.Body.Position))
                .FirstOrDefault();

            sharedState.CombatTarget = closestThreat;
            return UniTask.CompletedTask;
        }
    }
}