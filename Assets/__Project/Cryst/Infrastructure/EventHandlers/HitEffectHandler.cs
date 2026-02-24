using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Events;
using Gast.Core.Tasks;
using Gast.Domain.Characters;
using Cryst.Domain.Characters;
using R3;
using Gast.Domain.Loot;
using Cryst.Domain.Combat;
using System.Threading;

namespace Cryst.Infrastructure.EventHandlers
{
    public class HitEffectHandler : ILifecycleTask
    {
        readonly IDomainEventSubscriber eventSubscriber;
        readonly IDomainEventPublisher eventPublisher;
        readonly ICharacterRepository characterRepository;
        readonly ICharacterTypeRepository typeRepository;
        readonly ICharacterBrainManager brainManager;

        public HitEffectHandler(
            IDomainEventSubscriber eventSubscriber,
            IDomainEventPublisher eventPublisher,
            ICharacterRepository characterRepository,
            ICharacterTypeRepository typeRepository,
            ICharacterBrainManager brainManager)
        {
            this.eventSubscriber = eventSubscriber;
            this.eventPublisher = eventPublisher;
            this.characterRepository = characterRepository;
            this.typeRepository = typeRepository;
            this.brainManager = brainManager;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            eventSubscriber.Subscribe<CharacterHitEvent<AttackInfo>>(OnCharacterHit)
                .AddTo(cancellationToken);
            await UniTask.WaitUntilCanceled(cancellationToken);
        }

        void OnCharacterHit(CharacterHitEvent<AttackInfo> e)
        {
            var attackInfo = e.Context;

            var hitActor = e.HitCharacter.As<ICrystCharacter>();
            var attacker = characterRepository.Get(attackInfo.SourceCharacterId).As<ICrystCharacter>();

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
                eventPublisher.Publish(
                    new LootPotentialDropEvent(typeDefinition.LootTable, hitActor.Body.Position));
            }
        }
    }
}
