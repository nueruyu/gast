using System;
using System.Collections;
using System.Collections.Generic;
using R3;
using UnityEngine.UIElements;

namespace Gast.Lib.AI.Editor.Debugging
{
    [UxmlElement]
    partial class CurrentPlanView : VisualElement, IDisposable
    {
        const string ActiveItemClass = "list-item--active";

        readonly CompositeDisposable disposables = new();

        public void Bind(AIDebuggerViewModel vm)
        {
            var currentMethodLabel = this.Q<Label>("current-method-label");
            var planList = this.Q<ListView>("plan-list");

            planList.makeItem = () =>
            {
                var label = new Label();
                label.AddToClassList("list-item");
                return label;
            };

            IDisposable infoBindings = null;

            vm.SelectedDebugInfo.Subscribe(info =>
            {
                infoBindings?.Dispose();
                infoBindings = null;

                if (info == null)
                {
                    if (currentMethodLabel != null) currentMethodLabel.text = "";
                    planList.itemsSource = null;
                    planList.Rebuild();
                    return;
                }

                var bindings = new CompositeDisposable();

                info.CurrentMethodName
                    .Subscribe(text => currentMethodLabel.text = text)
                    .AddTo(bindings);

                IReadOnlyList<string> currentPlan = null;
                string currentTaskPath = null;

                planList.bindItem = (element, index) =>
                {
                    if (currentPlan == null || index < 0 || index >= currentPlan.Count) return;
                    var item = currentPlan[index];
                    var label = (Label)element;
                    label.text = item;
                    label.EnableInClassList(ActiveItemClass, currentTaskPath?.EndsWith(item) ?? false);
                };

                info.CurrentPlan.Subscribe(items =>
                {
                    currentPlan = items;
                    planList.itemsSource = items as IList ?? new List<string>(items);
                    planList.Rebuild();
                }).AddTo(bindings);

                info.ActiveTaskPath.Subscribe(path =>
                {
                    currentTaskPath = path;
                    planList.RefreshItems();
                }).AddTo(bindings);

                infoBindings = bindings;
            }).AddTo(disposables);

            disposables.Add(Disposable.Create(() => infoBindings?.Dispose()));
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
