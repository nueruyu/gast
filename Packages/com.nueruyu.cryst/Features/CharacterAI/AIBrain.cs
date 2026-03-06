using System.Linq;
using Cryst.Domain.Characters;
using Cryst.Domain.Characters.Facets;
using Cryst.Features.CharacterAI.Combat;
using Cryst.Features.CharacterAI.Gathering;
using Cryst.Features.CharacterAI.Strategic;
using Gast.Domain.Characters;
using Gast.Lib.AI;
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
        readonly CombatState combatState = new();
        readonly GatheringDomain gatheringDomain;
        readonly GatheringState gatheringState = new();

        readonly StrategicDomain strategicDomain;

        readonly StrategicState strategicState = new();

        public AIBrain(
            StrategicDomain strategicDomain,
            CombatDomain combatDomain,
            GatheringDomain gatheringDomain,
            IContextRegistry contextRegistry,
            AIBrainServices services,
            ObjectiveManager objectiveManager) : base(contextRegistry,
            services,
            objectiveManager)
        {
            this.strategicDomain = strategicDomain;
            this.combatDomain = combatDomain;
            this.gatheringDomain = gatheringDomain;
        }

        protected override void RegisterDomains(IDomainRegistrar registrar)
        {
            var strategicKey = new ContextKey(actor.Id,
                StrategicDomainName);
            var strategicActorContext = new ActorContext<StrategicState>(
                services,
                actor,
                character,
                memory,
                strategicState,
                UpdateStrategicWorldState);
            var strategicContext = new AIContext<ActorContext<StrategicState>>(strategicKey, strategicActorContext);
            registrar.Register(strategicKey,
                strategicDomain.CreateRunner(),
                strategicContext);

            combatState.AttackRange = 1.5f;
            combatState.CombatRange = 4.5f;
            var combatKey = new ContextKey(actor.Id,
                CombatDomainName);
            var combatActorContext = new ActorContext<CombatState>(
                services,
                actor,
                character,
                memory,
                combatState,
                UpdateCombatWorldState);
            var combatContext = new AIContext<ActorContext<CombatState>>(combatKey, combatActorContext);
            registrar.Register(combatKey,
                combatDomain.CreateRunner(),
                combatContext);

            var gatheringKey = new ContextKey(actor.Id,
                GatheringDomainName);
            var gatheringActorContext = new ActorContext<GatheringState>(
                services,
                actor,
                character,
                memory,
                gatheringState,
                UpdateGatheringWorldState);
            var gatheringContext = new AIContext<ActorContext<GatheringState>>(gatheringKey, gatheringActorContext);
            registrar.Register(gatheringKey,
                gatheringDomain.CreateRunner(),
                gatheringContext);
        }

        void UpdateStrategicWorldState()
        {
            strategicState.AvailableObjectives = CurrentObjectives
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

        void UpdateGatheringWorldState()
        {
            var currentGoal = memory.CurrentObjective;
            gatheringState.CurrentGoal = currentGoal;
            gatheringState.HasGoal = currentGoal != null;
            gatheringState.IsInCombat = combatState.HasTarget;

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
