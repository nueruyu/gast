using Gast.Core.Events;
using Gast.Features.Combat;

namespace Gast.Infrastructure.Factories
{
    public class CombatMethodFactory : ICombatMethodFactory
    {
        readonly CombatFeedbackService combatFeedbackService;
        readonly IDomainEventPublisher eventPublisher;

        public CombatMethodFactory(
            CombatFeedbackService combatFeedbackService,
            IDomainEventPublisher eventPublisher)
        {
            this.combatFeedbackService = combatFeedbackService;
            this.eventPublisher = eventPublisher;
        }

        public ICombatMethod CreateMethod(CombatMethodSettings methodSettings)
        {
            var context = new CombatContext(
                combatFeedbackService,
                eventPublisher);

            return methodSettings.CreateMethod(context);
        }
    }
}