using Gast.Application.UseCases.Npcs;
using Gast.Features.Npcs;
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
        }
    }
}