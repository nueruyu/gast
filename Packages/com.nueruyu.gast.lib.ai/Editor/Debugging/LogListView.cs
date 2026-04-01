using System;
using System.Collections.Generic;
using ObservableCollections;
using R3;
using UnityEngine.UIElements;

namespace Gast.Lib.AI.Editor.Debugging
{
    [UxmlElement]
    partial class LogListView : VisualElement, IDisposable
    {
        public static readonly string UssClassName = "log-list-view";
        static readonly string HeaderUssClassName = "log-header";
        static readonly string SectionLabelUssClassName = "section-header";
        static readonly string ControlsUssClassName = "log-controls";
        static readonly string ListUssClassName = "log-list";
        static readonly string ListItemUssClassName = "list-item";
        static readonly string ListItemLogUssClassName = "list-item--log";

        readonly ListView logList;
        readonly Toggle autoScrollToggle;
        readonly Button clearButton;
        readonly CompositeDisposable disposables = new();

        public LogListView()
        {
            AddToClassList(UssClassName);

            var header = new VisualElement();
            header.AddToClassList(HeaderUssClassName);

            var sectionLabel = new Label("Logs");
            sectionLabel.AddToClassList(SectionLabelUssClassName);
            header.Add(sectionLabel);

            var controls = new VisualElement();
            controls.AddToClassList(ControlsUssClassName);
            autoScrollToggle = new Toggle("Auto-scroll") { value = true };
            clearButton = new Button { text = "Clear" };
            controls.Add(autoScrollToggle);
            controls.Add(clearButton);
            header.Add(controls);

            Add(header);

            logList = new ListView();
            logList.AddToClassList(ListUssClassName);
            logList.makeItem = () =>
            {
                var label = new Label();
                label.AddToClassList(ListItemUssClassName);
                label.AddToClassList(ListItemLogUssClassName);
                return label;
            };
            Add(logList);
        }

        public void Bind(AIDebuggerViewModel vm)
        {
            autoScrollToggle.value = vm.LogAutoScroll.Value;
            autoScrollToggle.RegisterValueChangedCallback(evt => vm.LogAutoScroll.Value = evt.newValue);

            IDisposable logBindings = null;

            vm.SelectedDebugInfo.Subscribe(info =>
            {
                logBindings?.Dispose();
                logBindings = null;

                if (info == null)
                {
                    logList.itemsSource = null;
                    logList.Rebuild();
                    return;
                }

                var bindings = new CompositeDisposable();
                var logs = info.Logs;
                var logListSource = new List<string>(logs);

                logList.bindItem = (element, index) =>
                {
                    if (index < 0 || index >= logListSource.Count) return;
                    ((Label)element).text = logListSource[index];
                };

                logList.itemsSource = logListSource;
                logList.Rebuild();

                logs.ObserveAdd().Subscribe(e =>
                {
                    logListSource.Insert(e.Index, e.Value);
                    logList.RefreshItems();
                    if (vm.LogAutoScroll.Value) logList.ScrollToItem(logListSource.Count - 1);
                }).AddTo(bindings);

                logs.ObserveRemove().Subscribe(e =>
                {
                    logListSource.RemoveAt(e.Index);
                    logList.RefreshItems();
                }).AddTo(bindings);

                logs.ObserveReset().Subscribe(_ =>
                {
                    logListSource.Clear();
                    logList.RefreshItems();
                }).AddTo(bindings);

                void onClearClicked() => logs.Clear();
                clearButton.clicked += onClearClicked;
                bindings.Add(Disposable.Create(() => clearButton.clicked -= onClearClicked));

                logBindings = bindings;
            }).AddTo(disposables);

            disposables.Add(Disposable.Create(() => logBindings?.Dispose()));
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
