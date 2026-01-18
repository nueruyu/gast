using Cysharp.Threading.Tasks;
using Gast.Api.AI;
using Gast.Domain.Characters;
using Gast.Features.AI.Combat;
using Gast.Features.AI.Strategic;
using Gast.Lib.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Gast.Features.AI
{
    public class GenericBrain : ICharacterBrain, IGoalAssignable
    {
        readonly StrategicDomain strategicDomain;
        readonly CombatDomain combatDomain;
        readonly GoalManager goalManager;

        ICharacter character;
        CancellationTokenSource cts;

        AIRunner<StrategicState, AIContext<StrategicState>> strategicAgentRunner;
        AIRunner<CombatState, AIContext<CombatState>> combatAgentRunner;

        readonly AIMemory memory = new();
        readonly StrategicState strategicState = new();
        readonly CombatState combatState = new();

        public GenericBrain(
            StrategicDomain strategicDomain,
            CombatDomain combatDomain,
            GoalManager goalManager)
        {
            this.strategicDomain = strategicDomain;
            this.combatDomain = combatDomain;
            this.goalManager = goalManager;
        }

        public void OnAttached(ICharacter character)
        {
            this.character = character;

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
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
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
                await strategicAgentRunner.RunAsync(new(character, strategicState, memory, token));
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        async UniTask TacticalLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await combatAgentRunner.RunAsync(new(character, combatState, memory, token));
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
                combatState.DistanceToTarget = float.MaxValue;
            }
            combatState.IsReadyToAttack = character.CanAttack;
        }
    }
}