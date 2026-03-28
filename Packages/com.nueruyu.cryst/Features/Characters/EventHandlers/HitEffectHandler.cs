using Gast.Domain.Characters;
using Cryst.Domain.Characters;
using Cryst.Domain.Combat;
using Cryst.Application.Characters;

namespace Cryst.Features.Characters.EventHandlers
{
    public class HitEffectHandler
    {
        readonly ICharacterRepository characterRepository;
        readonly ICharacterDeathService characterDeathService;

        public HitEffectHandler(
            ICharacterRepository characterRepository,
            ICharacterDeathService characterDeathService)
        {
            this.characterRepository = characterRepository;
            this.characterDeathService = characterDeathService;
        }

        public void Handle(CharacterHitEvent<AttackInfo> e)
        {
            var attackInfo = e.Context;

            var hitActor = e.HitCharacter.As<BaseCharacter>();
            var attacker = characterRepository.Get(attackInfo.SourceCharacterId).As<BaseCharacter>();

            if (hitActor.Faction == attacker.Faction)
                return;

            var damageInfo = new DamageInfo(
                attackInfo.Damage,
                e.HitPoint,
                attackInfo.KnockbackForce,
                attackInfo.SourceCharacterId
            );

            var result = hitActor.TakeDamage(damageInfo);

            if (result == TakeDamageResult.Defeated)
            {
                characterDeathService.Kill(e.HitCharacter, damageInfo.AttackerId);
            }
        }
    }
}
