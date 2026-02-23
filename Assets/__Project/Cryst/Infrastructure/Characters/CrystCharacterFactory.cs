using Cryst.Domain.Characters;
using Cryst.Infrastructure.Economy;
using Cryst.Modules.CharacterActions;
using Gast.Core.Events;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Interactions;
using Gast.Features.Cameras;
using Gast.Features.Characters;
using Gast.Features.Navigations;
using Gast.Features.Sensors;
using Gast.Infrastructure.Characters;
using Gast.Shared.UnityExtensions;
using R3;
using R3.Triggers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cryst.Infrastructure.Characters
{
    public class CrystCharacterFactory : ICharacterFactory<CharacterCreationParameters>
    {
        readonly Func<Vector3, Quaternion, CharacterCreationParameters, ICharacter> factory;

        public CrystCharacterFactory(
            CharacterTypeRepository typeRepository,
            ICharacterActionFactory actionFactory,
            IDomainEventPublisher eventPublisher,
            ICharacterBrainManager brainManager)
        {
            factory = Create;

            ICharacter Create(Vector3 position, Quaternion rotation, CharacterCreationParameters parameters)
            {
                var definition = typeRepository.Get(parameters.TypeId);
                var characterPrefab = definition.CharacterPrefab;

                var characterId = CharacterId.New();
                var characterGo = UnityEngine.Object.Instantiate(characterPrefab, position, rotation);
                characterGo.name = $"{definition.name}@{characterPrefab.name}:{characterId}";

                var visual = UnityEngine.Object.Instantiate(definition.VisualPrefab, characterGo.transform);
                visual.name = $"Visual ({definition.VisualPrefab.name})";

                var character = new Character(characterGo);

                var context = new CharacterContext(
                    characterId,
                    definition,
                    characterGo,
                    character.CancellationToken);

                InitializeContext(context, definition);

                var actionController = CreateActionController(context, definition);

                characterGo.UpdateAsObservable().Subscribe(_ => actionController.Update());

                character.Initialize(context);

                var crystCharacter = new CrystCharacter(
                    character,
                    characterId,
                    definition,
                    parameters.Faction,
                    actionController,
                    context.Resolve<CharacterStatus>(),
                    context.Resolve<CharacterActionStateStore>(),
                    context.Resolve<ICharacterBody>(),
                    context.Resolve<IVisionSensor>(),
                    context.Resolve<INavigationProvider>(),
                    eventPublisher,
                    brainManager);

                var wallet = new Wallet(definition.InitialMoney);
                var inventory = new Inventory(definition.SlotCapacity);

                character.RegisterFacets(
                    new()
                    {
                        { typeof(ICrystCharacter), crystCharacter },
                        { typeof(IWalletHost), new WalletHost(wallet) },
                        { typeof(IInventoryHost), new InventoryHost(inventory) }
                    });

                var host = characterGo.AddComponent<CharacterHost>();
                host.AssignCharacter(character);

                return character;
            }

            void InitializeContext(CharacterContext context, CharacterTypeDefinition typeDefinition)
            {
                var body = context.GameObject.RequireComponentInChildren<CharacterBody>();
                var visionSensor = context.GameObject.RequireComponentInChildren<ConeVisionSensor>();
                var navigationProvider = context.GameObject.RequireComponentInChildren<NavMeshNavigator>();
                var interactionSensor = context.GameObject.RequireComponentInChildren<IInteractionSensor>();
                var interactor = context.GameObject.RequireComponentInChildren<IInteractor>();
                var animator = context.GameObject.GetComponentInChildren<CharacterAnimator>();
                var audio = context.GameObject.RequireComponentInChildren<CharacterAudio>();
                var cameraFocusTarget = context.GameObject.RequireComponentInChildren<CameraFocusTarget>();

                var visionSettings = typeDefinition.GetExtension<ConeVisionSensorSettings>();
                visionSensor.ViewAngle = visionSettings.ViewAngle;
                visionSensor.ViewRadius = visionSettings.ViewRadius;
                visionSensor.EyeOffset = visionSettings.EyeOffset;

                var navSettings = typeDefinition.GetExtension<NavMeshNavigatorSettings>();
                navigationProvider.StoppingDistance = navSettings.StoppingDistance;

                var statusSettings = typeDefinition.GetExtension<CharacterStatusSettings>();
                var status = new CharacterStatus(statusSettings.InitialMaxHealth);
                context.Register(status);

                context.Register<IVisionSensor>(visionSensor);
                context.Register<INavigationProvider>(navigationProvider);
                context.Register(interactionSensor);
                context.Register(interactor);
                context.Register(body);
                context.Register(typeof(ICharacterBody), body);
                context.Register(animator);
                context.Register(audio);
                context.Register(cameraFocusTarget);

                var stateStore = new CharacterActionStateStore();
                var movement = new CharacterMovement(
                    animator,
                    body,
                    context.TypeDefinition);

                context.Register(stateStore);
                context.Register(movement);

                var footstepSettings = typeDefinition.GetExtension<CharacterFootstepSettings>();
                new CharacterFootstepHandler(animator, audio, footstepSettings);
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

        public ICharacter Create(Vector3 position, Quaternion rotation, CharacterCreationParameters parameters)
        {
            return factory(position, rotation, parameters);
        }
    }
}