using System.Linq;
using Cryst.Domain.Characters;
using Cryst.Domain.Characters.Facets;
using Gast.Domain.Characters;

namespace Cryst.Features.CharacterAI.Humanoid.Strategic
{
    public class StrategicWorldStateUpdater : IWorldStateUpdater<StrategicState>
    {
        public void Update(ActorContext<StrategicState> context)
        {
            var character = context.Character;
            var actor = context.Actor;
            var state = context.WorldState;

            state.AvailableObjectives = context.ObjectiveManager.CurrentObjectives
                .Where(o => !o.IsCompleted.Value)
                .ToList();

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

            state.IsThreatened = actor.VisionSensor.VisibleCharacters
                .Select(c => c.As<BaseCharacter>())
                .Any(otherActor => otherActor.IsThreatTo(actor));
        }
    }
}