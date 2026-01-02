using DescrioGames.Core.Tasks;
using DescrioGames.UI.Hud;
using DescrioGames.UI.Interactions;
using DescrioGames.UI.Root;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace DescrioGames.UI
{
    public class UIBootstrap : ILifecycleTask
    {
        readonly UIDocument uiDocument;
        readonly UIAssetSettings assetSettings;
        readonly GameHudViewFactory gameHudViewFactory;
        readonly InteractionPromptViewFactory interactionPromptViewFactory;

        public UIBootstrap(
            UIDocument uiDocument,
            UIAssetSettings assetSettings,
            GameHudViewFactory gameHudViewFactory,
            InteractionPromptViewFactory interactionPromptViewFactory)
        {
            this.uiDocument = uiDocument;
            this.assetSettings = assetSettings;
            this.gameHudViewFactory = gameHudViewFactory;
            this.interactionPromptViewFactory = interactionPromptViewFactory;
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            var rootView = new GameRootView(assetSettings.GameRootView);
            uiDocument.rootVisualElement.Add(rootView);

            var gameHudView = gameHudViewFactory.Create(cancellationToken);
            rootView.HudLayer.Add(gameHudView);

            var interactionPromptView = interactionPromptViewFactory.Create(cancellationToken);
            rootView.DialogLayer.Add(interactionPromptView);

            return Task.CompletedTask;
        }
    }
}