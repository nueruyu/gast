using Cryst.Domain.Characters.Facets;

namespace Cryst.Features.CharacterAI.Humanoid.Strategic
{
    public class StrategicWorldStateUpdater : IWorldStateUpdater<StrategicState>
    {
        public void Update(ActorContext<StrategicState> context)
        {
            var character = context.Character;
            var aiModeMemory = context.GetModule<AIModeMemory>();
            var combatMemory = context.GetModule<CombatMemory>();
            var gatheringMemory = context.GetModule<GatheringMemory>();

            var isOutOfOuterTerritory = false;
            var isOutOfInnerTerritory = false;
            if (character.Is(out TerritorialCharacter territorial))
            {
                isOutOfOuterTerritory = territorial.IsOutOfTerritory();
                isOutOfInnerTerritory = !territorial.HasReturnedToTerritory();
            }

            context.WorldState.Update(
                aiModeMemory.CurrentMode,
                combatMemory.IsThreatened,
                gatheringMemory.HasGatheringTarget,
                combatMemory.HasObjectiveCombatTarget,
                isOutOfOuterTerritory,
                isOutOfInnerTerritory);
        }
    }
}