using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using GastGame.Domain.Characters;
using System;
using System.Linq;
using UnityEngine;

namespace GastGame.Features.AI.Strategic.Actions
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
                .Select(c => c.As<IGameCharacter>())
                .Where(a => a.IsThreatTo(self))
                .OrderBy(a => Vector3.Distance(self.VisionSensor.EyePosition, a.Body.Position))
                .FirstOrDefault();

            ctx.Memory.CombatTarget = closestThreat;
            return UniTask.CompletedTask;
        }
    }
}