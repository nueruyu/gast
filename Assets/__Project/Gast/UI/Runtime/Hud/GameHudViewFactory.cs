using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.UI.Command;

namespace Gast.UI.Hud
{
    public class GameHudViewFactory
    {
        readonly GameHudViewModel hudViewModel;
        readonly CommandViewModel commandViewModel;
        readonly UIAssetSettings assetSettings;

        public GameHudViewFactory(GameHudViewModel hudViewModel, CommandViewModel commandViewModel, UIAssetSettings assetSettings)
        {
            this.hudViewModel = hudViewModel;
            this.commandViewModel = commandViewModel;
            this.assetSettings = assetSettings;
        }

        public GameHudView Create(CancellationToken cancellationToken)
        {
            var view = new GameHudView(assetSettings.GameHudView);
            view.Bind(hudViewModel, commandViewModel).AddTo(cancellationToken);
            return view;
        }
    }
}
