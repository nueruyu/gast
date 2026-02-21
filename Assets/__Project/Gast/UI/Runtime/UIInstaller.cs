using Gast.Core.DI;
using Gast.Core.Tasks;
using Gast.UI.Command;
using Gast.UI.Hud;
using Gast.UI.Hud.Objectives;
using Gast.UI.Hud.Status;
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
            builder.Register<CursorController>().As<ILifecycleTask>();

            // HUD Parent
            builder.Register<GameHudViewModel>();
            builder.Register<GameHudViewFactory>();

            // HUD Children
            builder.Register<PlayerStatusViewModel>();
            builder.Register<PlayerStatusViewFactory>();
            builder.Register<InventoryViewModel>();
            builder.Register<InventoryViewFactory>();
            builder.Register<AIStatusViewModel>();
            builder.Register<AIStatusViewFactory>();
            builder.Register<AIObjectivesViewModel>();
            builder.Register<AIObjectivesViewFactory>();

            // Menu
            builder.Register<MenuViewModel>();
            builder.Register<MenuViewFactory>();

            // Interaction
            builder.Register<InteractionPromptViewModel>();
            builder.Register<InteractionPromptViewFactory>();

            // Command
            builder.Register<CommandViewModel>();
            builder.Register<CommandViewFactory>();

            // ViewModels
            builder.Register<ItemStackViewModelFactory>();
            builder.Register<AcquireItemObjectiveViewModel>(Lifetime.Transient);
            builder.Register<DefeatCharacterObjectiveViewModel>(Lifetime.Transient);
        }
    }
}
