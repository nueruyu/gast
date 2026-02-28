using Cryst.Domain.Characters;
using Cryst.Features.CharacterActions;
using Cysharp.Threading.Tasks;
using Gast.Core.Observables;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Interactions;
using R3;
using R3.Triggers;
using System;
using Cryst.Features.Characters.Footsteps;
using Gast.Unity.Features.Cameras;
using Gast.Unity.Features.Characters;
using Gast.Unity.Features.Navigations;
using Gast.Unity.Features.Sensors;
using Gast.Unity.Infrastructure.Characters;
using Gast.Unity.Shared.UnityExtensions;
using UnityEngine;

namespace Cryst.Infrastructure.Characters
{
    public class CharacterFactory : ICharacterFactory<CharacterCreationParameters>
    {
        readonly Func<Vector3, Quaternion, CharacterCreationParameters, ICharacter> factory;

        public CharacterFactory(
            CharacterTypeRepository typeRepository,
            ICharacterActionFactory actionFactory)
        {
            factory = Create;

            ICharacter Create(Vector3 position, Quaternion rotation, CharacterCreationParameters parameters)
            {
                var definition = typeRepository.Get(parameters.TypeId);
                var characterPrefab = definition.CharacterPrefab;

                var characterId = CharacterId.New();
                var characterGo = UnityEngine.Object.Instantiate(characterPrefab, position, rotation);
                characterGo.name = $"{definition.DisplayName}@{characterPrefab.name}:{characterId}";

                var visual = UnityEngine.Object.Instantiate(definition.VisualPrefab, characterGo.transform);
                visual.name = $"Visual ({definition.VisualPrefab.name})";

                var destroyCancellationToken = characterGo.GetCancellationTokenOnDestroy();
                var context = new CharacterContext(
                    characterId,
                    characterGo,
                    destroyCancellationToken);

                InitializeContext(context, characterGo, definition);

                var actionController = CreateActionController(context, definition);

                characterGo.UpdateAsObservable().Subscribe(_ => actionController.Update());

                var crystCharacter = new CrystCharacter(
                    characterId,
                    definition,
                    parameters.Faction,
                    actionController,
                    context.Resolve<CharacterStatus>(),
                    context.Resolve<CharacterActionStateStore>(),
                    context.Resolve<ICharacterBody>(),
                    context.Resolve<IVisionSensor>(),
                    context.Resolve<INavigationProvider>());

                var wallet = new Wallet(definition.InitialMoney);
                var inventory = new Inventory(definition.SlotCapacity);
                var interactor = characterGo.RequireComponentInChildren<IInteractor>();

                var character = new Character(
                    context,
                    new()
                    {
                        { typeof(CrystCharacter), crystCharacter },
                        { typeof(IWalletHost), new WalletHost(wallet) },
                        { typeof(IInventoryHost), new InventoryHost(inventory) },
                        { typeof(ICameraFocusTarget), new CameraFocusTarget(characterGo.transform) },
                        { typeof(IInteractor), interactor }
                    });

                var host = characterGo.AddComponent<CharacterHost>();
                host.AssignCharacter(character);

                return character;
            }

            void InitializeContext(CharacterContext context, GameObject gameObject, CharacterTypeDefinition typeDefinition)
            {
                var body = gameObject.RequireComponentInChildren<CharacterBody>();
                var visionSensor = gameObject.RequireComponentInChildren<ConeVisionSensor>();
                var navigationProvider = gameObject.RequireComponentInChildren<NavMeshNavigator>();
                var animator = gameObject.GetComponentInChildren<CharacterAnimator>();
                var audio = gameObject.RequireComponentInChildren<CharacterAudio>();

                var visionSettings = typeDefinition.GetExtension<ConeVisionSensorSettings>();
                visionSensor.ViewAngle = visionSettings.ViewAngle;
                visionSensor.ViewRadius = visionSettings.ViewRadius;
                visionSensor.EyeOffset = visionSettings.EyeOffset;

                var navSettings = typeDefinition.GetExtension<NavMeshNavigatorSettings>();
                navigationProvider.StoppingDistance = navSettings.StoppingDistance;

                var statusSettings = typeDefinition.GetExtension<CharacterStatusSettings>();
                var status = new CharacterStatus(statusSettings.InitialMaxHealth);
                context.Register(status);

                context.Register<ICharacterTypeDefinition>(typeDefinition);
                context.Register<IVisionSensor>(visionSensor);
                context.Register<INavigationProvider>(navigationProvider);
                context.Register(body);
                context.Register(typeof(ICharacterBody), body);
                context.Register(animator);
                context.Register(audio);

                var stateStore = new CharacterActionStateStore();
                var movement = new CharacterMovement(
                    animator,
                    body,
                    typeDefinition);

                context.Register(stateStore);
                context.Register(movement);

                var footstepSettings = typeDefinition.GetExtension<CharacterFootstepSettings>();
                new CharacterFootstepHandler(animator, audio, footstepSettings).AddTo(gameObject);
            }

            CharacterActionController CreateActionController(CharacterContext context, CharacterTypeDefinition definition)
            {
                var actionController = new CharacterActionController();

                foreach (var settings in definition.ActionSettings)
                {
                    var action = actionFactory.Create(settings, context);
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