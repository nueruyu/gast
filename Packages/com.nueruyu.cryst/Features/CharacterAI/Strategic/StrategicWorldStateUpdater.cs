using System.Linq;
using Cryst.Domain.Characters;
using Gast.Domain.Characters;

namespace Cryst.Features.CharacterAI.Strategic
{
    public class StrategicWorldStateUpdater : IWorldStateUpdater<StrategicState>
    {
        public void Update(ActorContext<StrategicState> context)
        {
            var actor = context.Actor;
            var strategicState = context.WorldState;

            strategicState.AvailableObjectives = context.ObjectiveManager.CurrentObjectives
                .Where(o => !o.IsCompleted.Value)
                .ToList();

            strategicState.IsThreatened = actor.VisionSensor.VisibleCharacters
                .Select(c => c.As<BaseCharacter>())
                .Any(otherActor => otherActor.IsThreatTo(actor));
        }
    }
}
