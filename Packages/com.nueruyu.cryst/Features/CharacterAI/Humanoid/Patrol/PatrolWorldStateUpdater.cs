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
            var isOutOfOuterTerritory = false;
            var isOutOfInnerTerritory = false;

            if (character.Is(out TerritorialCharacter territorial))
            {
                isOutOfOuterTerritory = territorial.IsOutOfTerritory();
                isOutOfInnerTerritory = !territorial.HasReturnedToTerritory();
            }

            context.WorldState.Update(isActive, isOutOfOuterTerritory, isOutOfInnerTerritory);
        }
    }
}