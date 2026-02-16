using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Events;
using Gast.Core.Tasks;
using Gast.Domain.Characters;
using Gast.Domain.Combat;
using Cryst.Domain.Characters;
using R3;
using Cryst.Domain.Combat;

namespace Cryst.Application.EventHandlers
{
    public class DamageApplicationHandler : ILifecycleTask
    {
        readonly IDomainEventSubscriber eventSubscriber;
        readonly ICharacterRepository characterRepository;

        public DamageApplicationHandler(
            IDomainEventSubscriber eventSubscriber,
            ICharacterRepository characterRepository)
        {
            this.eventSubscriber = eventSubscriber;
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
            var attacker = characterRepository.Get(e.AttackInfo.AttackerId).As<ICrystCharacter>();

            var damageInfo = new DamageInfo(
                e.AttackInfo.Damage,
                e.HitPoint,
                e.AttackInfo.KnockbackForce,
                e.AttackInfo.AttackerId
            );

            hitActor.Hit(attacker, damageInfo);
        }
    }
}