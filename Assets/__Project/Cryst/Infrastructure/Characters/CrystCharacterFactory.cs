using Cryst.Domain.Characters;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Features.Characters;
using Gast.Infrastructure.Characters;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cryst.Infrastructure.Characters
{
    public class CrystCharacterFactory : ICharacterFactory
    {
        readonly CharacterTypeRepository typeRepository;
        readonly ICharacterFacetFactoryRegistry facetFactoryRegistry;
        readonly IEnumerable<ICharacterContextInitializer> contextInitializers;
        readonly ICharacterActionFactory actionFactory;

        public Type ParametersType => typeof(CharacterCreationParameters);

        public CrystCharacterFactory(
            CharacterTypeRepository typeRepository,
            ICharacterFacetFactoryRegistry facetFactoryRegistry,
            IEnumerable<ICharacterContextInitializer> contextInitializers,
            ICharacterActionFactory actionFactory)
        {
            this.typeRepository = typeRepository;
            this.facetFactoryRegistry = facetFactoryRegistry;
            this.contextInitializers = contextInitializers;
            this.actionFactory = actionFactory;
        }

        public ICharacter Create(Vector3 position, Quaternion rotation, ICharacterCreationParameters parameters)
        {
            if (parameters is not CharacterCreationParameters creationParams)
            {
                throw new ArgumentException($"Invalid parameter type. Expected {nameof(CharacterCreationParameters)}.", nameof(parameters));
            }

            var definition = typeRepository.Get(creationParams.TypeId);
            var characterPrefab = definition.CharacterPrefab;

            var characterId = CharacterId.New();
            var character = UnityEngine.Object.Instantiate(characterPrefab, position, rotation);
            character.name = $"{definition.name}@{characterPrefab.name}:{characterId}";

            var visual = UnityEngine.Object.Instantiate(definition.VisualPrefab, character.transform);
            visual.name = $"Visual ({definition.VisualPrefab.name})";

            var context = new CharacterContext(
                characterId,
                definition,
                character.gameObject,
                character.destroyCancellationToken);

            foreach (var extension in definition.Extensions)
                context.Register(extension.GetType(), extension);

            foreach (var initializer in contextInitializers)
                initializer.Initialize(context);

            var wallet = new Wallet(definition.InitialMoney);
            var inventory = new Inventory(definition.SlotCapacity);

            var actionController = CreateActionController(context, definition);
            character.Initialize(
                context,
                definition,
                actionController,
                creationParams.Faction,
                wallet,
                inventory,
                facetFactoryRegistry);

            return character;
        }

        CharacterActionController CreateActionController(CharacterContext character, CharacterTypeDefinition definition)
        {
            var actionController = new CharacterActionController();

            foreach (var settings in definition.ActionSettings)
            {
                var action = actionFactory.Create(settings, character);
                if (action is ICharacterExecutableAction executable)
                {
                    actionController.RegisterAction(executable);
                }
                else
                {
                    actionController.RegisterDefaultAction(action);
                }
            }

            return actionController;
        }
    }
}
