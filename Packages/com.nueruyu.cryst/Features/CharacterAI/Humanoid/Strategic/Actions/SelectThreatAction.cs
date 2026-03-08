using System;
using System.Linq;
using System.Threading;
using Cryst.Domain.Characters;
using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Strategic.Actions
{
    [Serializable]
    public class SelectThreatAction : IAction<ActorContext<StrategicState>, StrategicState>
    {
        public bool CanExecute(StrategicState worldState) => worldState.IsThreatened;

        public void Simulate(StrategicState worldState)
        {
        }

        public UniTask ExecuteAsync(ActorContext<StrategicState> context, CancellationToken cancellationToken)
        {
            var self = context.Actor;
            var closestThreat = self.VisionSensor.VisibleCharacters
                .Select(c => c.As<BaseCharacter>())
                .Where(a => a.IsThreatTo(self))
                .OrderBy(a => Vector3.Distance(self.VisionSensor.EyePosition, a.Body.Position))
                .FirstOrDefault();

            context.Memory.CombatTarget = closestThreat;
            return UniTask.CompletedTask;
        }
    }
}
