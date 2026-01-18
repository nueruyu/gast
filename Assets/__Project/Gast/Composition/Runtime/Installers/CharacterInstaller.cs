using Gast.Features.Combat;
using Gast.Infrastructure.Factories;
using Gast.Infrastructure.Repositories;
using Gast.Infrastructure.Services;
using VContainer;
using VContainer.Unity;

namespace Gast.Composition.Installers
{
    public class CharacterInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // Character management
            builder.Register<CharacterRepository>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<CharacterActorRepository>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<CharacterTypeRepository>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<CharacterFactory>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<CharacterAIBrainFactory>(Lifetime.Singleton).AsImplementedInterfaces();

            // Combat
            builder.Register<CombatFeedbackService>(Lifetime.Singleton);
            builder.Register<CombatMethodFactory>(Lifetime.Singleton).AsImplementedInterfaces();

            // Misc
            builder.Register<CharacterFootstepService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
    }
}