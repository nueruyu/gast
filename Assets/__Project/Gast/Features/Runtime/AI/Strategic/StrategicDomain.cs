using Gast.Domain.AI.Goals;
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
            ClearTargetAction clearTargetAction,
            FindItemPickupAction findItemPickupAction,
            MoveToInteractableAction moveToInteractableAction,
            InteractWithTargetAction interactWithTargetAction,
            ClearInteractableTargetAction clearInteractableTargetAction)
        {
            domain = new AIDomainBuilder<StrategicState, AIContext<StrategicState>>()
                .RegisterTask("FindTargetForGoal", findTargetForGoalAction)
                .RegisterTask("SelectThreat", selectThreatAction)
                .RegisterTask("ClearTarget", clearTargetAction)
                .RegisterTask("FindItemPickup", findItemPickupAction)
                .RegisterTask("MoveToInteractable", moveToInteractableAction)
                .RegisterTask("InteractWithTarget", interactWithTargetAction)
                .RegisterTask("ClearInteractableTarget", clearInteractableTargetAction)
                .RegisterTask("Idle", new IdleAction())
                .RegisterTask("Interval", new WaitAction(2))
                .DefineCompound("AcquireItem")
                    .AddMethod("FindAndCollect")
                        .Do("FindItemPickup", "MoveToInteractable", "InteractWithTarget")
                    .End()
                    .AddMethod("ClearTargetIfNotFound")
                        .Do("ClearInteractableTarget")
                    .End()
                .End()
                .DefineCompound("Root")
                    .AddMethod("SelectClosestThreat")
                        .Condition(s => s.IsThreatened)
                        .Do("SelectThreat")
                    .End()
                    .AddMethod("AcquireItemGoal")
                        .Condition(s => s.HasGoal && s.CurrentGoal is AcquireItemGoal)
                        .Do("AcquireItem")
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