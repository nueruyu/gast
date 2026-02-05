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
        VisualTreeAsset acquireItemObjectiveView;

        public VisualTreeAsset AcquireItemObjectiveView => acquireItemObjectiveView;

        [SerializeField]
        VisualTreeAsset defeatCharacterObjectiveView;

        public VisualTreeAsset DefeatCharacterObjectiveView => defeatCharacterObjectiveView;
    }
}