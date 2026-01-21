using Gast.Features.AI.Strategic.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Gast.Features.AI.Strategic
{
    public class StrategicDomain
    {
        readonly AIDomain<StrategicState, AIContext<StrategicState>> domain;

        public StrategicDomain(
            FindTargetForGoalAction findTargetForGoalAction,
            SelectThreatAction selectThreatAction,
            ClearTargetAction clearTargetAction)
        {
            domain = new AIDomainBuilder<StrategicState, AIContext<StrategicState>>()
                .RegisterAction("FindTargetForGoal", findTargetForGoalAction)
                .RegisterAction("SelectThreat", selectThreatAction)
                .RegisterAction("ClearTarget", clearTargetAction)
                .RegisterAction("Idle", new IdleAction())
                .RegisterAction("Wait", new WaitAction())
                .DefineCompound("Root")
                    .AddMethod("SelectClosestThreat")
                        .Condition(s => s.IsThreatened)
                        .Do("SelectThreat")
                    .End()
                    .AddMethod("SelectTargetBasedOnGoal")
                        .Condition(s => s.HasGoal)
                        .Do("FindTargetForGoal")
                    .End()
                    .AddMethod("Thinking")
                        .Do("Wait", 2f)
                    .End()
                    .AddMethod("Idle")
                        .Do("ClearTarget", "Idle")
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