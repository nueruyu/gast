using Cryst.Domain.AI.Objectives;
using Cryst.Features.CharacterAI.Gathering.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Cryst.Features.CharacterAI.Gathering
{
    public class GatheringDomain
    {
        readonly AIDomain<ActorContext<GatheringState>, GatheringState> domain;

        public GatheringDomain()
        {
            var builder = new AIDomainBuilder<ActorContext<GatheringState>, GatheringState>();

            var acquireItem = builder.DefineCompound("AcquireItem");

            acquireItem.AddMethod("FindAndCollect")
                .Do(new FindItemPickupAction())
                .Do(new MoveToInteractableAction())
                .Do(new WaitAction(0.3f))
                .Do(new InteractWithTargetAction());
            acquireItem.AddMethod("ClearTargetIfNotFound")
                .Do(new ClearInteractableTargetAction());

            var root = builder.DefineCompound("Root");

            root.AddMethod("AcquireItemGoal")
                .Condition(s => !s.IsInCombat && s.HasGoal && s.CurrentGoal is AcquireItemObjective)
                .Do(acquireItem);
            root.AddMethod("Idle")
                .Do(new WaitAction(0.5f));

            domain = builder.Build("Root");
        }

        public AIRunner<ActorContext<GatheringState>, GatheringState> CreateRunner()
        {
            return domain.CreateRunner();
        }
    }
}
