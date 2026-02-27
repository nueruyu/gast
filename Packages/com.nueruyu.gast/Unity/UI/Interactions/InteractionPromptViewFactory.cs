using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.Unity.UI.Interactions
{
    public class InteractionPromptViewFactory
    {
        readonly InteractionPromptViewModel viewModel;
        readonly UIAssetSettings assetSettings;

        public InteractionPromptViewFactory(
            InteractionPromptViewModel viewModel,
            UIAssetSettings assetSettings)
        {
            this.viewModel = viewModel;
            this.assetSettings = assetSettings;
        }

        public InteractionPromptView Create(CancellationToken cancellationToken)
        {
            var view = new InteractionPromptView(assetSettings.InteractionPromptView);
            view.Bind(viewModel).AddTo(cancellationToken);
            return view;
        }
    }
}