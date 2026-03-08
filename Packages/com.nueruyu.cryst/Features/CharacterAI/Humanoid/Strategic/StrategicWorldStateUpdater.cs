using System.Linq;
using Cryst.Domain.Characters;
using Gast.Domain.Characters;

namespace Cryst.Features.CharacterAI.Humanoid.Strategic
{
    public class StrategicWorldStateUpdater : IWorldStateUpdater<StrategicState>
    {
        public void Update(ActorContext<StrategicState> context)
        {
            var actor = context.Actor;
            var state = context.WorldState;

            state.AvailableObjectives = context.ObjectiveManager.CurrentObjectives
                .Where(o => !o.IsCompleted.Value)
                .ToList();

            state.IsThreatened = actor.VisionSensor.VisibleCharacters
                .Select(c => c.As<BaseCharacter>())
                .Any(otherActor => otherActor.IsThreatTo(actor));
        }
    }
}
