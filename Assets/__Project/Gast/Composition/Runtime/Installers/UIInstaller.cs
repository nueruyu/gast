using DescrioGames.UI;
using DescrioGames.UI.Hud;
using DescrioGames.UI.Interactions;
using DescrioGames.UI.Root;
using VContainer;
using VContainer.Unity;

namespace DescrioGames.Composition.Installers
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