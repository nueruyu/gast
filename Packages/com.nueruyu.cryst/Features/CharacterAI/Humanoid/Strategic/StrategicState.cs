using System.Linq;
using Cryst.Domain.AI.Objectives;
using Cryst.Domain.Characters;
using Cryst.Domain.Characters.Facets;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Strategic
{
    public class StrategicState : IWorldState<StrategicState>
    {
        public AIMode CurrentMode { get; private set; }
        public bool HasCombatObjective { get; private set; }
        public bool HasGatheringObjective { get; private set; }
        public bool HasCombatTarget { get; private set; }
        public bool IsThreatened { get; private set; }
        public bool IsOutOfTerritory { get; private set; }
        public bool IsOutOfTerritoryCore { get; private set; }

        public void WriteTo(ref StrategicState dest)
        {
            dest ??= new();
            dest.CurrentMode = CurrentMode;
            dest.HasCombatObjective = HasCombatObjective;
            dest.HasGatheringObjective = HasGatheringObjective;
            dest.HasCombatTarget = HasCombatTarget;
            dest.IsThreatened = IsThreatened;
            dest.IsOutOfTerritory = IsOutOfTerritory;
            dest.IsOutOfTerritoryCore = IsOutOfTerritoryCore;
        }

        public void Update(ActorContext<StrategicState> context)
        {
            var character = context.Character;
            var actor = context.Actor;
            var memory = context.GetModule<HumanoidMemory>();

            CurrentMode = memory.CurrentMode;
            HasCombatObjective = memory.CurrentObjective is DefeatCharacterObjective;
            HasGatheringObjective = memory.CurrentObjective is AcquireItemObjective;
            HasCombatTarget = memory.HasTarget;

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

            IsThreatened = actor.VisionSensor.VisibleCharacters
                .Select(c => c.As<BaseCharacter>())
                .Any(otherActor => otherActor.IsThreatTo(actor));
        }
    }
}
