using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Features.Navigations;
using Gast.Features.Characters;
using Gast.Features.Characters.Actions;
using Gast.Features.Sensors;
using Gast.Infrastructure.Repositories;
using Gast.Infrastructure.Settings;
using System;
using UnityEngine;
using Gast.Features.Combat;
using Gast.Features.Characters.Audios;
using Gast.Shared.UnityExtensions;

namespace Gast.Infrastructure.Factories
{
    /// <summary>
    /// Factory implementation for creating character instances.
    /// </summary>
    public class CharacterFactory : ICharacterFactory
    {
        readonly CharacterTypeRepository typeRepository;
        readonly ICharacterActorRepository characterActorRepository;
        readonly CharacterFootstepService footstepService;
        readonly ICombatMethodFactory combatMethodFactory;

        public CharacterFactory(
            CharacterTypeRepository typeRepository,
            ICharacterActorRepository characterActorRepository,
            CharacterFootstepService footstepService,
            ICombatMethodFactory combatMethodFactory)
        {
            this.typeRepository = typeRepository ?? throw new ArgumentNullException(nameof(typeRepository));
            this.characterActorRepository = characterActorRepository ?? throw new ArgumentNullException(nameof(characterActorRepository));
            this.footstepService = footstepService ?? throw new ArgumentNullException(nameof(footstepService));
            this.combatMethodFactory = combatMethodFactory ?? throw new ArgumentNullException(nameof(combatMethodFactory));
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

            var context = CreateContext(characterId, character.gameObject, definition, faction);

            var status = new CharacterStatus(definition.MaxHealth, faction);
            var wallet = new Wallet(definition.InitialMoney);
            var inventory = new Inventory(definition.SlotCapacity);

            var actionController = CreateActionController(context, definition);
            character.Initialize(
                context,
                definition,
                actionController,
                status,
                wallet,
                inventory);

            characterActorRepository.Register(character);
            footstepService.Register(context, definition.FootstepSettings);

            return character;
        }

        CharacterContext CreateContext(
            CharacterId id,
            GameObject characterGo,
            CharacterTypeDefinition definition,
            Faction faction)
        {
            var body = characterGo.RequireComponent<CharacterBody>();
            var animator = characterGo.RequireComponentInChildren<CharacterAnimator>();
            var animationReceiver = characterGo.RequireComponentInChildren<CharacterAnimationReceiver>();
            var audio = characterGo.RequireComponentInChildren<CharacterAudio>();

            var visionSensor = characterGo.RequireComponentInChildren<ConeVisionSensor>();
            visionSensor.ViewAngle = definition.SensorViewAngle;
            visionSensor.ViewRadius = definition.SensorViewRadius;
            visionSensor.EyeOffset = definition.SensorEyeOffset;

            var navigationProvider = characterGo.RequireComponentInChildren<NavMeshNavigator>();
            navigationProvider.StoppingDistance = definition.NavigationStoppingDistance;

            return new CharacterContext(
                id,
                definition.TypeId,
                faction,
                body,
                animator,
                animationReceiver,
                audio,
                visionSensor,
                navigationProvider);
        }

        CharacterActionController CreateActionController(CharacterContext character, CharacterTypeDefinition definition)
        {
            var combatMethod = combatMethodFactory.CreateMethod(definition.CombatMethodSettings);
            combatMethod.BindEvents(character);

            var actionController = new CharacterActionController(character);

            var dieAction = new DieAction(character);
            actionController.RegisterAction(dieAction);

            var dashAction = new DashAction(
                character,
                definition.DashDuration,
                definition.DashCooldown,
                definition.DashForce,
                definition.DashSpeedCurve
            );
            actionController.RegisterAction(dashAction);

            var hitAction = new HitAction(character);
            actionController.RegisterAction(hitAction);

            var attackAction = new AttackAction(
                character,
                combatMethod,
                definition.AttackCooldown);
            actionController.RegisterAction(attackAction);

            var guardAction = new GuardAction(character);
            actionController.RegisterAction(guardAction);

            return actionController;
        }
    }
}