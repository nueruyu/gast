using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Events;
using Gast.Core.Tasks;
using Gast.Domain.Combat;
using Gast.Features.Combat;
using Cryst.Modules.CharacterActions;
using R3;

namespace Cryst.Application.EventHandlers
{
    public class CharacterDamageFeedbackHandler : ILifecycleTask
    {
        readonly IDomainEventSubscriber eventSubscriber;
        readonly CombatFeedbackService feedbackService;
        readonly MeleeAttackEffectSettings effectSettings;

        public CharacterDamageFeedbackHandler(
            IDomainEventSubscriber eventSubscriber,
            CombatFeedbackService feedbackService,
            MeleeAttackEffectSettings effectSettings)
        {
            this.eventSubscriber = eventSubscriber;
            this.feedbackService = feedbackService;
            this.effectSettings = effectSettings;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            eventSubscriber.Subscribe<CharacterDamagedEvent>(OnCharacterDamaged)
                .AddTo(cancellationToken);
            await UniTask.WaitUntilCanceled(cancellationToken);
        }

        void OnCharacterDamaged(CharacterDamagedEvent e)
        {
            var point = e.HitPoint;

            feedbackService.PlayHitEffect(
                point.position,
                point.rotation,
                effectSettings.HitVfxPrefab);

            feedbackService.PlaySound(
                point.position,
                effectSettings.HitSfx,
                effectSettings.SfxVolume);
        }
    }
}
