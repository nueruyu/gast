using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.UI.Hud
{
    public class GameHudViewFactory
    {
        readonly GameHudViewModel hudViewModel;
        readonly UIAssetSettings assetSettings;

        public GameHudViewFactory(GameHudViewModel hudViewModel, UIAssetSettings assetSettings)
        {
            this.hudViewModel = hudViewModel;
            this.assetSettings = assetSettings;
        }

        public GameHudView Create(CancellationToken cancellationToken)
        {
            var view = new GameHudView(assetSettings.GameHudView, assetSettings);
            view.Bind(hudViewModel).AddTo(cancellationToken);
            return view;
        }
    }
}
