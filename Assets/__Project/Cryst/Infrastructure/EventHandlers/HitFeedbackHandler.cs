using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Events;
using Gast.Core.Tasks;
using Gast.Domain.Combat;
using Gast.Features.Combat;
using R3;
using Cryst.Domain.Combat;
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
            var point = e.HitPoint;

            switch (e.Effect)
            {
                case IAttackEffect:
                    feedbackService.PlayHitEffect(
                        point.position,
                        point.rotation,
                        settings.HitVfxPrefab);

                    feedbackService.PlaySound(
                        point.position,
                        settings.HitSfx,
                        settings.SfxVolume);
                    break;
            }
        }
    }
}