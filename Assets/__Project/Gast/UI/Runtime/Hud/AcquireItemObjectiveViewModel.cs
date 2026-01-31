using Gast.Domain.AI.Objectives;
using Gast.Domain.Economy;
using Gast.Shared.Observables;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gast.UI.Hud
{
    public class AcquireItemObjectiveViewModel : IAIObjectiveViewModel
    {
        public ReadOnlyReactiveProperty<bool> IsCompleted { get; }
        public ReadOnlyReactiveProperty<float> ProgressRatio { get; }
        public ReadOnlyReactiveProperty<string> ProgressText { get; }
        public Sprite ItemIcon { get; }
        public string Description { get; }

        public AcquireItemObjectiveViewModel(
            AcquireItemObjective objective,
            IItemRepository itemRepository,
            Sprite itemIcon)
        {
            var itemDefinition = itemRepository.Get(objective.TargetItemId);

            Description = itemDefinition != null
                ? $"Acquire {itemDefinition.Name}"
                : "Acquire Item";

            ItemIcon = itemIcon;

            IsCompleted = objective.IsCompleted.ToObservable().ToReadOnlyReactiveProperty();

            ProgressRatio = objective.CurrentQuantity.ToObservable()
                .Select(current => objective.TargetQuantity > 0 ? (float)current / objective.TargetQuantity : 0f)
                .ToReadOnlyReactiveProperty();

            ProgressText = objective.CurrentQuantity.ToObservable()
                .Select(current => $"{current} / {objective.TargetQuantity}")
                .ToReadOnlyReactiveProperty();
        }

        public VisualElement CreateView(UIAssetSettings assetSettings)
        {
            var view = new AcquireItemObjectiveView(assetSettings.AcquireItemObjectiveView);
            view.Bind(this);
            return view;
        }
    }
}