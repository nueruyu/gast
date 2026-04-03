using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Unity.UI.Hud.AIStatus;
using Gast.Unity.UI.Hud.Equipment;
using Gast.Unity.UI.Hud.Inventory;
using Gast.Unity.UI.Hud.Objectives;
using Gast.Unity.UI.Hud.PlayerStatus;

namespace Gast.Unity.UI.Hud
{
    public class GameHudViewFactory
    {
        readonly GameHudViewModel hudViewModel;
        readonly UIAssetSettings assetSettings;
        readonly IPlayerStatusViewFactory playerStatusViewFactory;
        readonly InventoryViewFactory inventoryViewFactory;
        readonly AIStatusViewFactory aiStatusViewFactory;
        readonly AIObjectivesViewFactory aiObjectivesViewFactory;
        readonly EquipmentViewFactory equipmentViewFactory;

        public GameHudViewFactory(
            GameHudViewModel hudViewModel,
            UIAssetSettings assetSettings,
            IPlayerStatusViewFactory playerStatusViewFactory,
            InventoryViewFactory inventoryViewFactory,
            AIStatusViewFactory aiStatusViewFactory,
            AIObjectivesViewFactory aiObjectivesViewFactory,
            EquipmentViewFactory equipmentViewFactory)
        {
            this.hudViewModel = hudViewModel;
            this.assetSettings = assetSettings;
            this.playerStatusViewFactory = playerStatusViewFactory;
            this.inventoryViewFactory = inventoryViewFactory;
            this.aiStatusViewFactory = aiStatusViewFactory;
            this.aiObjectivesViewFactory = aiObjectivesViewFactory;
            this.equipmentViewFactory = equipmentViewFactory;
        }

        public GameHudView Create(CancellationToken cancellationToken)
        {
            var playerStatusView = playerStatusViewFactory.Create(cancellationToken);
            var inventoryView = inventoryViewFactory.Create(cancellationToken);
            var aiStatusView = aiStatusViewFactory.Create(cancellationToken);
            var aiObjectivesView = aiObjectivesViewFactory.Create(cancellationToken);
            var equipmentView = equipmentViewFactory.Create(cancellationToken);

            var view = new GameHudView(
                assetSettings.GameHudView,
                playerStatusView,
                inventoryView,
                aiStatusView,
                aiObjectivesView,
                equipmentView);

            view.Bind(hudViewModel).AddTo(cancellationToken);
            return view;
        }
    }
}
