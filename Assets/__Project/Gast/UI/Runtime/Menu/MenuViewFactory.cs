using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.UI.Interactions
{
    public class MenuViewFactory
    {
        readonly MenuViewModel viewModel;
        readonly UIAssetSettings assetSettings;

        public MenuViewFactory(
            MenuViewModel viewModel,
            UIAssetSettings assetSettings)
        {
            this.viewModel = viewModel;
            this.assetSettings = assetSettings;
        }

        public MenuView Create(CancellationToken cancellationToken)
        {
            var view = new MenuView(assetSettings.MenuView);
            view.Bind(viewModel).AddTo(cancellationToken);
            return view;
        }
    }
}