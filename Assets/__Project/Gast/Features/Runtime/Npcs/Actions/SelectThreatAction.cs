using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using System;
using System.Linq;
using UnityEngine;

namespace Gast.Features.Npcs.Actions
{
    [Serializable]
    public class SelectThreatAction : IAction<StrategicWorldState, AIContext<StrategicWorldState>>
    {
        readonly SharedAIState sharedState;

        public SelectThreatAction(SharedAIState sharedState)
        {
            this.sharedState = sharedState;
        }

        public string Name => "SelectThreatAction";

        public bool CanExecute(StrategicWorldState worldState) => worldState.IsThreatened;

        public void Simulate(StrategicWorldState worldState)
        {
        }

        public UniTask ExecuteAsync(AIContext<StrategicWorldState> ctx)
        {
            var self = ctx.Actor;
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
