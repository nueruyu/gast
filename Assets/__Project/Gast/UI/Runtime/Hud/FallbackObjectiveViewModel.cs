using Gast.Domain.AI;
using Gast.Domain.AI.Attributes;
using Gast.Shared.Observables;
using R3;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gast.UI.Hud
{
    public class FallbackObjectiveViewModel : IAIObjectiveViewModel
    {
        public ReadOnlyReactiveProperty<bool> IsCompleted { get; }
        readonly string description;

        public FallbackObjectiveViewModel(IAIObjective objective)
        {
            IsCompleted = objective.IsCompleted.ToObservable().ToReadOnlyReactiveProperty();

            var attr = objective.GetType().GetCustomAttribute<AIObjectiveAttribute>();
            description = attr?.Description ?? objective.GetType().Name;
        }

        public VisualElement CreateView(UIAssetSettings assetSettings)
        {
            var container = new VisualElement();
            container.AddToClassList("game-hud__objective-item");

            var label = new Label(description);
            label.AddToClassList("game-hud__objective-description");
            container.Add(label);

            IsCompleted.Subscribe(completed =>
            {
                container.EnableInClassList("game-hud__objective-item--completed", completed);
            });

            return container;
        }
    }
}
