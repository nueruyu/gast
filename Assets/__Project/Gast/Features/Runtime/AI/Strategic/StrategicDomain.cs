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
                .RegisterTask("FindTargetForGoal", findTargetForGoalAction)
                .RegisterTask("SelectThreat", selectThreatAction)
                .RegisterTask("ClearTarget", clearTargetAction)
                .RegisterTask("Idle", new IdleAction())
                .RegisterTask("Interval", new WaitAction(2))
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
                        .Do("Interval")
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
