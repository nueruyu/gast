using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Cryst.Domain.Characters.Facets;
using Gast.Lib.AI;
using ActorContext_ = Cryst.Features.CharacterAI.ActorContext<Cryst.Features.CharacterAI.Combat.CombatState>;

namespace Cryst.Features.CharacterAI.Combat.Actions
{
    [Serializable]
    public class ChaseTargetAction : IAction<ActorContext_, CombatState>
    {
        public bool CanExecute(CombatState worldState)
        {
            return worldState.HasTarget && !worldState.IsInAttackRange;
        }

        public void Simulate(CombatState worldState)
        {
            worldState.DistanceToTarget = worldState.AttackRange;
        }

        public async UniTask ExecuteAsync(ActorContext_ context, CancellationToken cancellationToken)
        {
            var actor = context.Actor;
            if (context.Character.Is(out SprintableCharacter sprintable))
            {
                sprintable.SetSprint(true);
            }

            var navigator = actor.NavigationProvider;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var worldState = context.WorldState;
                    navigator.SetDestination(worldState.TargetPosition);

                    var selfToTarget = worldState.TargetPosition - actor.Body.Position;
                    selfToTarget.y = 0;

                    var currentDist = selfToTarget.magnitude;
                    if (currentDist <= worldState.AttackRange)
                    {
                        navigator.Stop();
                        if (context.Character.Is(out SprintableCharacter sprintableOnExit))
                        {
                            sprintableOnExit.SetSprint(false);
                        }
                        return;
                    }

                    var direction = navigator.NextSteeringDirection;
                    if (direction != Vector3.zero)
                    {
                        actor.Move(direction);
                    }

                    await UniTask.Yield(cancellationToken);
                }
            }
            finally
            {
                if (context.Character.Is(out SprintableCharacter sprintableOnFinally))
                {
                    sprintableOnFinally.SetSprint(false);
                }
                navigator.Stop();
            }
        }
    }
}
