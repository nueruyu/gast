using Gast.Domain.AI.Objectives;
using Gast.Domain.Characters;
using Gast.Shared.Observables;
using R3;
using UnityEngine.UIElements;

namespace Gast.UI.Hud
{
    public class DefeatCharacterObjectiveViewModel : IAIObjectiveViewModel
    {
        readonly DefeatCharacterObjective objective;
        readonly ICharacterTypeDefinition targetType;

        public ReadOnlyReactiveProperty<bool> IsCompleted { get; }
        public ReadOnlyReactiveProperty<float> ProgressRatio { get; }
        public ReadOnlyReactiveProperty<string> ProgressText { get; }
        public string Description { get; }

        public DefeatCharacterObjectiveViewModel(DefeatCharacterObjective objective, ICharacterTypeRepository characterTypeRepository)
        {
            this.objective = objective;

            try
            {
                this.targetType = characterTypeRepository.Get(objective.TargetTypeId);
                Description = $"Defeat {targetType.DisplayName}";
            }
            catch
            {
                this.targetType = null;
                Description = "Defeat Enemy";
            }

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
            var view = new DefeatCharacterObjectiveView(assetSettings.DefeatCharacterObjectiveView);
            view.Bind(this);
            return view;
        }
    }
}
