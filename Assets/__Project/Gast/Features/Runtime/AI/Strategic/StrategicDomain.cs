using Gast.Features.AI.Strategic.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Gast.Features.AI.Strategic
{
    public class StrategicDomain
    {
        readonly AIDomain<StrategicState, AIContext<StrategicState>> domain;

        public StrategicDomain(
            SelectObjectiveAction selectObjectiveAction,
            SelectThreatAction selectThreatAction)
        {
            domain = new AIDomainBuilder<StrategicState, AIContext<StrategicState>>()
                .RegisterAction("SelectObjective", selectObjectiveAction)
                .RegisterAction("SelectThreat", selectThreatAction)
                .RegisterAction("Idle", new IdleAction())
                .RegisterAction("Wait", new WaitAction())
                .DefineCompound("Root")
                    .AddMethod("RespondToThreat")
                        .Condition(s => s.IsThreatened)
                        .Do("SelectThreat")
                    .End()
                    .AddMethod("PursueObjective")
                        .Condition(s => s.AvailableObjectives.Count > 0)
                        .Do("SelectObjective")
                    .End()
                    .AddMethod("Idle")
                        .Do("Idle")
                    .End()
                .End()
                .Build("Root");
        }

        public AIRunner<StrategicState, AIContext<StrategicState>> CreateRunner()
        {
            return domain.CreateRunner();
        }
    }
}