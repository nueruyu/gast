using Cryst.Domain.Characters.Facets;

namespace Cryst.Features.CharacterAI.Humanoid.Patrol
{
    public class PatrolWorldStateUpdater : IWorldStateUpdater<PatrolState>
    {
        public void Update(ActorContext<PatrolState> context)
        {
            var aiModeMemory = context.GetModule<AIModeMemory>();
            var character = context.Character;

            var isActive = aiModeMemory.CurrentMode == AIMode.ReturningToHome;
            var isOutOfTerritory = false;

            if (character.Is(out TerritorialCharacter territorial))
            {
                // Use the existing state to decide which condition to check.
                isOutOfTerritory = context.WorldState.IsOutOfTerritory
                    ? !territorial.HasReturnedToTerritory()
                    : territorial.IsOutOfTerritory();
            }

            context.WorldState.Update(isActive, isOutOfTerritory);
        }
    }
}
