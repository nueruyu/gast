using System.Threading;
using Cryst.Domain.Characters.Facets;
using Cryst.Features.CharacterAI.Common;
using Cryst.Features.CharacterAI.Humanoid;
using Cryst.Features.CharacterAI.Humanoid.Patrol;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Actions
{
    public class ReturnToHomeAction : IAction<ActorContext<PatrolState>, PatrolState>
    {
        public bool IsAvailable(PatrolState worldState)
        {
            return worldState.IsOutOfTerritory;
        }

        public void Simulate(PatrolState worldState)
        {
            worldState.IsOutOfTerritory = false;
        }

        public async UniTask ExecuteAsync(ActorContext<PatrolState> context, CancellationToken cancellationToken)
        {
            if (!context.Character.Is(out TerritorialCharacter territorialCharacter))
                return;

            if (context.Character.Is(out SprintableCharacter sprintable))
                sprintable.SetSprint(true);

            try
            {
                await context.Actor.MoveToAsync(
                    static c => c.Territory.HomePosition,
                    static c => c.HasReturnedToTerritory(),
                    territorialCharacter,
                    cancellationToken);
            }
            finally
            {
                sprintable?.SetSprint(false);
            }
        }
    }
}