using System.Linq;
using Cryst.Domain.AI.Objectives;
using Cryst.Domain.Characters;
using Gast.Domain.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid
{
    public class CombatMemory
    {
        DefeatCharacterObjective CurrentObjective { get; set; }
        public BaseCharacter ObjectiveCombatTarget { get; private set; }
        public BaseCharacter ThreatTarget { get; private set; }

        public bool IsThreatened => ThreatTarget != null && ThreatTarget.Status.IsAlive.Value;
        public bool HasObjectiveCombatTarget => ObjectiveCombatTarget != null && ObjectiveCombatTarget.Status.IsAlive.Value;

        public void SetObjective(DefeatCharacterObjective objective, BaseCharacter target)
        {
            CurrentObjective = objective;
            ObjectiveCombatTarget = target;
        }

        public void Update(BaseCharacter actor)
        {
            ThreatTarget = actor.VisionSensor.VisibleCharacters
                .Select(c => c.As<BaseCharacter>())
                .Where(a => a.IsThreatTo(actor))
                .OrderBy(a => Vector3.Distance(actor.VisionSensor.EyePosition, a.Body.Position))
                .FirstOrDefault();

            if (CurrentObjective?.IsCompleted.Value == true)
            {
                CurrentObjective = null;
                ObjectiveCombatTarget = null;
            }
        }
    }
}
