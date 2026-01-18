using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;
using System.Linq;
using UnityEngine;

namespace Gast.Features.AI.Strategic.Actions
{
    [Serializable]
    public class SelectThreatAction : IAction<StrategicWorldState, AIContext<StrategicWorldState>>
    {
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

            ctx.Memory.CombatTarget = closestThreat;
            return UniTask.CompletedTask;
        }
    }
}