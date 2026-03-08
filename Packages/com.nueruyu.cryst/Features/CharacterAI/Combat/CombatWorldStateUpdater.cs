using Cryst.Domain.Characters.Facets;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Combat
{
    public class CombatWorldStateUpdater : IWorldStateUpdater<CombatState>
    {
        public void Update(ActorContext<CombatState> context)
        {
            var memory = context.Memory;
            var character = context.Character;
            var actor = context.Actor;
            var combatState = context.WorldState;

            combatState.AttackRange = 1.5f;
            combatState.CombatRange = 4.5f;

            if (memory.HasTarget)
            {
                var target = memory.CombatTarget;
                combatState.HasTarget = true;
                combatState.TargetPosition = target.Body.Position;
                combatState.TargetForward = target.Body.Forward;
                combatState.DistanceToTarget = Vector3.Distance(actor.Body.Position,
                    target.Body.Position);
            }
            else
            {
                combatState.HasTarget = false;
                combatState.DistanceToTarget = float.PositiveInfinity;
            }

            combatState.IsReadyToAttack = character.Is(out AttackableCharacter attackable) && attackable.CanAttack();
            combatState.CanGuard = character.Is(out GuardableCharacter guardable) && guardable.CanGuard();

            var currentHealth = actor.Status.Health.Value;
            var maxHealth = actor.Status.MaxHealth.Value;
            combatState.SelfHealthRatio = maxHealth > 0 ? currentHealth / maxHealth : 1f;
        }
    }
}
