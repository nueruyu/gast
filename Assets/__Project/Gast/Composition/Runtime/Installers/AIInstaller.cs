using Gast.Application.UseCases.Npcs;
using Gast.Features.Npcs;
using Gast.Features.Npcs.Actions;
using VContainer;
using VContainer.Unity;

namespace Gast.Composition.Installers
{
    public class AIInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<CommandAIUseCase>(Lifetime.Singleton);

            builder.Register<ClearTargetAction>(Lifetime.Transient);
            builder.Register<FindTargetForGoalAction>(Lifetime.Transient);
            builder.Register<FindThreatAction>(Lifetime.Transient);

            builder.Register<FindItemPickupAction>(Lifetime.Transient);
            builder.Register<MoveToInteractableAction>(Lifetime.Transient);
            builder.Register<InteractWithTargetAction>(Lifetime.Transient);
            builder.Register<ClearInteractableTargetAction>(Lifetime.Transient);
        }
    }
}