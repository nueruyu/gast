using Cryst.Domain.Characters.Facets;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Combat
{
    public class CombatWorldStateUpdater : IWorldStateUpdater<CombatState>
    {
        public void Update(ActorContext<CombatState> context)
        {
            var memory = context.Memory;
            var character = context.Character;
            var actor = context.Actor;
            var state = context.WorldState;

            state.AttackRange = 1.5f;
            state.CombatRange = 4.5f;

            if (memory.HasTarget)
            {
                var target = memory.CombatTarget;
                state.HasTarget = true;
                state.TargetPosition = target.Body.Position;
                state.TargetForward = target.Body.Forward;
                state.DistanceToTarget = Vector3.Distance(actor.Body.Position,
                    target.Body.Position);
            }
            else
            {
                state.HasTarget = false;
                state.DistanceToTarget = float.PositiveInfinity;
            }

            state.IsReadyToAttack = character.Is(out AttackableCharacter attackable) && attackable.CanAttack();
            state.CanGuard = character.Is(out GuardableCharacter guardable) && guardable.CanGuard();

            var currentHealth = actor.Status.Health.Value;
            var maxHealth = actor.Status.MaxHealth.Value;
            state.SelfHealthRatio = maxHealth > 0 ? currentHealth / maxHealth : 1f;
        }
    }
}
