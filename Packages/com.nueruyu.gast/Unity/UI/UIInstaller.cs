using Gast.Core.DI;
using Gast.Core.Tasks;
using Gast.Unity.UI.Command;
using Gast.Unity.UI.Hud;
using Gast.Unity.UI.Hud.AIStatus;
using Gast.Unity.UI.Hud.Inventory;
using Gast.Unity.UI.Hud.Objectives;
using Gast.Unity.UI.Interactions;
using Gast.Unity.UI.Menu;
using Gast.Unity.UI.System;

namespace Gast.Unity.UI
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
        }
    }
}