using Gast.Domain.Characters;
using Gast.Domain.Loot;
using Cryst.Domain.Combat;
using Gast.Core.Events;

namespace Cryst.Features.Characters.EventHandlers
{
    public class HitEffectHandler
    {
        readonly IDomainEventPublisher eventPublisher;
        readonly ICharacterRepository characterRepository;
        readonly ICharacterTypeRepository typeRepository;
        readonly ICharacterBrainManager brainManager;

        public HitEffectHandler(
            IDomainEventPublisher eventPublisher,
            ICharacterRepository characterRepository,
            ICharacterTypeRepository typeRepository,
            ICharacterBrainManager brainManager)
        {
            this.eventPublisher = eventPublisher;
            this.characterRepository = characterRepository;
            this.typeRepository = typeRepository;
            this.brainManager = brainManager;
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
                brainManager.DetachBrain(hitActor.Id);

                var typeDefinition = typeRepository.Get(hitActor.TypeId);

                eventPublisher.Publish(
                    new CharacterDefeatedEvent(e.HitCharacter, damageInfo.AttackerId));

                if (typeDefinition.TryGetSettings<ILootTable>(out var lootTable))
                {
                    eventPublisher.Publish(
                        new LootPotentialDropEvent(lootTable, hitActor.Body.Position));
                }
            }
        }
    }
}