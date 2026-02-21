// Assets/__Project/Gast/UI/Runtime/Hud/GameHudViewFactory.cs
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.UI.Hud
{
    public class GameHudViewFactory
    {
        readonly GameHudViewModel hudViewModel;
        readonly UIAssetSettings assetSettings;
        readonly PlayerStatusViewFactory playerStatusViewFactory;
        readonly InventoryViewFactory inventoryViewFactory;
        readonly AIStatusViewFactory aiStatusViewFactory;
        readonly AIObjectivesViewFactory aiObjectivesViewFactory;

        public GameHudViewFactory(
            GameHudViewModel hudViewModel,
            UIAssetSettings assetSettings,
            PlayerStatusViewFactory playerStatusViewFactory,
            InventoryViewFactory inventoryViewFactory,
            AIStatusViewFactory aiStatusViewFactory,
            AIObjectivesViewFactory aiObjectivesViewFactory)
        {
            this.hudViewModel = hudViewModel;
            this.assetSettings = assetSettings;
            this.playerStatusViewFactory = playerStatusViewFactory;
            this.inventoryViewFactory = inventoryViewFactory;
            this.aiStatusViewFactory = aiStatusViewFactory;
            this.aiObjectivesViewFactory = aiObjectivesViewFactory;
        }

        public GameHudView Create(CancellationToken cancellationToken)
        {
            var playerStatusView = playerStatusViewFactory.Create(cancellationToken);
            var inventoryView = inventoryViewFactory.Create(cancellationToken);
            var aiStatusView = aiStatusViewFactory.Create(cancellationToken);
            var aiObjectivesView = aiObjectivesViewFactory.Create(cancellationToken);

            var view = new GameHudView(
                assetSettings.GameHudView,
                playerStatusView,
                inventoryView,
                aiStatusView,
                aiObjectivesView);

            view.Bind(hudViewModel).AddTo(cancellationToken);
            return view;
        }
    }
}
