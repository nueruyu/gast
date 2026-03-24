using Cryst.Domain.Characters;
using Gast.Domain.Characters;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Objective
{
    public class ObjectiveState : IWorldState<ObjectiveState>
    {
        public IObjectiveQueries Queries { get; private set; }
        public IActorInfo Self { get; private set; }

        public void WriteTo(ref ObjectiveState dest)
        {
            dest ??= new();
            dest.Queries = Queries;
            dest.Self = Self;
        }

        public void Update(IObjectiveQueries queries, IActorInfo self)
        {
            Queries = queries;
            Self = self;
        }

        record ActorInfo(
            CharacterId Id,
            CharacterTypeId TypeId,
            Vector3 Position,
            Faction Faction,
            bool IsAlive) : IActorInfo;
    }
}
