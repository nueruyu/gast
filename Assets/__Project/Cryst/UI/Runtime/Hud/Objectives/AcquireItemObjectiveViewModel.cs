using Gast.Application.Economy;
using Gast.Domain.AI.Objectives;
using Gast.Domain.Economy;
using Gast.Shared.Observables;
using Gast.UI.Hud.Objectives;
using R3;
using UnityEngine;

namespace Cryst.UI.Hud.Objectives
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
            IItemAssetService itemAssetService)
        {
            var itemDefinition = itemRepository.Get(objective.TargetItemId);

            Description = itemDefinition != null
                ? $"Acquire {itemDefinition.Name}"
                : "Acquire Item";

            ItemIcon = itemAssetService.GetItemIcon(objective.TargetItemId);

            IsCompleted = objective.IsCompleted.ToObservable().ToReadOnlyReactiveProperty();

            ProgressRatio = objective.CurrentQuantity.ToObservable()
                .Select(current => objective.TargetQuantity > 0 ? (float)current / objective.TargetQuantity : 0f)
                .ToReadOnlyReactiveProperty();

            ProgressText = objective.CurrentQuantity.ToObservable()
                .Select(current => $"{current} / {objective.TargetQuantity}")
                .ToReadOnlyReactiveProperty();
        }
    }
}
