// Assets/__Project/Gast/UI/Runtime/Hud/PlayerStatusViewFactory.cs
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.UI.Hud
{
    public class PlayerStatusViewFactory
    {
        readonly PlayerStatusViewModel viewModel;
        readonly UIAssetSettings assetSettings;

        public PlayerStatusViewFactory(PlayerStatusViewModel viewModel, UIAssetSettings assetSettings)
        {
            this.viewModel = viewModel;
            this.assetSettings = assetSettings;
        }

        public PlayerStatusView Create(CancellationToken cancellationToken)
        {
            var view = new PlayerStatusView(assetSettings.PlayerStatusView, assetSettings);
            view.Bind(viewModel).AddTo(cancellationToken);
            return view;
        }
    }
}
