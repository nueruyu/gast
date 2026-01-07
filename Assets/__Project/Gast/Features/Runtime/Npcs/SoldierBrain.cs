using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Api.AI;
using Gast.Api.AI.Goals;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using UnityEngine;

namespace Gast.Features.Npcs
{
    public class SoldierBrain : ICharacterBrain, IGoalAssignable
    {
        readonly GoalManager goalManager;
        readonly Domain<CombatWorldState> combatDomain;

        ICharacter character;
        CancellationTokenSource cts;

        CombatWorldState combatState;

        public SoldierBrain(SoldierBrainSettings settings, GoalManager goalManager)
        {
            this.goalManager = goalManager;
            this.combatDomain = CombatDomain.Create(settings);
        }

        public void OnAttached(ICharacter character)
        {
            this.character = character;

            combatState = new CombatWorldState
            {
                AttackRange = 1.5f,
                CombatRange = 4.5f
            };

            cts = new();
            RunAsync(cts.Token).Forget();
        }

        public void OnDetached()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }

        // Called externally by a use case to set goals
        public void SetGoals(List<IGoal> goals)
        {
            goalManager.SetGoals(goals);
        }

        async UniTaskVoid RunAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                UpdateCombatWorldState();

                var goals = goalManager.CurrentGoals;
                var currentGoal = goals.FirstOrDefault(g => !g.IsCompleted);

                if (currentGoal != null)
                {
                    // For now, simple goal-directed behavior.
                    // This part will be replaced by the strategic HTN domain later.
                    await ExecuteGoalDirectedBehavior(currentGoal, token);
                }
                else
                {
                    // No goals, execute default combat behavior
                    var combatCtx = new Context<CombatWorldState>(combatState, () => combatState, character, token);
                    await combatDomain.RootTask.RunAsync(combatCtx);
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        async UniTask ExecuteGoalDirectedBehavior(IGoal goal, CancellationToken token)
        {
            // This is a simplified placeholder for a strategic HTN.
            // It just finds the nearest target related to the goal and engages.
            ICharacter target = null;

            if (goal is DefeatCharacterGoal defeatGoal)
            {
                target = FindClosestCharacterOfType(defeatGoal.TargetTypeId);
            }

            if (target != null)
            {
                // We have a strategic target, force the combat state to focus on it.
                combatState.HasTarget = true;
                combatState.TargetPosition = target.Body.Position;
                combatState.TargetForward = target.Body.Forward;
                combatState.DistanceToTarget = Vector3.Distance(character.Body.Position, target.Body.Position);
            }
            else
            {
                // Can't find target for goal, revert to default behavior for now.
                combatState.HasTarget = false;
            }

            var combatCtx = new Context<CombatWorldState>(combatState, () => combatState, character, token);
            await combatDomain.RootTask.RunAsync(combatCtx);
        }

        void UpdateCombatWorldState()
        {
            // This is the default "situational awareness" logic.
            // It can be overridden by goal-directed behavior.
            var visibleEnemies = character.VisionSensor.VisibleCharacters
                .Where(c => c != character && c.IsAlive)
                .ToArray();

            if (visibleEnemies.Length > 0)
            {
                var closestEnemy = visibleEnemies
                    .OrderBy(e => Vector3.Distance(character.VisionSensor.EyePosition, e.Body.Position))
                    .First();

                combatState.HasTarget = true;
                combatState.TargetPosition = closestEnemy.Body.Position;
                combatState.TargetForward = closestEnemy.Body.Forward;
                combatState.DistanceToTarget = Vector3.Distance(character.Body.Position, closestEnemy.Body.Position);
            }
            else
            {
                combatState.HasTarget = false;
                combatState.DistanceToTarget = float.MaxValue;
            }

            combatState.IsReadyToAttack = character.CanAttack;
        }

        ICharacter FindClosestCharacterOfType(CharacterTypeId typeId)
        {
            // In a real scenario, this would involve searching beyond vision sensor.
            // For now, we'll just use the vision sensor for simplicity.
            return character.VisionSensor.VisibleCharacters
                .Where(c => c.TypeId == typeId && c.IsAlive)
                .OrderBy(c => Vector3.Distance(character.Body.Position, c.Body.Position))
                .FirstOrDefault();
        }
    }
}