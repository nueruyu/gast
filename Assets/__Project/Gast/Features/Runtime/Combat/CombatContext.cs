using Gast.Core.Events;

namespace Gast.Features.Combat
{
    public record CombatContext(
        CombatFeedbackService FeedbackService,
        IDomainEventPublisher EventPublisher);
}