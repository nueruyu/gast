using Cryst.Domain.Characters.Facets;

namespace Cryst.Features.CharacterAI.Humanoid.Patrol
{
    public class PatrolWorldStateUpdater : IWorldStateUpdater<PatrolState>
    {
        public void Update(ActorContext<PatrolState> context)
        {
            var memory = context.GetModule<HumanoidMemory>();
            var character = context.Character;
            var state = context.WorldState;

            state.CurrentMode = memory.CurrentMode;

            if (character.Is(out TerritorialCharacter territorial))
            {
                if (state.IsOutOfTerritory)
                    state.IsOutOfTerritory = !territorial.HasReturnedToTerritory();
                else
                    state.IsOutOfTerritory = territorial.IsOutOfTerritory();
            }
            else
            {
                state.IsOutOfTerritory = false;
            }
        }
    }
}
