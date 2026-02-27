using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Unity.UI.Command;

namespace Gast.Unity.UI.Menu
{
    public class MenuViewFactory
    {
        readonly MenuViewModel viewModel;
        readonly CommandViewModel commandViewModel;
        readonly UIAssetSettings assetSettings;

        public MenuViewFactory(
            MenuViewModel viewModel,
            CommandViewModel commandViewModel,
            UIAssetSettings assetSettings)
        {
            this.viewModel = viewModel;
            this.commandViewModel = commandViewModel;
            this.assetSettings = assetSettings;
        }

        public MenuView Create(CancellationToken cancellationToken)
        {
            var view = new MenuView(assetSettings.MenuView);
            view.Bind(viewModel, commandViewModel).AddTo(cancellationToken);
            return view;
        }
    }
}