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
        public bool HasActiveObjective { get; private set; }

        public void SetActiveObjective()
        {
            HasActiveObjective = true;
        }

        public void WriteTo(ref ObjectiveState dest)
        {
            dest ??= new();
            dest.Queries = Queries;
            dest.Self = Self;
            dest.HasActiveObjective = HasActiveObjective;
        }

        public void Update(ActorContext<ObjectiveState> context)
        {
            var actor = context.Actor;
            var memory = context.GetModule<HumanoidMemory>();

            Queries = context.ObjectiveQueries;
            Self = new ActorInfo(actor.Id, actor.TypeId, actor.Body.Position, actor.Faction,
                actor.Status.IsAlive.Value);
            HasActiveObjective = memory.CurrentObjective != null;
        }

        record ActorInfo(
            CharacterId Id,
            CharacterTypeId TypeId,
            Vector3 Position,
            Faction Faction,
            bool IsAlive) : IActorInfo;
    }
}
