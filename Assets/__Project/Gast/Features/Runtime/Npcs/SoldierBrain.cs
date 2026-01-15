using Cysharp.Threading.Tasks;
using Gast.Api.AI;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Gast.Features.Npcs
{
    public class SoldierBrain : ICharacterBrain, IGoalAssignable
    {
        readonly Domain<StrategicWorldState, AIContext<StrategicWorldState>> strategicDomain;
        readonly Domain<CombatWorldState, AIContext<CombatWorldState>> combatDomain;
        readonly GoalManager goalManager;
        readonly SharedAIState sharedState;
        readonly IDisposable scope;

        ICharacter character;
        CancellationTokenSource cts;

        IAgentRunner<StrategicWorldState, AIContext<StrategicWorldState>> strategicAgentRunner;
        IAgentRunner<CombatWorldState, AIContext<CombatWorldState>> combatAgentRunner;

        readonly StrategicWorldState strategicState = new();
        readonly CombatWorldState combatState = new();

        public SoldierBrain(
            Domain<StrategicWorldState, AIContext<StrategicWorldState>> strategicDomain,
            Domain<CombatWorldState, AIContext<CombatWorldState>> combatDomain,
            GoalManager goalManager,
            SharedAIState sharedState,
            IDisposable scope)
        {
            this.strategicDomain = strategicDomain;
            this.combatDomain = combatDomain;
            this.goalManager = goalManager;
            this.sharedState = sharedState;
            this.scope = scope;
        }

        public void OnAttached(ICharacter character)
        {
            this.character = character;

            strategicAgentRunner = strategicDomain.CreateAgentRunner();
            combatAgentRunner = combatDomain.CreateAgentRunner();

            combatState.AttackRange = 1.5f;
            combatState.CombatRange = 4.5f;

            cts = new CancellationTokenSource();
            RunAsync(cts.Token).Forget();
        }

        public void OnDetached()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }

        public void SetGoals(List<IGoal> goals)
        {
            goalManager.Update(character.Id, goals);
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
                await strategicAgentRunner.RunAsync(new(character, strategicState, token));
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        async UniTask TacticalLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await combatAgentRunner.RunAsync(new(character, combatState, token));
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        void UpdateStrategicWorldState()
        {
            var goals = goalManager.CurrentGoals;
            var currentGoal = goals.FirstOrDefault(g => !g.IsCompleted);
            strategicState.CurrentGoal = currentGoal;
            strategicState.HasGoal = currentGoal != null;

            if (sharedState.InteractableTarget is Component interactableTargetComponent &&
                !interactableTargetComponent)
            {
                sharedState.InteractableTarget = null;
            }

            var interactableTarget = sharedState.InteractableTarget;

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
            var target = sharedState.CombatTarget;
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
                combatState.DistanceToTarget = float.MaxValue;
            }
            combatState.IsReadyToAttack = character.CanAttack;
        }

        public void Dispose()
        {
            scope?.Dispose();
        }
    }
}