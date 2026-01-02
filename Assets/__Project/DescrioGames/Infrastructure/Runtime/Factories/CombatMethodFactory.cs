using DescrioGames.Domain.Characters;
using DescrioGames.Features.Characters;
using DescrioGames.Features.Combat;
using DescrioGames.Features.Npcs;
using DescrioGames.Infrastructure.Settings;
using System;
using System.Collections.Generic;

namespace DescrioGames.Infrastructure.Factories
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