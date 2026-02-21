using System;
using Gast.Shared.UnityExtensions;
using R3;
using UnityEngine.UIElements;

namespace Gast.UI.Hud
{
    public class AIObjectivesView : VisualElement
    {
        private readonly VisualElement objectivesList;
        private readonly Button minimizeButton;
        private readonly ScrollView objectivesScrollView;
        private readonly UIAssetSettings assetSettings;

        public AIObjectivesView(VisualTreeAsset asset, UIAssetSettings assetSettings)
        {
            this.assetSettings = assetSettings;
            asset.CloneTree(this);

            objectivesList = this.Q<VisualElement>("AiObjectivesList");
            minimizeButton = this.Q<Button>("MinimizeButton");
            objectivesScrollView = this.Q<ScrollView>("AiObjectivesScrollView");
        }

        public IDisposable Bind(AIObjectivesViewModel viewModel)
        {
            var d = new CompositeDisposable();

            viewModel.IsAiControlActive
                .Subscribe(active => style.display = active ? DisplayStyle.Flex : DisplayStyle.None)
                .AddTo(d);

            viewModel.AiObjectives.Subscribe(objectives =>
            {
                objectivesList.Clear();
                foreach (var objectiveVm in objectives)
                {
                    var objectiveView = new AIObjectiveView(assetSettings.AIObjectiveView);
                    objectiveView.Bind(objectiveVm);
                    objectivesList.Add(objectiveView);
                }
            }).AddTo(d);

            bool isMinimized = false;
            minimizeButton.SubscribeEvent<ClickEvent>(_ =>
            {
                isMinimized = !isMinimized;
                objectivesScrollView.style.display = isMinimized ? DisplayStyle.None : DisplayStyle.Flex;
                minimizeButton.text = isMinimized ? "+" : "-";
            }).AddTo(d);

            return d;
        }
    }
}