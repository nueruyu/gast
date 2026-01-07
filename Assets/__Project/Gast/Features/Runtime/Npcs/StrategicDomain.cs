using Gast.Features.Npcs.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Gast.Features.Npcs
{
    public static class StrategicDomain
    {
        public static Domain<StrategicWorldState> Create(
            FindTargetForGoalAction findTargetForGoalAction,
            FindThreatAction findThreatAction,
            ClearTargetAction clearTargetAction)
        {
            return new DomainBuilder<StrategicWorldState>()
                .RegisterTask(findTargetForGoalAction)
                .RegisterTask(findThreatAction)
                .RegisterTask(clearTargetAction)
                .DefineRoot()
                    .AddMethod("SelectTargetBasedOnGoal")
                        .Condition(s => s.HasGoal)
                        .Do(findTargetForGoalAction)
                    .End()
                    .AddMethod("SelectClosestThreat")
                        .Condition(s => !s.HasGoal)
                        .Do(findThreatAction)
                    .End()
                    .AddMethod("Idle")
                        .Do(clearTargetAction)
                    .End()
                .End()
                .Build();
        }
    }
}
