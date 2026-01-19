using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;
using System.Linq;
using UnityEngine;

namespace Gast.Features.AI.Strategic.Actions
{
    [Serializable]
    public class SelectThreatAction : IAction<StrategicState, AIContext<StrategicState>>
    {
        public bool CanExecute(StrategicState worldState) => worldState.IsThreatened;

        public void Simulate(StrategicState worldState)
        {
        }

        public UniTask ExecuteAsync(AIContext<StrategicState> ctx)
        {
            var self = ctx.Actor;
            var closestThreat = self.VisionSensor.VisibleCharacters
                .Where(c => c.IsAlive)
                .Where(c => c.Status.Faction != self.Status.Faction)
                .OrderBy(e => Vector3.Distance(self.VisionSensor.EyePosition, e.Body.Position))
                .FirstOrDefault();

            ctx.Memory.CombatTarget = closestThreat;
            //return UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: ctx.CancellationToken);
            return UniTask.CompletedTask;
        }
    }
}