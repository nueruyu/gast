using Cysharp.Threading.Tasks;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Features.AI.Combat;
using Gast.Features.AI.Strategic;
using Gast.Lib.AI;
using Gast.Lib.AI.Debugging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Gast.Features.AI
{
    public class AIBrain : ICharacterAIBrain
    {
        const string StrategicDomainName = "Strategic";
        const string CombatDomainName = "Combat";

        readonly StrategicDomain strategicDomain;
        readonly CombatDomain combatDomain;
        readonly GoalManager goalManager;
        readonly IContextRegistry contextRegistry;

        ICharacter character;
        ContextKey strategicContextKey;
        ContextKey combatContextKey;
        CancellationTokenSource cts;

        public IReadOnlyList<IGoal> CurrentGoals => goalManager.CurrentGoals;

        AIRunner<StrategicState, AIContext<StrategicState>> strategicAgentRunner;
        AIRunner<CombatState, AIContext<CombatState>> combatAgentRunner;

        readonly AIMemory memory = new();
        readonly StrategicState strategicState = new();
        readonly CombatState combatState = new();

        public AIBrain(
            StrategicDomain strategicDomain,
            CombatDomain combatDomain,
            GoalManager goalManager,
            IContextRegistry contextRegistry)
        {
            this.strategicDomain = strategicDomain;
            this.combatDomain = combatDomain;
            this.goalManager = goalManager;
            this.contextRegistry = contextRegistry;
        }

        public void OnAttached(ICharacter character)
        {
            Debug.Log($"[AIBrain] OnAttached: {character.Id}");

            this.character = character;

            strategicContextKey = new(character.Id, StrategicDomainName);
            combatContextKey = new(character.Id, CombatDomainName);

            contextRegistry.Register(strategicContextKey, strategicState);
            contextRegistry.Register(combatContextKey, combatState);

            strategicAgentRunner = strategicDomain.CreateRunner();
            combatAgentRunner = combatDomain.CreateRunner();

            combatState.AttackRange = 1.5f;
            combatState.CombatRange = 4.5f;

            cts = new CancellationTokenSource();
            RunAsync(cts.Token).Forget();

            goalManager
                .BindCharacter(character.Id)
                .AddTo(cts.Token);
        }

        public void OnDetached()
        {
            if (character == null)
                return;

            DebugLogger.ClearContext(strategicContextKey);
            DebugLogger.ClearContext(combatContextKey);

            contextRegistry.Unregister(strategicContextKey);
            contextRegistry.Unregister(combatContextKey);

            cts?.Cancel();
            cts?.Dispose();
            cts = null;

            character = null;
            strategicContextKey = default;
            combatContextKey = default;
        }

        public void SetGoals(IEnumerable<IGoal> goals)
        {
            goalManager.UpdateGoals(goals);
        }

        async UniTaskVoid RunAsync(CancellationToken token)
        {
            await UniTask.WhenAll(
                StateUpdateLoop(token),
                StrategicLoop(token),
                TacticalLoop(token)
            );
        }

        async UniTask StateUpdateLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                UpdateStrategicWorldState();
                UpdateCombatWorldState();

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        async UniTask StrategicLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await strategicAgentRunner.RunAsync(new(
                    character,
                    strategicState,
                    memory,
                    StrategicDomainName,
                    token));

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        async UniTask TacticalLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await combatAgentRunner.RunAsync(new(
                    character,
                    combatState,
                    memory,
                    CombatDomainName,
                    token));

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        void UpdateStrategicWorldState()
        {
            var goals = goalManager.CurrentGoals;
            var currentGoal = goals.FirstOrDefault(g => !g.IsCompleted);
            strategicState.CurrentGoal = currentGoal;
            strategicState.HasGoal = currentGoal != null;

            if (memory.InteractableTarget is Component interactableTargetComponent &&
                !interactableTargetComponent)
            {
                memory.InteractableTarget = null;
            }

            var interactableTarget = memory.InteractableTarget;

            strategicState.HasInteractableTarget = interactableTarget != null;
            if (interactableTarget != null)
            {
                strategicState.InteractableTargetId = interactableTarget.Id;
                strategicState.InteractableTargetPosition = interactableTarget.Position;
                var distance = Vector3.Distance(character.Body.Position, interactableTarget.Position);
                strategicState.IsInRangeToInteract = distance <= 1.5f;
            }

            strategicState.IsThreatened = character.VisionSensor.VisibleCharacters
                .Any(c => c.IsAlive && c.Status.Faction != character.Status.Faction);
        }

        void UpdateCombatWorldState()
        {
            var target = memory.CombatTarget;
            if (target != null && target.IsAlive)
            {
                combatState.HasTarget = true;
                combatState.TargetPosition = target.Body.Position;
                combatState.TargetForward = target.Body.Forward;
                combatState.DistanceToTarget = Vector3.Distance(character.Body.Position, target.Body.Position);
            }
            else
            {
                combatState.HasTarget = false;
                combatState.DistanceToTarget = float.PositiveInfinity;
            }
            combatState.IsReadyToAttack = character.CanAttack;
        }
    }
}