using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Api.AI;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using UnityEngine;

namespace Gast.Features.Npcs
{
    public class SoldierBrain : ICharacterBrain, IGoalAssignable
    {
        readonly GoalManager goalManager;
        readonly Domain<StrategicWorldState> strategicDomain;
        readonly Domain<CombatWorldState> combatDomain;
        readonly SharedAIState sharedState;

        ICharacter character;
        CancellationTokenSource cts;

        StrategicWorldState strategicState;
        CombatWorldState combatState;

        public SoldierBrain(
            SoldierBrainSettings settings,
            GoalManager goalManager,
            Domain<StrategicWorldState> strategicDomain,
            SharedAIState sharedState)
        {
            this.goalManager = goalManager;
            this.strategicDomain = strategicDomain;
            this.combatDomain = CombatDomain.Create(settings);
            this.sharedState = sharedState;
        }

        public void OnAttached(ICharacter character)
        {
            this.character = character;
            combatState = new CombatWorldState { AttackRange = 1.5f, CombatRange = 4.5f };
            strategicState = new StrategicWorldState();

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
            goalManager.SetGoals(goals);
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
                var context = new Context<StrategicWorldState>(strategicState, () => strategicState, character, token);
                await strategicDomain.RootTask.RunAsync(context);

                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
            }
        }

        async UniTask TacticalLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var context = new Context<CombatWorldState>(combatState, () => combatState, character, token);
                await combatDomain.RootTask.RunAsync(context);

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        void UpdateStrategicWorldState()
        {
            var goals = goalManager.CurrentGoals;
            var currentGoal = goals.FirstOrDefault(g => !g.IsCompleted);
            strategicState.CurrentGoal = currentGoal;
            strategicState.HasGoal = currentGoal != null;

            var interactableTarget = sharedState.InteractableTarget;
            strategicState.HasInteractableTarget = interactableTarget != null;
            if (interactableTarget != null)
            {
                strategicState.InteractableTargetId = interactableTarget.Id;
                strategicState.InteractableTargetPosition = interactableTarget.Position;
                var distance = Vector3.Distance(character.Body.Position, interactableTarget.Position);
                strategicState.IsInRangeToInteract = distance <= 1.5f;
            }
        }

        void UpdateCombatWorldState()
        {
            var target = sharedState.StrategicTarget;
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