using System.Linq;
using Cryst.Domain.Characters;
using Cryst.Domain.Characters.Facets;
using Cryst.Features.CharacterAI.Combat;
using Cryst.Features.CharacterAI.Gathering;
using Cryst.Features.CharacterAI.Strategic;
using Gast.Domain.Characters;
using Gast.Lib.AI.Debugging;
using UnityEngine;

namespace Cryst.Features.CharacterAI
{
    public class AIBrain : BaseAIBrain
    {
        const string StrategicDomainName = "Strategic";
        const string CombatDomainName = "Combat";
        const string GatheringDomainName = "Gathering";

        readonly CombatDomain combatDomain;
        readonly GatheringDomain gatheringDomain;
        readonly StrategicDomain strategicDomain;

        public AIBrain(
            StrategicDomain strategicDomain,
            CombatDomain combatDomain,
            GatheringDomain gatheringDomain,
            IContextRegistry contextRegistry,
            AIBrainServices services,
            ObjectiveManager objectiveManager) :
            base(contextRegistry, services, objectiveManager)
        {
            this.strategicDomain = strategicDomain;
            this.combatDomain = combatDomain;
            this.gatheringDomain = gatheringDomain;
        }

        protected override void RegisterDomains(IDomainRegistrar registrar)
        {
            registrar.Register(
                StrategicDomainName,
                strategicDomain.CreateRunner(),
                new StrategicState(),
                UpdateStrategicWorldState);

            registrar.Register(
                CombatDomainName,
                combatDomain.CreateRunner(),
                new CombatState(),
                UpdateCombatWorldState);

            registrar.Register(
                GatheringDomainName,
                gatheringDomain.CreateRunner(),
                new GatheringState(),
                UpdateGatheringWorldState);
        }

        void UpdateStrategicWorldState(ActorContext<StrategicState> context)
        {
            var actor = context.Actor;
            var strategicState = context.WorldState;

            strategicState.AvailableObjectives = CurrentObjectives
                .Where(o => !o.IsCompleted.Value)
                .ToList();

            strategicState.IsThreatened = actor.VisionSensor.VisibleCharacters
                .Select(c => c.As<BaseCharacter>())
                .Any(otherActor => otherActor.IsThreatTo(actor));
        }

        void UpdateCombatWorldState(ActorContext<CombatState> context)
        {
            var memory = context.Memory;
            var character = context.Character;
            var actor = context.Actor;
            var combatState = context.WorldState;

            combatState.AttackRange = 1.5f;
            combatState.CombatRange = 4.5f;

            if (memory.HasTarget)
            {
                var target = memory.CombatTarget;
                combatState.HasTarget = true;
                combatState.TargetPosition = target.Body.Position;
                combatState.TargetForward = target.Body.Forward;
                combatState.DistanceToTarget = Vector3.Distance(actor.Body.Position,
                    target.Body.Position);
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

        void UpdateGatheringWorldState(ActorContext<GatheringState> context)
        {
            var memory = context.Memory;
            var actor = context.Actor;
            var gatheringState = context.WorldState;

            var currentGoal = memory.CurrentObjective;
            gatheringState.CurrentGoal = currentGoal;
            gatheringState.HasGoal = currentGoal != null;
            gatheringState.IsInCombat = memory.HasTarget;

            if (memory.InteractableTarget is Component interactableTargetComponent &&
                !interactableTargetComponent)
                memory.InteractableTarget = null;

            var interactableTarget = memory.InteractableTarget;

            gatheringState.HasInteractableTarget = interactableTarget != null;
            if (interactableTarget != null)
            {
                gatheringState.InteractableTargetId = interactableTarget.Id;
                gatheringState.InteractableTargetPosition = interactableTarget.Position;
                var distance = Vector3.Distance(actor.Body.Position,
                    interactableTarget.Position);
                gatheringState.IsInRangeToInteract = distance <= 1.5f;
            }
        }
    }
}