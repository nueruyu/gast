using System.Threading;
using System.Threading.Tasks;
using Gast.Core.Tasks;
using Gast.Unity.UI.Command;
using Gast.Unity.UI.Hud;
using Gast.Unity.UI.Interactions;
using Gast.Unity.UI.Menu;
using Gast.Unity.UI.Root;
using UnityEngine.UIElements;

namespace Gast.Unity.UI
{
    public class UIBootstrap : ILifecycleTask
    {
        readonly UIDocument uiDocument;
        readonly UIAssetSettings assetSettings;
        readonly GameHudViewFactory gameHudViewFactory;
        readonly MenuViewFactory menuViewFactory;
        readonly InteractionPromptViewFactory interactionPromptViewFactory;
        readonly CommandViewFactory commandViewFactory;

        public UIBootstrap(
            UIDocument uiDocument,
            UIAssetSettings assetSettings,
            GameHudViewFactory gameHudViewFactory,
            MenuViewFactory menuViewFactory,
            InteractionPromptViewFactory interactionPromptViewFactory,
            CommandViewFactory commandViewFactory)
        {
            this.uiDocument = uiDocument;
            this.assetSettings = assetSettings;
            this.gameHudViewFactory = gameHudViewFactory;
            this.menuViewFactory = menuViewFactory;
            this.interactionPromptViewFactory = interactionPromptViewFactory;
            this.commandViewFactory = commandViewFactory;
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            var rootView = new GameRootView(assetSettings.GameRootView);
            uiDocument.rootVisualElement.Add(rootView);

            var gameHudView = gameHudViewFactory.Create(cancellationToken);
            rootView.HudLayer.Add(gameHudView);

            var menuView = menuViewFactory.Create(cancellationToken);
            rootView.MenuLayer.Add(menuView);

            var commandView = commandViewFactory.Create(cancellationToken);
            rootView.DialogLayer.Add(commandView);

            var interactionPromptView = interactionPromptViewFactory.Create(cancellationToken);
            rootView.HudLayer.Add(interactionPromptView);

            return Task.CompletedTask;
        }
    }
}
