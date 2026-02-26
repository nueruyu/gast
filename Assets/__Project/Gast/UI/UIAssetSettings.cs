using UnityEngine;
using UnityEngine.UIElements;

namespace Gast.UI
{
    [CreateAssetMenu(fileName = "UIAssetSettings", menuName = "Gast/UI/Asset Settings")]
    public class UIAssetSettings : ScriptableObject
    {
        [Header("Root")]
        [SerializeField]
        VisualTreeAsset gameRootView;

        public VisualTreeAsset GameRootView => gameRootView;

        [Header("HUD")]
        [SerializeField]
        VisualTreeAsset gameHudView;

        public VisualTreeAsset GameHudView => gameHudView;

        [SerializeField]
        VisualTreeAsset playerStatusView;

        public VisualTreeAsset PlayerStatusView => playerStatusView;

        [SerializeField]
        VisualTreeAsset inventoryView;

        public VisualTreeAsset InventoryView => inventoryView;

        [SerializeField]
        VisualTreeAsset aiStatusView;

        public VisualTreeAsset AIStatusView => aiStatusView;

        [SerializeField]
        VisualTreeAsset aiObjectivesView;

        public VisualTreeAsset AIObjectivesView => aiObjectivesView;

        [Header("Menu")]
        [SerializeField]
        VisualTreeAsset menuView;

        public VisualTreeAsset MenuView => menuView;

        [Header("Interaction")]
        [SerializeField]
        VisualTreeAsset interactionPromptView;

        public VisualTreeAsset InteractionPromptView => interactionPromptView;

        [Header("Command")]
        [SerializeField]
        VisualTreeAsset commandView;

        public VisualTreeAsset CommandView => commandView;

        [Header("HUD Objectives")]
        [SerializeField]
        VisualTreeAsset aiObjectiveView;

        public VisualTreeAsset AIObjectiveView => aiObjectiveView;

        [Header("Theme")]
        [SerializeField]
        ThemeStyleSheet themeStyleSheet;

        public ThemeStyleSheet ThemeStyleSheet => themeStyleSheet;
    }
}