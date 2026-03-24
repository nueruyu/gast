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

            bool isOutOfTerritory = false;
            bool isOutOfTerritoryCore = false;
            if (character.Is(out TerritorialCharacter territorial))
            {
                isOutOfTerritory = territorial.IsOutOfTerritory();
                isOutOfTerritoryCore = !territorial.HasReturnedToTerritory();
            }

            context.WorldState.Update(
                currentMode: aiModeMemory.CurrentMode,
                isThreatened: combatMemory.IsThreatened,
                hasGatheringTarget: gatheringMemory.HasGatheringTarget,
                hasObjectiveCombatTarget: combatMemory.HasObjectiveCombatTarget,
                isOutOfTerritory: isOutOfTerritory,
                isOutOfTerritoryCore: isOutOfTerritoryCore);
        }
    }
}
