using Gast.Domain.Characters;
using Gast.Features.Npcs;
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
            return new SoldierBrain(settings.SoldierBrainSettings, goalManager);
        }
    }
}