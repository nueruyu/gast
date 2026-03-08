using Cryst.Features.CharacterAI.Actions;
using Cryst.Features.CharacterAI.Humanoid.Strategic.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Cryst.Features.CharacterAI.Humanoid.Strategic
{
    public class StrategicDomainFactory : IAIDomainFactory<StrategicState>
    {
        readonly AIDomain<ActorContext<StrategicState>, StrategicState> domain;

        public StrategicDomainFactory()
        {
            var builder = new AIDomainBuilder<ActorContext<StrategicState>, StrategicState>();

            var root = builder.DefineCompound("Root");

            root.AddMethod("RespondToThreat")
                .Condition(s => s.IsThreatened)
                .Do(new SelectThreatAction());
            root.AddMethod("PursueObjective")
                .Condition(s => !s.IsThreatened && s.AvailableObjectives.Count > 0)
                .Do(new SelectObjectiveAction());
            root.AddMethod("Idle")
                .Condition(s => !s.IsThreatened && s.AvailableObjectives.Count == 0)
                .Do(new ClearTargetAction())
                .Do(new WaitAction(1.0f));

            domain = builder.Build("Root");
        }

        public AIDomain<ActorContext<StrategicState>, StrategicState> CreateDomain()
        {
            return domain;
        }
    }
}
