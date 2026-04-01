using System;
using System.Collections.Generic;
using ObservableCollections;
using R3;
using UnityEngine.UIElements;

namespace Gast.Lib.AI.Editor.Debugging
{
    class LogListView : IDisposable
    {
        readonly ListView logList;
        readonly Toggle autoScrollToggle;
        readonly Button clearButton;
        readonly CompositeDisposable disposables = new();

        public LogListView(ListView logList, Toggle autoScrollToggle, Button clearButton)
        {
            this.logList = logList;
            this.autoScrollToggle = autoScrollToggle;
            this.clearButton = clearButton;

            logList.makeItem = () =>
            {
                var label = new Label();
                label.AddToClassList("list-item");
                label.AddToClassList("list-item--log");
                return label;
            };
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
