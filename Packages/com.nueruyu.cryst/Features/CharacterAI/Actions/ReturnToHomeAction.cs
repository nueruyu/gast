using System.Threading;
using Cryst.Domain.Characters.Facets;
using Cryst.Features.CharacterAI.Common;
using Cryst.Features.CharacterAI.Humanoid;
using Cryst.Features.CharacterAI.Humanoid.Strategic;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Actions
{
    public class ReturnToHomeAction : IAction<ActorContext<StrategicState>, StrategicState>
    {
        public bool IsAvailable(StrategicState worldState)
        {
            return worldState.IsOutOfTerritory;
        }

        public void Simulate(StrategicState worldState)
        {
            worldState.IsOutOfTerritory = false;
        }

        public async UniTask ExecuteAsync(ActorContext<StrategicState> context, CancellationToken cancellationToken)
        {
            if (!context.Character.Is(out TerritorialCharacter territorialCharacter))
                return;

            // Clear any targets before returning
            var memory = context.GetModule<HumanoidMemory>();
            memory.CombatTarget = null;
            memory.CurrentObjective = null;
            memory.InteractableTarget = null;

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