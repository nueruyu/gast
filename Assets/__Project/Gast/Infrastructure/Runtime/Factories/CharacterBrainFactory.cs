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
            using var scope = resolver.CreateScope(builder =>
            {
                builder.Register<SharedAIState>(Lifetime.Scoped);
            });

            var goalManager = resolver.Resolve<GoalManager>();

            var findTargetForGoalAction = scope.Resolve<FindTargetForGoalAction>();
            var findThreatAction = scope.Resolve<FindThreatAction>();
            var clearTargetAction = scope.Resolve<ClearTargetAction>();
            var sharedState = scope.Resolve<SharedAIState>();

            var findItemPickupAction = scope.Resolve<FindItemPickupAction>();
            var moveToInteractableAction = scope.Resolve<MoveToInteractableAction>();
            var interactWithTargetAction = scope.Resolve<InteractWithTargetAction>();
            var clearInteractableTargetAction = scope.Resolve<ClearInteractableTargetAction>();

            var strategicDomain = StrategicDomain.Create(
                findTargetForGoalAction,
                findThreatAction,
                clearTargetAction,
                findItemPickupAction,
                moveToInteractableAction,
                interactWithTargetAction,
                clearInteractableTargetAction
            );

            return new SoldierBrain(settings.SoldierBrainSettings, goalManager, strategicDomain, sharedState);
        }
    }
}