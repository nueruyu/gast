using UnityEngine.UIElements;

namespace Gast.UI.Root
{
    /// <summary>
    /// Root visual element that manages the SPA-style UI structure with layered Z-order.
    /// </summary>
    public class GameRootView : VisualElement
    {
        public VisualElement HudLayer { get; }
        public VisualElement MenuLayer { get; }
        public VisualElement DialogLayer { get; }

        /// <summary>
        /// Creates the root view and initializes the layer hierarchy.
        /// </summary>
        /// <param name="asset">The VisualTreeAsset to clone for the UI structure.</param>
        public GameRootView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);
            pickingMode = PickingMode.Ignore;

            HudLayer = this.Q<VisualElement>("HudLayer");
            MenuLayer = this.Q<VisualElement>("MenuLayer");
            DialogLayer = this.Q<VisualElement>("DialogLayer");

            HudLayer.pickingMode = PickingMode.Ignore;
            MenuLayer.pickingMode = PickingMode.Ignore;
            DialogLayer.pickingMode = PickingMode.Ignore;
        }
    }
}