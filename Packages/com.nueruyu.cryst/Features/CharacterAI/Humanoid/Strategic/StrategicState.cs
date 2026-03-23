using Cryst.Domain.AI.Objectives;
using Cryst.Domain.Characters.Facets;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Strategic
{
    public class StrategicState : IWorldState<StrategicState>
    {
        public AIMode CurrentMode { get; private set; }
        public bool HasGatheringTarget { get; private set; }
        public bool HasObjectiveCombatTarget { get; private set; }
        public bool IsThreatened { get; private set; }
        public bool IsOutOfTerritory { get; private set; }
        public bool IsOutOfTerritoryCore { get; private set; }

        public void WriteTo(ref StrategicState dest)
        {
            dest ??= new();
            dest.CurrentMode = CurrentMode;
            dest.HasGatheringTarget = HasGatheringTarget;
            dest.HasObjectiveCombatTarget = HasObjectiveCombatTarget;
            dest.IsThreatened = IsThreatened;
            dest.IsOutOfTerritory = IsOutOfTerritory;
            dest.IsOutOfTerritoryCore = IsOutOfTerritoryCore;
        }

        public void Update(ActorContext<StrategicState> context)
        {
            var character = context.Character;
            var aiModeMemory = context.GetModule<AIModeMemory>();
            var combatMemory = context.GetModule<CombatMemory>();
            var gatheringMemory = context.GetModule<GatheringMemory>();

            IsThreatened = combatMemory.IsThreatened;
            CurrentMode = aiModeMemory.CurrentMode;
            HasGatheringTarget = gatheringMemory.HasGatheringTarget;
            HasObjectiveCombatTarget = combatMemory.HasObjectiveCombatTarget;

            if (character.Is(out TerritorialCharacter territorial))
            {
                IsOutOfTerritory = territorial.IsOutOfTerritory();
                IsOutOfTerritoryCore = !territorial.HasReturnedToTerritory();
            }
            else
            {
                IsOutOfTerritory = false;
                IsOutOfTerritoryCore = false;
            }
        }
    }
}