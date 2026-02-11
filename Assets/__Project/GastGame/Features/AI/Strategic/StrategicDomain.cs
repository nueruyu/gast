using GastGame.Features.AI.Strategic.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace GastGame.Features.AI.Strategic
{
    public class StrategicDomain
    {
        readonly AIDomain<StrategicState, AIContext<StrategicState>> domain;

        public StrategicDomain(
            SelectObjectiveAction selectObjectiveAction,
            SelectThreatAction selectThreatAction,
            ClearTargetAction clearTargetAction)
        {
            domain = new AIDomainBuilder<StrategicState, AIContext<StrategicState>>()
                .RegisterAction("SelectObjective", selectObjectiveAction)
                .RegisterAction("SelectThreat", selectThreatAction)
                .RegisterAction("ClearTarget", clearTargetAction)
                .RegisterAction("Wait", new WaitAction())
                .DefineCompound("Root")
                    .AddMethod("RespondToThreat")
                        .Condition(s => s.IsThreatened)
                        .Do("SelectThreat")
                    .End()
                    .AddMethod("PursueObjective")
                        .Condition(s => !s.IsThreatened && s.AvailableObjectives.Count > 0)
                        .Do("SelectObjective")
                    .End()
                    .AddMethod("Idle")
                        .Condition(s => !s.IsThreatened && s.AvailableObjectives.Count == 0)
                        .Do("ClearTarget")
                        .Do("Wait", 1.0f)
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