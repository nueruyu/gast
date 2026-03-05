using Cryst.Domain.AI.Objectives;
using Cryst.Features.CharacterAI.Gathering.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Cryst.Features.CharacterAI.Gathering
{
    public class GatheringDomain
    {
        readonly AIDomain<GatheringState, AIContext<GatheringState>> domain;

        public GatheringDomain()
        {
            var builder = new AIDomainBuilder<GatheringState, AIContext<GatheringState>>();

            var findItemPickup = builder.RegisterAction("FindItemPickup", new FindItemPickupAction());
            var moveToInteractable = builder.RegisterAction("MoveToInteractable", new MoveToInteractableAction());
            var interactWithTarget = builder.RegisterAction("InteractWithTarget", new InteractWithTargetAction());
            var clearInteractableTarget = builder.RegisterAction("ClearInteractableTarget", new ClearInteractableTargetAction());
            var wait = builder.RegisterAction<float>("Wait", new WaitAction());

            var acquireItem = builder.DefineCompound("AcquireItem", c =>
            {
                c.AddMethod("FindAndCollect")
                    .Do(findItemPickup)
                    .Do(moveToInteractable)
                    .Do(wait, 0.3f)
                    .Do(interactWithTarget)
                    .End();
                c.AddMethod("ClearTargetIfNotFound")
                    .Do(clearInteractableTarget)
                    .End();
            });

            builder.DefineCompound("Root", c =>
            {
                c.AddMethod("AcquireItemGoal")
                    .Condition(s => !s.IsInCombat && s.HasGoal && s.CurrentGoal is AcquireItemObjective)
                    .Do(acquireItem)
                    .End();
                c.AddMethod("Idle")
                    .Do(wait, 0.5f)
                    .End();
            });

            domain = builder.Build("Root");
        }

        public AIRunner<GatheringState, AIContext<GatheringState>> CreateRunner()
        {
            return domain.CreateRunner();
        }
    }
}
