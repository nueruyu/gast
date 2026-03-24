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

        public void Update(
            AIMode currentMode,
            bool isThreatened,
            bool hasGatheringTarget,
            bool hasObjectiveCombatTarget,
            bool isOutOfTerritory,
            bool isOutOfTerritoryCore)
        {
            CurrentMode = currentMode;
            IsThreatened = isThreatened;
            HasGatheringTarget = hasGatheringTarget;
            HasObjectiveCombatTarget = hasObjectiveCombatTarget;
            IsOutOfTerritory = isOutOfTerritory;
            IsOutOfTerritoryCore = isOutOfTerritoryCore;
        }
    }
}
