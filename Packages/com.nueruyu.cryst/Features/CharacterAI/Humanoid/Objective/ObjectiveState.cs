using Gast.Lib.AI;

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
    }
}
