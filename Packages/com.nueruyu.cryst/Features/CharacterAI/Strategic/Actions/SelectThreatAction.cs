using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using Cryst.Domain.Characters;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using Gast.Domain.Characters;

namespace Cryst.Features.CharacterAI.Strategic.Actions
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
