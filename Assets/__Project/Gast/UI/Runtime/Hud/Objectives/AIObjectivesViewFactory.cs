using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.UI.Hud.Objectives
{
    public class AIObjectivesViewFactory
    {
        readonly AIObjectivesViewModel viewModel;
        readonly UIAssetSettings assetSettings;

        public AIObjectivesViewFactory(AIObjectivesViewModel viewModel, UIAssetSettings assetSettings)
        {
            this.viewModel = viewModel;
            this.assetSettings = assetSettings;
        }

        public AIObjectivesView Create(CancellationToken cancellationToken)
        {
            var view = new AIObjectivesView(assetSettings.AIObjectivesView, assetSettings);
            view.Bind(viewModel).AddTo(cancellationToken);
            return view;
        }
    }
}
