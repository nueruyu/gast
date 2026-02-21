using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.UI;
using Gast.UI.Hud.Status;
using UnityEngine.UIElements;

namespace Cryst.UI.Hud.Status
{
    public class PlayerStatusViewFactory : IPlayerStatusViewFactory
    {
        readonly PlayerStatusViewModel viewModel;
        readonly UIAssetSettings assetSettings;

        public PlayerStatusViewFactory(PlayerStatusViewModel viewModel, UIAssetSettings assetSettings)
        {
            this.viewModel = viewModel;
            this.assetSettings = assetSettings;
        }

        public VisualElement Create(CancellationToken cancellationToken)
        {
            var view = new PlayerStatusView(assetSettings.PlayerStatusView);
            view.Bind(viewModel).AddTo(cancellationToken);
            return view;
        }
    }
}
