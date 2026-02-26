using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.UI.Hud
{
    public class AIStatusViewFactory
    {
        readonly AIStatusViewModel viewModel;
        readonly UIAssetSettings assetSettings;

        public AIStatusViewFactory(AIStatusViewModel viewModel, UIAssetSettings assetSettings)
        {
            this.viewModel = viewModel;
            this.assetSettings = assetSettings;
        }

        public AIStatusView Create(CancellationToken cancellationToken)
        {
            var view = new AIStatusView(assetSettings.AIStatusView);
            view.Bind(viewModel).AddTo(cancellationToken);
            return view;
        }
    }
}
