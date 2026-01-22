using Gast.Domain.AI.Goals;
using Gast.Features.AI.Gathering.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Gast.Features.AI.Gathering
{
    public class GatheringDomain
    {
        readonly AIDomain<GatheringState, AIContext<GatheringState>> domain;

        public GatheringDomain(
            FindItemPickupAction findItemPickupAction,
            MoveToInteractableAction moveToInteractableAction,
            InteractWithTargetAction interactWithTargetAction,
            ClearInteractableTargetAction clearInteractableTargetAction)
        {
            domain = new AIDomainBuilder<GatheringState, AIContext<GatheringState>>()
                .RegisterAction("FindItemPickup", findItemPickupAction)
                .RegisterAction("MoveToInteractable", moveToInteractableAction)
                .RegisterAction("InteractWithTarget", interactWithTargetAction)
                .RegisterAction("ClearInteractableTarget", clearInteractableTargetAction)
                .RegisterAction("Wait", new WaitAction())
                .DefineCompound("AcquireItem")
                    .AddMethod("FindAndCollect")
                        .Do("FindItemPickup", "MoveToInteractable", "InteractWithTarget")
                    .End()
                    .AddMethod("ClearTargetIfNotFound")
                        .Do("ClearInteractableTarget")
                    .End()
                .End()
                .DefineCompound("Root")
                    .AddMethod("AcquireItemGoal")
                        .Condition(s => !s.IsInCombat && s.HasGoal && s.CurrentGoal is AcquireItemGoal)
                        .Do("AcquireItem")
                    .End()
                    .AddMethod("Idle")
                        .Do("Wait", 0.5f)
                    .End()
                .End()
                .Build("Root");
        }

        public AIRunner<GatheringState, AIContext<GatheringState>> CreateRunner()
        {
            return domain.CreateRunner();
        }
    }
}