using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.Unity.UI.Hud.Inventory
{
    public class InventoryViewFactory
    {
        readonly InventoryViewModel viewModel;
        readonly UIAssetSettings assetSettings;

        public InventoryViewFactory(InventoryViewModel viewModel, UIAssetSettings assetSettings)
        {
            this.viewModel = viewModel;
            this.assetSettings = assetSettings;
        }

        public InventoryView Create(CancellationToken cancellationToken)
        {
            var view = new InventoryView(assetSettings.InventoryView);
            view.Bind(viewModel).AddTo(cancellationToken);
            return view;
        }
    }
}