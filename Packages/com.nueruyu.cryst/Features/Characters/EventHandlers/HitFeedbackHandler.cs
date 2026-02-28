using Gast.Core.Events;
using Gast.Domain.Characters;
using Cryst.Domain.Combat;
using Cryst.Features.Characters.Feedbacks;

namespace Cryst.Features.Characters.EventHandlers
{
    public class HitFeedbackHandler
    {
        readonly CharacterFeedbackService feedbackService;
        readonly HitFeedbackSettings settings;

        public HitFeedbackHandler(
            CharacterFeedbackService feedbackService,
            HitFeedbackSettings settings)
        {
            this.feedbackService = feedbackService;
            this.settings = settings;
        }

        public void Handle(CharacterHitEvent<AttackInfo> e)
        {
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