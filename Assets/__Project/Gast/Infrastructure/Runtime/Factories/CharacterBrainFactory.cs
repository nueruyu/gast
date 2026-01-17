using Gast.Domain.Characters;
using Gast.Features.Npcs;
using Gast.Features.Npcs.Actions;
using Gast.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using VContainer;

namespace Gast.Infrastructure.Factories
{
    public class CharacterBrainFactory : ICharacterBrainFactory
    {
        readonly CharacterBrainFactorySettings settings;
        readonly IObjectResolver resolver;
        readonly Dictionary<CharacterTypeId, Func<ICharacterBrain>> factoryMap = new();

        public CharacterBrainFactory(CharacterBrainFactorySettings settings, IObjectResolver resolver)
        {
            this.settings = settings;
            this.resolver = resolver;

            foreach (var typeRef in settings.SoldierBrainTypes)
            {
                factoryMap.Add(typeRef.Id, CreateSoldierBrain);
            }
        }

        public ICharacterBrain Create(CharacterTypeId typeId)
        {
            if (!factoryMap.TryGetValue(typeId, out var factory))
                throw new KeyNotFoundException($"Key '{typeId}' not found");

            return factory();
        }

        ICharacterBrain CreateSoldierBrain()
        {
            var goalManager = resolver.Resolve<GoalManager>();

            var findTargetForGoalAction = resolver.Resolve<FindTargetForGoalAction>();
            var selectThreatAction = resolver.Resolve<SelectThreatAction>();
            var clearTargetAction = resolver.Resolve<ClearTargetAction>();

            var findItemPickupAction = resolver.Resolve<FindItemPickupAction>();
            var moveToInteractableAction = resolver.Resolve<MoveToInteractableAction>();
            var interactWithTargetAction = resolver.Resolve<InteractWithTargetAction>();
            var clearInteractableTargetAction = resolver.Resolve<ClearInteractableTargetAction>();

            var strategicDomain = StrategicDomain.Create(
                findTargetForGoalAction,
                selectThreatAction,
                clearTargetAction,
                findItemPickupAction,
                moveToInteractableAction,
                interactWithTargetAction,
                clearInteractableTargetAction
            );
            var combatDomain = CombatDomain.Create(settings.SoldierBrainSettings);

            return new SoldierBrain(
                strategicDomain,
                combatDomain,
                goalManager);
        }
    }
}