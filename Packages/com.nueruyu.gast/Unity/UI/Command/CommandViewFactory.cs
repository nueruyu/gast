using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.Unity.UI.Command
{
    public class CommandViewFactory
    {
        readonly CommandViewModel viewModel;
        readonly UIAssetSettings assetSettings;

        public CommandViewFactory(CommandViewModel viewModel, UIAssetSettings assetSettings)
        {
            this.viewModel = viewModel;
            this.assetSettings = assetSettings;
        }

        public CommandView Create(CancellationToken cancellationToken)
        {
            var view = new CommandView(assetSettings.CommandView);
            view.Bind(viewModel).AddTo(cancellationToken);
            return view;
        }
    }
}