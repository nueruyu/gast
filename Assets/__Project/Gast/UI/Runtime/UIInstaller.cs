using Gast.Core.DI;
using Gast.Core.Tasks;
using Gast.UI.Command;
using Gast.UI.Hud;
using Gast.UI.Interactions;
using Gast.UI.Menu;
using Gast.UI.System;

namespace Gast.UI
{
    public class UIInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<UIBootstrap>().AsImplementedInterfaces();
            builder.Register<InputModeController>().AsImplementedInterfaces();

            builder.Register<GameHudViewModel>();
            builder.Register<GameHudViewFactory>();

            builder.Register<MenuViewModel>();
            builder.Register<MenuViewFactory>();

            builder.Register<InteractionPromptViewModel>();
            builder.Register<InteractionPromptViewFactory>();

            builder.Register<CommandViewModel>();
            builder.Register<CommandViewFactory>();

            builder.Register<ItemStackViewModelFactory>();
            builder.Register<AIObjectiveViewModelFactory>();

            builder.Register<CursorController>().As<ILifecycleTask>();
        }
    }
}
