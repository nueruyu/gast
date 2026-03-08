using Gast.Core.DI;
using Cryst.Features.CharacterAI.Humanoid;
using Cryst.Infrastructure.CharacterAI;

namespace Cryst.Features.CharacterAI
{
    public class CharacterAIInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // AI Brain
            builder.Register<AIBrainServices>(Lifetime.Singleton);
            builder.Register<ObjectiveManager>(Lifetime.Transient);
            builder.Register<CharacterAIBrainFactory>().AsImplementedInterfaces();

            // Install Humanoid-specific domains
            new HumanoidInstaller().Install(builder);
        }
    }
}
