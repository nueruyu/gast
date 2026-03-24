using Cryst.Domain.Characters;
using Cryst.Domain.Characters.Facets;
using Cryst.Features.CharacterActions;
using Cryst.Features.CharacterActions.Actions.Attack;
using Cryst.Features.CharacterActions.Actions.Dash;
using Cryst.Features.CharacterActions.Actions.Guard;
using Cryst.Features.CharacterActions.Actions.Jump;
using Cysharp.Threading.Tasks;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Interactions;
using R3;
using R3.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using Cryst.Features.Characters.Footsteps;
using Gast.Unity.Features.Cameras;
using Gast.Unity.Features.Characters;
using Gast.Unity.Features.Navigations;
using Gast.Unity.Features.Sensors;
using Gast.Unity.Shared.Attachments;
using Gast.Unity.Shared.UnityExtensions;
using UnityEngine;

namespace Cryst.Infrastructure.Characters
{
    public class CharacterFactory : ICharacterFactory<CharacterCreationParameters>
    {
        readonly Func<Vector3, Quaternion, CharacterCreationParameters, ICharacter> factory;

        public CharacterFactory(
            ICharacterTypeRepository typeRepository,
            ICharacterActionFactory actionFactory)
        {
            factory = Create;

            ICharacter Create(Vector3 position, Quaternion rotation, CharacterCreationParameters parameters)
            {
                var definition = typeRepository.Get(parameters.TypeId);
                var prefabSettings = definition.GetSettings<CharacterPrefabSettings>();
                var characterPrefab = prefabSettings.CharacterPrefab;

                var characterId = CharacterId.New();
                var characterGo = UnityEngine.Object.Instantiate(characterPrefab, position, rotation);
                characterGo.name = $"{definition.DisplayName}@{characterPrefab.name}:{characterId}";

                var visual = UnityEngine.Object.Instantiate(prefabSettings.VisualPrefab, characterGo.transform);
                visual.name = $"Visual ({prefabSettings.VisualPrefab.name})";

                var destroyCancellationToken = characterGo.GetCancellationTokenOnDestroy();
                var context = new CharacterContext(
                    characterId,
                    characterGo,
                    destroyCancellationToken);

                InitializeContext(context, characterGo, definition);

                var actionProfile = definition.GetSettings<CharacterActionProfile>();
                var actionController = CreateActionController(context, actionProfile.ActionSettings);

                characterGo.UpdateAsObservable().Subscribe(_ => actionController.Update());

                var baseCharacter = new BaseCharacter(
                    characterId,
                    definition,
                    parameters.Faction,
                    actionController,
                    context.Resolve<CharacterStatus>(),
                    context.Resolve<ICharacterBody>(),
                    context.Resolve<IVisionSensor>(),
                    context.Resolve<INavigationProvider>());

                var economySettings = definition.GetSettings<CharacterEconomySettings>();
                var wallet = new Wallet(economySettings.InitialMoney);
                var inventory = new Inventory(economySettings.SlotCapacity);
                var interactor = characterGo.RequireComponentInChildren<IInteractor>();

                var facets = CreateFacets(actionController, context, actionProfile.ActionSettings);
                facets[typeof(BaseCharacter)] = baseCharacter;
                facets[typeof(IWalletHost)] = new WalletHost(wallet);
                facets[typeof(IInventoryHost)] = new InventoryHost(inventory);
                facets[typeof(ICameraFocusTarget)] = new CameraFocusTarget(characterGo.transform);
                facets[typeof(IInteractor)] = interactor;

                if (parameters.Territory.HasValue)
                {
                    facets[typeof(TerritorialCharacter)] = new TerritorialCharacter(parameters.Territory.Value, context.Resolve<ICharacterBody>());
                }

                var character = new Character(context, facets);

                var host = characterGo.AddComponent<CharacterHost>();
                host.AssignCharacter(character);

                return character;
            }

            Dictionary<Type, ICharacterFacet> CreateFacets(
                ICharacterActionController actionController,
                CharacterContext context,
                IReadOnlyList<CharacterActionSettings> actionSettings)
            {
                var facets = new Dictionary<Type, ICharacterFacet>();
                var actionSettingsTypes = actionSettings.Select(s => s.GetType()).ToHashSet();

                var stateStore = context.Resolve<CharacterActionStateStore>();
                facets.Add(typeof(SprintableCharacter), new SprintableCharacter(stateStore));

                if (actionSettingsTypes.Contains(typeof(AttackActionSettings)))
                    facets.Add(typeof(AttackableCharacter), new AttackableCharacter(actionController));

                if (actionSettingsTypes.Contains(typeof(DashActionSettings)))
                    facets.Add(typeof(DashableCharacter), new DashableCharacter(actionController));

                if (actionSettingsTypes.Contains(typeof(GuardActionSettings)))
                    facets.Add(typeof(GuardableCharacter), new GuardableCharacter(actionController));

                if (actionSettingsTypes.Contains(typeof(JumpActionSettings)))
                    facets.Add(typeof(JumpableCharacter), new JumpableCharacter(actionController));

                return facets;
            }

            void InitializeContext(CharacterContext context, GameObject gameObject, ICharacterTypeDefinition typeDefinition)
            {
                var body = gameObject.RequireComponentInChildren<CharacterBody>();
                var visionSensor = gameObject.RequireComponentInChildren<ConeVisionSensor>();
                var navigationProvider = gameObject.RequireComponentInChildren<NavMeshNavigator>();
                var animator = gameObject.GetComponentInChildren<CharacterAnimator>();
                var audio = gameObject.RequireComponentInChildren<CharacterAudio>();

                var visionSettings = typeDefinition.GetSettings<ConeVisionSensorSettings>();
                visionSensor.ViewAngle = visionSettings.ViewAngle;
                visionSensor.ViewRadius = visionSettings.ViewRadius;
                visionSensor.EyeOffset = visionSettings.EyeOffset;

                var navSettings = typeDefinition.GetSettings<NavMeshNavigatorSettings>();
                navigationProvider.StoppingDistance = navSettings.StoppingDistance;

                var statusSettings = typeDefinition.GetSettings<CharacterStatusSettings>();
                var maxHunger = parameters.Faction == Faction.Ally
                    ? statusSettings.InitialMaxHunger
                    : float.MaxValue;
                var status = new CharacterStatus(statusSettings.InitialMaxHealth, maxHunger);
                context.Register(status);

                context.Register(typeDefinition);
                context.Register<IVisionSensor>(visionSensor);
                context.Register<INavigationProvider>(navigationProvider);
                context.Register(body);
                context.Register(typeof(ICharacterBody), body);
                context.Register(animator);
                context.Register(audio);

                var attachmentAnchors = gameObject.GetComponentsInChildren<AttachmentAnchor>();
                var anchorRegistry = new AttachmentAnchorRegistry(attachmentAnchors);
                context.Register(anchorRegistry);

                var stateStore = new CharacterActionStateStore();
                context.Register(stateStore);

                var footstepSettings = typeDefinition.GetSettings<CharacterFootstepSettings>();
                new CharacterFootstepHandler(animator, audio, footstepSettings).AddTo(gameObject);
            }

            CharacterActionController CreateActionController(CharacterContext context, IReadOnlyList<CharacterActionSettings> actionSettings) 
            {
                var actionController = new CharacterActionController();

                foreach (var settings in actionSettings)
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