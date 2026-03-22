using Cryst.Features.CharacterAI.Actions;
using Cryst.Features.CharacterAI.Humanoid.Gathering.Actions;
using Gast.Core.Values;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Cryst.Features.CharacterAI.Humanoid.Gathering
{
    public class GatheringDomainFactory : IAIDomainFactory<GatheringState>
    {
        readonly AIDomain<ActorContext<GatheringState>, GatheringState> domain;

        public GatheringDomainFactory()
        {
            var builder = new AIDomainBuilder<ActorContext<GatheringState>, GatheringState>();

            var acquireItem = builder.DefineCompound("AcquireItem");

            acquireItem.AddMethod("FindAndCollect")
                .Do(new FindItemPickupAction())
                .Do(new MoveToInteractableAction())
                .Do(new WaitAction(new FloatRange(0.2f, 0.4f)))
                .Do(new InteractWithTargetAction());

            var root = builder.DefineCompound("Root");

            root.AddMethod("AcquireItemGoal")
                .While(s => s.IsActive)
                .Do(acquireItem);
            root.AddMethod("Idle")
                .Do(new WaitAction(0.5f));

            domain = builder.Build("Root");
        }

        public AIDomain<ActorContext<GatheringState>, GatheringState> GetDomain()
        {
            return domain;
        }
    }
}