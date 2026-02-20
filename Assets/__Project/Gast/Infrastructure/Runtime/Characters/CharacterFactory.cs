using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Features.Characters;
using Gast.Shared.UnityExtensions;
using UnityEngine;

namespace Gast.Infrastructure.Characters
{
    public class CharacterFactory : ICharacterFactory
    {
        readonly CharacterTypeRepository typeRepository;
        readonly ICharacterActorRepository characterActorRepository;

        readonly ICharacterFacetFactoryRegistry facetFactoryRegistry;
        readonly IEnumerable<ICharacterContextInitializer> contextInitializers;
        readonly ICharacterActionFactory actionFactory;

        public CharacterFactory(
            CharacterTypeRepository typeRepository,
            ICharacterActorRepository characterActorRepository,
            ICharacterFacetFactoryRegistry facetFactoryRegistry,
            IEnumerable<ICharacterContextInitializer> contextInitializers,
            ICharacterActionFactory actionFactory)
        {
            this.typeRepository = typeRepository;
            this.characterActorRepository = characterActorRepository;
            this.facetFactoryRegistry = facetFactoryRegistry;
            this.contextInitializers = contextInitializers;
            this.actionFactory = actionFactory;
        }

        public ICharacter Create(CharacterTypeId typeId, Vector3 position, Quaternion rotation, Faction faction)
        {
            var definition = typeRepository.Get(typeId);

            var characterPrefab = definition.CharacterPrefab;
            if (characterPrefab == null)
                throw new InvalidOperationException($"Character type '{typeId}' has no prefab assigned.");

            var characterId = CharacterId.New();
            var character = UnityEngine.Object.Instantiate(characterPrefab, position, rotation);
            character.name = $"{definition.name}@{characterPrefab.name}:{characterId}";

            var visual = UnityEngine.Object.Instantiate(definition.VisualPrefab, character.transform);
            visual.name = $"Visual ({definition.VisualPrefab.name})";

            var context = CreateContext(characterId, character.gameObject, definition);

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

            characterActorRepository.Register(character);

            return character;
        }

        CharacterContext CreateContext(
            CharacterId id,
            GameObject characterGo,
            CharacterTypeDefinition definition)
        {
            return new CharacterContext(
                id,
                definition.TypeId,
                definition,
                characterGo,
                characterGo.GetCancellationTokenOnDestroy());
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