using Cryst.Domain.Characters.Facets;

namespace Cryst.Features.CharacterAI.Humanoid.Combat
{
    public class CombatWorldStateUpdater : IWorldStateUpdater<CombatState>
    {
        public void Update(ActorContext<CombatState> context)
        {
            var aiModeMemory = context.GetModule<AIModeMemory>();
            var combatMemory = context.GetModule<CombatMemory>();
            var character = context.Character;
            var actor = context.Actor;

            var combatTarget = combatMemory.ThreatTarget ?? combatMemory.ObjectiveCombatTarget;

            var isReadyToAttack = character.Is(out AttackableCharacter attackable) && attackable.CanAttack();
            var canGuard = character.Is(out GuardableCharacter guardable) && guardable.CanGuard();

            var currentHealth = actor.Status.Health.Value;
            var maxHealth = actor.Status.MaxHealth.Value;
            var selfHealthRatio = maxHealth > 0 ? currentHealth / maxHealth : 1f;

            context.WorldState.Update(
                isActive: aiModeMemory.CurrentMode == AIMode.Combat && combatTarget != null,
                combatTarget: combatTarget,
                actorPosition: actor.Body.Position,
                isReadyToAttack: isReadyToAttack,
                canGuard: canGuard,
                selfHealthRatio: selfHealthRatio);
        }
    }
}
