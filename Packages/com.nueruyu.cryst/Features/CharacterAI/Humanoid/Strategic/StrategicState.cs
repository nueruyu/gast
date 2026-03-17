using System.Collections.Generic;
using System.Linq;
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
        public bool IsThreatened { get; private set; }
        public bool IsOutOfTerritory { get; private set; }
        public bool IsOutOfTerritoryCore { get; private set; }
        public List<IAIObjective> AvailableObjectives { get; private set; } = new();

        public void WriteTo(ref StrategicState dest)
        {
            dest ??= new();
            dest.CurrentMode = CurrentMode;
            dest.IsThreatened = IsThreatened;
            dest.IsOutOfTerritory = IsOutOfTerritory;
            dest.IsOutOfTerritoryCore = IsOutOfTerritoryCore;
            dest.AvailableObjectives = AvailableObjectives;
        }

        public void Update(ActorContext<StrategicState> context)
        {
            var character = context.Character;
            var actor = context.Actor;
            var memory = context.GetModule<HumanoidMemory>();

            CurrentMode = memory.CurrentMode;
            if (CurrentMode == AIMode.Combat)
                if (!memory.HasTarget)
                    CurrentMode = AIMode.Idle;

            AvailableObjectives = context.ObjectiveManager.CurrentObjectives
                .Where(o => !o.IsCompleted.Value)
                .ToList();

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