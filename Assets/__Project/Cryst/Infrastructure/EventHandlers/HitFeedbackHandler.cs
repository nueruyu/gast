using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Events;
using Gast.Core.Tasks;
using Gast.Domain.Characters;
using Gast.Shared.Phantoms;
using R3;
using Cryst.Infrastructure.Feedbacks;

namespace Cryst.Infrastructure.EventHandlers
{
    public class HitFeedbackHandler : ILifecycleTask
    {
        readonly IDomainEventSubscriber eventSubscriber;
        readonly CombatFeedbackService feedbackService;
        readonly HitFeedbackSettings settings;

        public HitFeedbackHandler(
            IDomainEventSubscriber eventSubscriber,
            CombatFeedbackService feedbackService,
            HitFeedbackSettings settings)
        {
            this.eventSubscriber = eventSubscriber;
            this.feedbackService = feedbackService;
            this.settings = settings;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            eventSubscriber.Subscribe<CharacterHitEvent>(OnCharacterHit)
                .AddTo(cancellationToken);
            await UniTask.WaitUntilCanceled(cancellationToken);
        }

        void OnCharacterHit(CharacterHitEvent e)
        {
            if (e.Context is not Phantom)
                return;

            var point = e.HitPoint;

            feedbackService.PlayHitEffect(
                point.position,
                point.rotation,
                settings.HitVfxPrefab);

            feedbackService.PlaySound(
                point.position,
                settings.HitSfx,
                settings.SfxVolume);
        }
    }
}