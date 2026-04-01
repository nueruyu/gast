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
        public static readonly string UssClassName = "current-plan-view";
        static readonly string HeaderUssClassName = UssClassName + "__header";
        static readonly string InfoRowUssClassName = UssClassName + "__info-row";
        static readonly string InfoLabelUssClassName = UssClassName + "__info-label";
        static readonly string InfoValueUssClassName = UssClassName + "__info-value";
        static readonly string ListUssClassName = UssClassName + "__list";
        static readonly string ItemUssClassName = UssClassName + "__item";
        static readonly string ActiveItemUssClassName = UssClassName + "__item--active";

        readonly Label currentMethodLabel;
        readonly ListView planList;
        readonly CompositeDisposable disposables = new();

        public CurrentPlanView()
        {
            AddToClassList(UssClassName);

            var header = new Label("Current Plan");
            header.AddToClassList(HeaderUssClassName);
            Add(header);

            var infoRow = new VisualElement();
            infoRow.AddToClassList(InfoRowUssClassName);
            var infoLabel = new Label("Method:");
            infoLabel.AddToClassList(InfoLabelUssClassName);
            currentMethodLabel = new Label();
            currentMethodLabel.AddToClassList(InfoValueUssClassName);
            infoRow.Add(infoLabel);
            infoRow.Add(currentMethodLabel);
            Add(infoRow);

            planList = new ListView();
            planList.AddToClassList(ListUssClassName);
            planList.makeItem = () =>
            {
                var label = new Label();
                label.AddToClassList(ItemUssClassName);
                return label;
            };
            Add(planList);
        }

        public void Bind(AIDebuggerViewModel vm)
        {
            IDisposable infoBindings = null;

            vm.SelectedDebugInfo.Subscribe(info =>
            {
                infoBindings?.Dispose();
                infoBindings = null;

                if (info == null)
                {
                    currentMethodLabel.text = "";
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
                    label.EnableInClassList(ActiveItemUssClassName, currentTaskPath?.EndsWith(item) ?? false);
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
