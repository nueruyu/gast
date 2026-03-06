using System.Collections.Generic;
using System.Linq;
using Gast.Core.Commands;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Pickups;
using Gast.Lib.AI.Debugging;
using Cryst.Domain.Characters;
using Cryst.Domain.Characters.Facets;
using Cryst.Features.CharacterAI.Combat;
using Cryst.Features.CharacterAI.Gathering;
using Cryst.Features.CharacterAI.Strategic;
using UnityEngine;

namespace Cryst.Features.CharacterAI
{
    public class AIBrain : BaseAIBrain
    {
        const string StrategicDomainName = "Strategic";
        const string CombatDomainName = "Combat";
        const string GatheringDomainName = "Gathering";

        readonly StrategicDomain strategicDomain;
        readonly CombatDomain combatDomain;
        readonly GatheringDomain gatheringDomain;
        readonly ObjectiveManager objectiveManager;

        readonly StrategicState strategicState = new();
        readonly CombatState combatState = new();
        readonly GatheringState gatheringState = new();

        public AIBrain(
            StrategicDomain strategicDomain,
            CombatDomain combatDomain,
            GatheringDomain gatheringDomain,
            IContextRegistry contextRegistry,
            ICharacterRepository characterRepository,
            IPickupRepository pickupRepository,
            ICommandDispatcher commandDispatcher,
            ObjectiveManager objectiveManager) : base(contextRegistry, characterRepository, pickupRepository, commandDispatcher, objectiveManager)
        {
            this.strategicDomain = strategicDomain;
            this.combatDomain = combatDomain;
            this.gatheringDomain = gatheringDomain;
            this.objectiveManager = objectiveManager;
        }

        protected override void RegisterDomains(IDomainRegistrar registrar)
        {
            registrar.Register<StrategicState, AIContext<StrategicState>>(
                StrategicDomainName,
                strategicState,
                strategicDomain.CreateRunner(),
                UpdateStrategicWorldState
            );

            combatState.AttackRange = 1.5f;
            combatState.CombatRange = 4.5f;
            registrar.Register<CombatState, AIContext<CombatState>>(
                CombatDomainName,
                combatState,
                combatDomain.CreateRunner(),
                UpdateCombatWorldState
            );

            registrar.Register<GatheringState, AIContext<GatheringState>>(
                GatheringDomainName,
                gatheringState,
                gatheringDomain.CreateRunner(),
                UpdateGatheringWorldState
            );
        }

        void UpdateStrategicWorldState()
        {
            strategicState.AvailableObjectives = objectiveManager.CurrentObjectives
                .Where(o => !o.IsCompleted.Value)
                .ToList();

            strategicState.IsThreatened = actor.VisionSensor.VisibleCharacters
                .Select(c => c.As<BaseCharacter>())
                .Any(otherActor => otherActor.IsThreatTo(actor));
        }

        void UpdateCombatWorldState()
        {
            var target = memory.CombatTarget;
            var isTargetAlive = target != null && target.Status.IsAlive.Value;

            if (isTargetAlive)
            {
                combatState.HasTarget = true;
                combatState.TargetPosition = target.Body.Position;
                combatState.TargetForward = target.Body.Forward;
                combatState.DistanceToTarget = Vector3.Distance(actor.Body.Position, target.Body.Position);
            }
            else
            {
                combatState.HasTarget = false;
                combatState.DistanceToTarget = float.PositiveInfinity;
            }

            combatState.IsReadyToAttack = character.Is(out AttackableCharacter attackable) && attackable.CanAttack();
            combatState.CanGuard = character.Is(out GuardableCharacter guardable) && guardable.CanGuard();

            var currentHealth = actor.Status.Health.Value;
            var maxHealth = actor.Status.MaxHealth.Value;
            combatState.SelfHealthRatio = maxHealth > 0 ? currentHealth / maxHealth : 1f;
        }

        void UpdateGatheringWorldState()
        {
            var currentGoal = memory.CurrentObjective;
            gatheringState.CurrentGoal = currentGoal;
            gatheringState.HasGoal = currentGoal != null;
            gatheringState.IsInCombat = combatState.HasTarget;

            if (memory.InteractableTarget is Component interactableTargetComponent &&
                !interactableTargetComponent)
            {
                memory.InteractableTarget = null;
            }

            var interactableTarget = memory.InteractableTarget;

            gatheringState.HasInteractableTarget = interactableTarget != null;
            if (interactableTarget != null)
            {
                gatheringState.InteractableTargetId = interactableTarget.Id;
                gatheringState.InteractableTargetPosition = interactableTarget.Position;
                var distance = Vector3.Distance(actor.Body.Position, interactableTarget.Position);
                gatheringState.IsInRangeToInteract = distance <= 1.5f;
            }
        }
    }
}
