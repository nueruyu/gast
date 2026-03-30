using Cryst.Domain.Characters;
using Cryst.Domain.Characters.Facets;
using Cryst.Domain.Combat;
using Gast.Domain.Characters;

namespace Cryst.Features.Characters.EventHandlers
{
    public class GrappleHitHandler
    {
        readonly ICharacterRepository characterRepository;

        public GrappleHitHandler(ICharacterRepository characterRepository)
        {
            this.characterRepository = characterRepository;
        }

        public void Handle(CharacterHitEvent<GrappleAttackInfo> e)
        {
            var attacker = characterRepository.Get(e.Context.AttackerId);
            var victim = e.HitCharacter;

            if (attacker == victim) return;

            // No friendly-fire grapples
            if (attacker.Is(out BaseCharacter attackerBase) && victim.Is(out BaseCharacter victimBase))
            {
                if (attackerBase.Faction == victimBase.Faction) return;
            }

            if (!victim.Is(out GrappleTargetableCharacter targetable)) return;

            if (attacker.Is(out GrappleableCharacter grappleable))
                grappleable.StartThrow(victim);

            targetable.GetGrappledBy(attacker);
        }
    }
}
