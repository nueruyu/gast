using Gast.Api.AI.Goals;
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
            ClearTargetAction clearTargetAction,
            FindItemPickupAction findItemPickupAction,
            MoveToInteractableAction moveToInteractableAction,
            InteractWithTargetAction interactWithTargetAction,
            ClearInteractableTargetAction clearInteractableTargetAction)
        {
            return new DomainBuilder<StrategicWorldState>()
                .RegisterTask(findTargetForGoalAction)
                .RegisterTask(findThreatAction)
                .RegisterTask(clearTargetAction)
                .RegisterTask(findItemPickupAction)
                .RegisterTask(moveToInteractableAction)
                .RegisterTask(interactWithTargetAction)
                .RegisterTask(clearInteractableTargetAction)
                .DefineCompound("AcquireItem")
                    .AddMethod("FindAndCollect")
                        .Do(findItemPickupAction, moveToInteractableAction, interactWithTargetAction)
                    .End()
                    .AddMethod("ClearTargetIfNotFound")
                        .Do(clearInteractableTargetAction)
                    .End()
                .End()
                .DefineRoot()
                    .AddMethod("AcquireItemGoal")
                        .Condition(s => s.HasGoal && s.CurrentGoal is AcquireItemGoal)
                        .Do("AcquireItem")
                    .End()
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
