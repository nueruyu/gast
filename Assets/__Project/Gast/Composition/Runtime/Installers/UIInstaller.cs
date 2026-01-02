using Gast.UI;
using Gast.UI.Hud;
using Gast.UI.Interactions;
using Gast.UI.Root;
using VContainer;
using VContainer.Unity;

namespace Gast.Composition.Installers
{
    public class UIInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<GameRootView>(Lifetime.Singleton);
            builder.Register<UIBootstrap>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<GameHudViewModel>(Lifetime.Singleton);
            builder.Register<GameHudViewFactory>(Lifetime.Singleton);

            builder.Register<InteractionPromptViewModel>(Lifetime.Singleton);
            builder.Register<InteractionPromptViewFactory>(Lifetime.Singleton);

            builder.Register<ItemStackViewModelFactory>(Lifetime.Singleton);
        }
    }
}