using Cysharp.Threading.Tasks;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using Gast.Lib.AI.Debugging;
using Cryst.Features.CharacterAI.Combat;
using Cryst.Features.CharacterAI.Gathering;
using Cryst.Domain.Characters;
using Cryst.Domain.Characters.Facets;
using Cryst.Features.CharacterAI.Strategic;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Cryst.Features.CharacterAI
{
    public class AIBrain : ICharacterAIBrain
    {
        const string StrategicDomainName = "Strategic";
        const string CombatDomainName = "Combat";
        const string GatheringDomainName = "Gathering";

        readonly StrategicDomain strategicDomain;
        readonly CombatDomain combatDomain;
        readonly GatheringDomain gatheringDomain;
        readonly ObjectiveManager objectiveManager;
        readonly IContextRegistry contextRegistry;

        BaseCharacter actor;
        ICharacter character;
        ContextKey strategicContextKey;
        ContextKey combatContextKey;
        ContextKey gatheringContextKey;
        CancellationTokenSource cts;

        public IReadOnlyList<IAIObjective> CurrentObjectives => objectiveManager.CurrentObjectives;

        AIRunner<StrategicState, AIContext<StrategicState>> strategicAgentRunner;
        AIRunner<CombatState, AIContext<CombatState>> combatAgentRunner;
        AIRunner<GatheringState, AIContext<GatheringState>> gatheringAgentRunner;

        readonly AIMemory memory = new();
        readonly StrategicState strategicState = new();
        readonly CombatState combatState = new();
        readonly GatheringState gatheringState = new();

        public AIBrain(
            StrategicDomain strategicDomain,
            CombatDomain combatDomain,
            GatheringDomain gatheringDomain,
            ObjectiveManager objectiveManager,
            IContextRegistry contextRegistry)
        {
            this.strategicDomain = strategicDomain;
            this.combatDomain = combatDomain;
            this.gatheringDomain = gatheringDomain;
            this.objectiveManager = objectiveManager;
            this.contextRegistry = contextRegistry;
        }

        public void OnAttached(ICharacter character)
        {
            Debug.Log($"[AIBrain] OnAttached: {character.Id}");

            this.character = character;
            actor = character.As<BaseCharacter>();

            strategicContextKey = new(actor.Id, StrategicDomainName);
            combatContextKey = new(actor.Id, CombatDomainName);
            gatheringContextKey = new(actor.Id, GatheringDomainName);

            contextRegistry.Register(strategicContextKey, strategicState);
            contextRegistry.Register(combatContextKey, combatState);
            contextRegistry.Register(gatheringContextKey, gatheringState);

            strategicAgentRunner = strategicDomain.CreateRunner();
            combatAgentRunner = combatDomain.CreateRunner();
            gatheringAgentRunner = gatheringDomain.CreateRunner();

            combatState.AttackRange = 1.5f;
            combatState.CombatRange = 4.5f;

            cts = new CancellationTokenSource();
            RunAsync(cts.Token).Forget();

            objectiveManager
                .BindCharacter(actor.Id)
                .AddTo(cts.Token);
        }

        public void OnDetached()
        {
            if (actor == null)
                return;

            DebugLogger.ClearContext(strategicContextKey);
            DebugLogger.ClearContext(combatContextKey);
            DebugLogger.ClearContext(gatheringContextKey);

            contextRegistry.Unregister(strategicContextKey);
            contextRegistry.Unregister(combatContextKey);
            contextRegistry.Unregister(gatheringContextKey);

            cts?.Cancel();
            cts?.Dispose();
            cts = null;

            actor = null;
            character = null;
            strategicContextKey = default;
            combatContextKey = default;
            gatheringContextKey = default;
        }

        public void SetObjectives(IEnumerable<IAIObjective> objectives)
        {
            objectiveManager.UpdateObjectives(objectives);
        }

        async UniTaskVoid RunAsync(CancellationToken cancellationToken)
        {
            await UniTask.WhenAll(
                StateUpdateLoop(cancellationToken),
                StrategicLoop(cancellationToken),
                TacticalLoop(cancellationToken),
                GatheringLoop(cancellationToken)
            );
        }

        async UniTask StateUpdateLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                UpdateStrategicWorldState();
                UpdateCombatWorldState();
                UpdateGatheringWorldState();

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }

        async UniTask StrategicLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await strategicAgentRunner.RunAsync(new(
                    strategicContextKey,
                    actor,
                    character,
                    strategicState,
                    memory,
                    UpdateStrategicWorldState,
                    cancellationToken));

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }

        async UniTask TacticalLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await combatAgentRunner.RunAsync(new(
                    combatContextKey,
                    actor,
                    character,
                    combatState,
                    memory,
                    UpdateCombatWorldState,
                    cancellationToken));

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }

        async UniTask GatheringLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await gatheringAgentRunner.RunAsync(new(
                    gatheringContextKey,
                    actor,
                    character,
                    gatheringState,
                    memory,
                    UpdateGatheringWorldState,
                    cancellationToken));

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
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
            if (maxHealth > 0)
            {
                combatState.SelfHealthRatio = currentHealth / maxHealth;
            }
            else
            {
                combatState.SelfHealthRatio = 1f;
            }
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