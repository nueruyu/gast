using Cryst.Domain.Characters;
using Gast.Domain.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Objective
{
    public class ObjectiveWorldStateUpdater : IWorldStateUpdater<ObjectiveState>
    {
        public void Update(ActorContext<ObjectiveState> context)
        {
            var actor = context.Actor;

            var selfInfo = new ActorInfo(
                actor.Id,
                actor.TypeId,
                actor.Body.Position,
                actor.Faction,
                actor.Status.IsAlive.Value);

            context.WorldState.Update(context.ObjectiveQueries, selfInfo);
        }

        record ActorInfo(
            CharacterId Id,
            CharacterTypeId TypeId,
            Vector3 Position,
            Faction Faction,
            bool IsAlive) : IActorInfo;
    }
}
