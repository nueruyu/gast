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
        public bool IsOutOfOuterTerritory { get; private set; }
        public bool IsOutOfInnerTerritory { get; private set; }

        public void WriteTo(ref StrategicState dest)
        {
            dest ??= new();
            dest.CurrentMode = CurrentMode;
            dest.HasGatheringTarget = HasGatheringTarget;
            dest.HasObjectiveCombatTarget = HasObjectiveCombatTarget;
            dest.IsThreatened = IsThreatened;
            dest.IsOutOfOuterTerritory = IsOutOfOuterTerritory;
            dest.IsOutOfInnerTerritory = IsOutOfInnerTerritory;
        }

        public void Update(
            AIMode currentMode,
            bool isThreatened,
            bool hasGatheringTarget,
            bool hasObjectiveCombatTarget,
            bool isOutOfOuterTerritory,
            bool isOutOfInnerTerritory)
        {
            CurrentMode = currentMode;
            IsThreatened = isThreatened;
            HasGatheringTarget = hasGatheringTarget;
            HasObjectiveCombatTarget = hasObjectiveCombatTarget;
            IsOutOfOuterTerritory = isOutOfOuterTerritory;
            IsOutOfInnerTerritory = isOutOfInnerTerritory;
        }
    }
}
