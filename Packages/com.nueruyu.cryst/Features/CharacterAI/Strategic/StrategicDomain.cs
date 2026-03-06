using Cryst.Features.CharacterAI.Strategic.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Cryst.Features.CharacterAI.Strategic
{
    public class StrategicDomain
    {
        readonly AIDomain<ActorContext<StrategicState>, StrategicState> domain;

        public StrategicDomain()
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

        public AIRunner<ActorContext<StrategicState>, StrategicState> CreateRunner()
        {
            return domain.CreateRunner();
        }
    }
}
