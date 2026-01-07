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
            builder.Register<GoalManager>(Lifetime.Singleton);
            builder.Register<CommandAIUseCase>(Lifetime.Singleton);

            builder.Register<SharedAIState>(Lifetime.Transient);
            builder.Register<ClearTargetAction>(Lifetime.Transient);
            builder.Register<FindTargetForGoalAction>(Lifetime.Transient);
            builder.Register<FindThreatAction>(Lifetime.Transient);
        }
    }
}
