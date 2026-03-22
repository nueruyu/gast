using Cryst.Features.CharacterAI.Actions;
using Cryst.Features.CharacterAI.Humanoid.Patrol.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Cryst.Features.CharacterAI.Humanoid.Patrol
{
    public class PatrolDomainFactory : IAIDomainFactory<PatrolState>
    {
        readonly AIDomain<ActorContext<PatrolState>, PatrolState> domain;

        public PatrolDomainFactory()
        {
            var builder = new AIDomainBuilder<ActorContext<PatrolState>, PatrolState>();

            var root = builder.DefineCompound("Root");

            root.AddMethod("ExecuteReturnToHome")
                .While(s => s.IsActive)
                .Do(new ReturnToHomeAction());
            root.AddMethod("Idle")
                .Do(new IdleAction());

            domain = builder.Build("Root");
        }

        public AIDomain<ActorContext<PatrolState>, PatrolState> GetDomain()
        {
            return domain;
        }
    }
}