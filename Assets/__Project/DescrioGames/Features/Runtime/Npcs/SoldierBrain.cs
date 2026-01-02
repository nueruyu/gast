using Cysharp.Threading.Tasks;
using DescrioGames.Domain.Characters;
using DescrioGames.Lib.AI;
using DescrioGames.Lib.AI.Builders;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace DescrioGames.Features.Npcs
{
    public class SoldierBrain : ICharacterBrain
    {
        readonly Domain<CombatWorldState> domain;

        CombatWorldState worldState;
        ICharacter character;
        CancellationTokenSource cts;

        public SoldierBrain(SoldierBrainSettings settings)
        {
            var idleAction = settings.IdleAction;
            var chaseAction = settings.ChaseTargetAction;
            var attackAction = settings.MeleeAttackAction;
            var backOffAction = settings.BackOffAction;
            var strafeAction = settings.StrafeAction;

            domain = new DomainBuilder<CombatWorldState>()
                .RegisterTask(chaseAction)
                .RegisterTask(attackAction)
                .RegisterTask(backOffAction)
                .RegisterTask(strafeAction)
                .RegisterTask(idleAction)
                .DefineCompound("EngageTarget")
                    .AddMethod("Attack")
                        .Condition(s => s.IsInAttackRange && s.IsReadyToAttack)
                        .Do(attackAction)
                    .End()
                    .AddMethod("Withdraw")
                        .Condition(s => s.IsInAttackRange && !s.IsReadyToAttack)
                        .Do(backOffAction)
                    .End()
                    .AddMethod("Approach_Tactical")
                        .Condition(s => !s.IsInAttackRange && s.IsInCombatRange)
                        .Do(strafeAction)
                    .End()
                    .AddMethod("Chase")
                        .Condition(s => !s.IsInCombatRange)
                        .Do(chaseAction)
                    .End()
                .End()

                .DefineRoot()
                    .AddMethod("Combat")
                        .Condition(s => s.HasTarget)
                        .Do("EngageTarget")
                    .End()
                    .AddMethod("Idle")
                        .Condition(s => !s.HasTarget)
                        .Do(idleAction)
                    .End()
                .End()

                .Build();
        }

        public void OnAttached(ICharacter character)
        {
            this.character = character;

            // Initialize world state for melee combat
            worldState = new CombatWorldState
            {
                HasTarget = false,
                TargetPosition = Vector3.zero,
                DistanceToTarget = float.MaxValue,
                IsReadyToAttack = true,
                AttackRange = 1.5f, // 1.5 meters melee attack range
                CombatRange = 4.5f  // 4.5 meters tactical positioning range
            };

            cts = new CancellationTokenSource();

            RunStateUpdateLoop(cts.Token).Forget();
            RunHTN(cts.Token).Forget();
        }

        public void OnDetached()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }

        async UniTaskVoid RunStateUpdateLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                UpdateWorldState();
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        async UniTaskVoid RunHTN(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var ctx = new Context<CombatWorldState>(
                        worldState,
                        () => worldState,
                        character,
                        token);

                    await domain.RootTask.RunAsync(ctx);

                    await UniTask.Yield(token);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        void UpdateWorldState()
        {
            var visibleEnemies = character.VisionSensor.VisibleCharacters
                .Where(c => c != character && c.IsAlive) // Exclude self and dead characters
                .ToArray();

            if (visibleEnemies.Length > 0)
            {
                // Pick closest enemy as target
                var closestEnemy = visibleEnemies
                    .OrderBy(e => Vector3.Distance(character.VisionSensor.EyePosition, e.Body.Position))
                    .First();

                worldState.HasTarget = true;
                worldState.TargetPosition = closestEnemy.Body.Position;
                worldState.TargetForward = closestEnemy.Body.Forward;
                worldState.DistanceToTarget = Vector3.Distance(character.Body.Position, closestEnemy.Body.Position);
            }
            else
            {
                worldState.HasTarget = false;
                worldState.TargetPosition = Vector3.zero;
                worldState.DistanceToTarget = float.MaxValue;
            }

            // Update attack readiness from character
            worldState.IsReadyToAttack = character.CanAttack;
        }
    }
}