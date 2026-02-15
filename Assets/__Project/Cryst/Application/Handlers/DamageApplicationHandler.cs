using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Events;
using Gast.Core.Tasks;
using Gast.Domain.Characters;
using Gast.Domain.Combat;
using Cryst.Domain.Characters;
using R3;

namespace Cryst.Application.Handlers
{
    public class DamageApplicationHandler : ILifecycleTask
    {
        readonly IDomainEventSubscriber eventSubscriber;
        readonly IDomainEventPublisher eventPublisher;
        readonly ICharacterRepository characterRepository;

        public DamageApplicationHandler(
            IDomainEventSubscriber eventSubscriber,
            IDomainEventPublisher eventPublisher,
            ICharacterRepository characterRepository)
        {
            this.eventSubscriber = eventSubscriber;
            this.eventPublisher = eventPublisher;
            this.characterRepository = characterRepository;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            eventSubscriber.Subscribe<CharacterDamagedEvent>(OnCharacterDamaged)
                .AddTo(cancellationToken);
            await UniTask.WaitUntilCanceled(cancellationToken);
        }

        void OnCharacterDamaged(CharacterDamagedEvent e)
        {
            var hitActor = e.HitCharacter.As<ICrystCharacter>();
            var attacker = characterRepository.Get(e.AttackInfo.AttackerId);

            if (hitActor.Faction == attacker.Faction) return;

            var damageInfo = new Cryst.Domain.Combat.DamageInfo(
                e.AttackInfo.Damage,
                e.HitPoint,
                e.AttackInfo.KnockbackForce,
                e.AttackInfo.AttackerId
            );

            hitActor.Hit(damageInfo);

            if (hitActor.IsAlive.Value)
            {
                hitActor.SetHealth(hitActor.Health.Value - damageInfo.Amount);

                if (hitActor.Health.Value <= 0)
                {
                    hitActor.Die();

                    eventPublisher.Publish(new CharacterDefeatedEvent(hitActor, damageInfo.AttackerId));
                    eventPublisher.Publish(
                        new LootSpawnEvent(
                            e.HitCharacter.TypeDefinition.LootTable,
                            e.HitCharacter.Body.Position));
                }
            }
        }
    }
}
