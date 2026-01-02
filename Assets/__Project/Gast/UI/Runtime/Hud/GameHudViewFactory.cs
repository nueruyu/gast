using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.UI.Hud
{
    public class GameHudViewFactory
    {
        readonly GameHudViewModel viewModel;
        readonly UIAssetSettings assetSettings;

        public GameHudViewFactory(GameHudViewModel viewModel, UIAssetSettings assetSettings)
        {
            this.viewModel = viewModel;
            this.assetSettings = assetSettings;
        }

        public GameHudView Create(CancellationToken cancellationToken)
        {
            var view = new GameHudView(assetSettings.GameHudView);
            view.Bind(viewModel).AddTo(cancellationToken);
            return view;
        }
    }
}