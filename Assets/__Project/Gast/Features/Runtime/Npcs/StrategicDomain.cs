using Gast.Api.AI.Goals;
using Gast.Features.Npcs.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Gast.Features.Npcs
{
    public static class StrategicDomain
    {
        public static AIDomain<StrategicWorldState, AIContext<StrategicWorldState>> Create(
            FindTargetForGoalAction findTargetForGoalAction,
            SelectThreatAction selectThreatAction,
            ClearTargetAction clearTargetAction,
            FindItemPickupAction findItemPickupAction,
            MoveToInteractableAction moveToInteractableAction,
            InteractWithTargetAction interactWithTargetAction,
            ClearInteractableTargetAction clearInteractableTargetAction)
        {
            return new AIDomainBuilder<StrategicWorldState, AIContext<StrategicWorldState>>()
                .RegisterAction(findTargetForGoalAction)
                .RegisterAction(selectThreatAction)
                .RegisterAction(clearTargetAction)
                .RegisterAction(findItemPickupAction)
                .RegisterAction(moveToInteractableAction)
                .RegisterAction(interactWithTargetAction)
                .RegisterAction(clearInteractableTargetAction)
                .DefineCompound("AcquireItem")
                    .AddMethod("FindAndCollect")
                        .Do(findItemPickupAction, moveToInteractableAction, interactWithTargetAction)
                    .End()
                    .AddMethod("ClearTargetIfNotFound")
                        .Do(clearInteractableTargetAction)
                    .End()
                .End()
                .DefineCompound("Root")
                    .AddMethod("SelectClosestThreat")
                        .Condition(s => s.IsThreatened)
                        .Do(selectThreatAction)
                    .End()
                    .AddMethod("AcquireItemGoal")
                        .Condition(s => s.HasGoal && s.CurrentGoal is AcquireItemGoal)
                        .Do("AcquireItem")
                    .End()
                    .AddMethod("SelectTargetBasedOnGoal")
                        .Condition(s => s.HasGoal)
                        .Do(findTargetForGoalAction)
                    .End()
                    .AddMethod("Idle")
                        .Do(clearTargetAction)
                    .End()
                .End()
                .Build("Root");
        }
    }
}