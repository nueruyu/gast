using Gast.Core.Tasks;
using Gast.UI.Hud;
using Gast.UI.Interactions;
using Gast.UI.Root;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Gast.UI
{
    public class UIBootstrap : ILifecycleTask
    {
        readonly UIDocument uiDocument;
        readonly UIAssetSettings assetSettings;
        readonly GameHudViewFactory gameHudViewFactory;
        readonly MenuViewFactory menuViewFactory;
        readonly InteractionPromptViewFactory interactionPromptViewFactory;

        public UIBootstrap(
            UIDocument uiDocument,
            UIAssetSettings assetSettings,
            GameHudViewFactory gameHudViewFactory,
            MenuViewFactory menuViewFactory,
            InteractionPromptViewFactory interactionPromptViewFactory)
        {
            this.uiDocument = uiDocument;
            this.assetSettings = assetSettings;
            this.gameHudViewFactory = gameHudViewFactory;
            this.menuViewFactory = menuViewFactory;
            this.interactionPromptViewFactory = interactionPromptViewFactory;
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            var rootView = new GameRootView(assetSettings.GameRootView);
            uiDocument.rootVisualElement.Add(rootView);

            var gameHudView = gameHudViewFactory.Create(cancellationToken);
            rootView.HudLayer.Add(gameHudView);

            var menuView = menuViewFactory.Create(cancellationToken);
            rootView.MenuLayer.Add(menuView);

            var interactionPromptView = interactionPromptViewFactory.Create(cancellationToken);
            rootView.DialogLayer.Add(interactionPromptView);

            return Task.CompletedTask;
        }
    }
}