using System.Threading;

namespace Gast.Unity.UI.Hud.Equipment
{
    public class EquipmentViewFactory
    {
        readonly EquipmentViewModel viewModel;
        readonly UIAssetSettings assetSettings;

        public EquipmentViewFactory(EquipmentViewModel viewModel, UIAssetSettings assetSettings)
        {
            this.viewModel = viewModel;
            this.assetSettings = assetSettings;
        }

        public EquipmentView Create(CancellationToken cancellationToken)
        {
            var view = new EquipmentView(assetSettings.EquipmentView);
            view.Bind(viewModel).AddTo(cancellationToken);
            return view;
        }
    }
}
