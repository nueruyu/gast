using DescrioGames.Domain.Characters;
using DescrioGames.Features.Npcs;
using DescrioGames.Infrastructure.Settings;
using System;
using System.Collections.Generic;

namespace DescrioGames.Infrastructure.Factories
{
    public class CharacterBrainFactory : ICharacterBrainFactory
    {
        readonly CharacterBrainFactorySettings settings;
        readonly Dictionary<CharacterTypeId, Func<ICharacterBrain>> factoryMap = new();

        public CharacterBrainFactory(CharacterBrainFactorySettings settings)
        {
            this.settings = settings;

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
            return new SoldierBrain(settings.SoldierBrainSettings);
        }
    }
}