using R3;
using UnityEngine.UIElements;

namespace Gast.UI.Hud
{
    public interface IAIObjectiveViewModel
    {
        ReadOnlyReactiveProperty<bool> IsCompleted { get; }
        VisualElement CreateView(UIAssetSettings assetSettings);
    }
}
