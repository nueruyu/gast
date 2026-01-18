using Gast.Domain.Characters;
using Gast.Features.Characters;
using Gast.Features.Combat;
using Gast.Features.AI;
using Gast.Infrastructure.Settings;
using System;
using System.Collections.Generic;

namespace Gast.Infrastructure.Factories
{
    public class CombatMethodFactory : ICombatMethodFactory
    {
        readonly CombatFeedbackService combatFeedbackService;

        public CombatMethodFactory(CombatFeedbackService combatFeedbackService)
        {
            this.combatFeedbackService = combatFeedbackService;
        }

        public ICombatMethod CreateMethod(CombatMethodSettings methodSettings)
        {
            var context = new CombatContext(
                combatFeedbackService);

            return methodSettings.CreateMethod(context);
        }
    }
}