using Gast.Core.DI;
using Gast.Features.AI.Gathering;
using Gast.Features.AI.Gathering.Actions;

namespace Gast.Features.Installers
{
    public class GatheringAIInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<GatheringDomain>(Lifetime.Transient);
            builder.Register<FindItemPickupAction>(Lifetime.Transient);
            builder.Register<MoveToInteractableAction>(Lifetime.Transient);
            builder.Register<InteractWithTargetAction>(Lifetime.Transient);
            builder.Register<ClearInteractableTargetAction>(Lifetime.Transient);
        }
    }
}
