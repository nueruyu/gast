using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Features.Characters;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gast.Infrastructure.Characters
{
    public class CharacterFactory : ICharacterFactory
    {
        readonly CharacterTypeRepository typeRepository;

        readonly ICharacterFacetFactoryRegistry facetFactoryRegistry;
        readonly IEnumerable<ICharacterContextInitializer> contextInitializers;
        readonly ICharacterActionFactory actionFactory;

        public CharacterFactory(
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

        public ICharacter Create(CharacterTypeId typeId, Vector3 position, Quaternion rotation, Faction faction)
        {
            var definition = typeRepository.Get(typeId);

            var characterPrefab = definition.CharacterPrefab;

            var characterId = CharacterId.New();
            var character = UnityEngine.Object.Instantiate(characterPrefab, position, rotation);
            character.name = $"{definition.name}@{characterPrefab.name}:{characterId}";

            var visual = UnityEngine.Object.Instantiate(definition.VisualPrefab, character.transform);
            visual.name = $"Visual ({definition.VisualPrefab.name})";

            var context = new CharacterContext(
                characterId,
                definition.TypeId,
                definition,
                character.gameObject,
                character.destroyCancellationToken);

            foreach (var extension in definition.Extensions)
                context.Register(extension.GetType(), extension);

            foreach (var initializer in contextInitializers)
                initializer.Initialize(context);

            var status = new CharacterStatus();
            definition.StatSchema.Initialize(status);

            var wallet = new Wallet(definition.InitialMoney);
            var inventory = new Inventory(definition.SlotCapacity);

            var actionController = CreateActionController(context, definition);
            character.Initialize(
                context,
                definition,
                actionController,
                status,
                faction,
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