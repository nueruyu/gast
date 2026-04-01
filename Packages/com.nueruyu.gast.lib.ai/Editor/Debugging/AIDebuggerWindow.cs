using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Gast.Lib.AI.Debugging;
using ObservableCollections;
using R3;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gast.Lib.AI.Editor.Debugging
{
    public class AIDebuggerWindow : EditorWindow
    {
        const string ActiveItemClass = "list-item--active";

        [SerializeField] VisualTreeAsset visualTreeAsset;

        AIDebuggerViewModel viewModel;
        CompositeDisposable disposables;
        VisualElement worldStateContainer;

        protected virtual void OnEnable()
        {
            viewModel = new AIDebuggerViewModel();
            disposables = new CompositeDisposable();
            EditorApplication.update += OnEditorUpdate;
        }

        protected virtual void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
            disposables?.Dispose();
            viewModel?.Dispose();
        }

        public virtual void CreateGUI()
        {
            var root = rootVisualElement;
            root.Clear();

            if (visualTreeAsset == null)
            {
                root.Add(new Label("VisualTreeAsset is not assigned"));
                return;
            }

            visualTreeAsset.CloneTree(root);

            var actorList = root.Q<ListView>("actor-list");
            var domainToolbar = root.Q<VisualElement>("domain-toolbar");
            worldStateContainer = root.Q<VisualElement>("world-state-container");
            var currentMethodLabel = root.Q<Label>("current-method-label");
            var planList = root.Q<ListView>("plan-list");
            var logList = root.Q<ListView>("log-list");
            var logClearButton = root.Q<Button>("log-clear-button");
            var logAutoScrollToggle = root.Q<Toggle>("log-autoscroll-toggle");

            Bind(viewModel, actorList, domainToolbar, currentMethodLabel, planList, logList, logClearButton, logAutoScrollToggle);
        }

        [MenuItem("Gast/Tools/AI Debugger")]
        public static void ShowWindow()
        {
            GetWindow<AIDebuggerWindow>("AI Debugger");
        }

        void Bind(
            AIDebuggerViewModel vm,
            ListView actorList,
            VisualElement domainToolbar,
            Label currentMethodLabel,
            ListView planList,
            ListView logList,
            Button logClearButton,
            Toggle logAutoScrollToggle)
        {
            logAutoScrollToggle.value = vm.LogAutoScroll.Value;
            logAutoScrollToggle.RegisterValueChangedCallback(evt => vm.LogAutoScroll.Value = evt.newValue);

            actorList.makeItem = () => new Label();
            actorList.bindItem = (element, i) =>
            {
                var actor = vm.Actors.CurrentValue[i];
                ((Label)element).text = $"{actor.Name} ({actor.Id})";
            };

            vm.Actors.Subscribe(actorInfos =>
            {
                actorList.itemsSource = actorInfos;
                actorList.Rebuild();
            }).AddTo(disposables);

            Action<IEnumerable<object>> onActorSelectionChanged = _ => vm.SelectedActorIndex.Value = actorList.selectedIndex;
            actorList.selectionChanged += onActorSelectionChanged;
            disposables.Add(Disposable.Create(() => actorList.selectionChanged -= onActorSelectionChanged));

            vm.SelectedActorIndex.Subscribe(index => actorList.selectedIndex = index).AddTo(disposables);

            CompositeDisposable toggleDisposables = null;

            vm.DomainNameChoices.Subscribe(domains =>
            {
                toggleDisposables?.Dispose();
                toggleDisposables = new CompositeDisposable();

                domainToolbar.Clear();

                if (domains.Length > 0)
                {
                    if (vm.SelectedDomainIndex.Value < 0 || vm.SelectedDomainIndex.Value >= domains.Length)
                        vm.SelectedDomainIndex.Value = 0;
                }
                else
                {
                    vm.SelectedDomainIndex.Value = -1;
                }

                for (var i = 0; i < domains.Length; i++)
                {
                    var index = i;
                    var toggle = new ToolbarToggle { text = domains[i] };
                    toggle.SetValueWithoutNotify(index == vm.SelectedDomainIndex.Value);

                    var callback = new EventCallback<ChangeEvent<bool>>(evt =>
                    {
                        if (evt.newValue) vm.SelectedDomainIndex.Value = index;
                    });
                    toggle.RegisterValueChangedCallback(callback);
                    toggleDisposables.Add(Disposable.Create(() => toggle.UnregisterValueChangedCallback(callback)));
                    domainToolbar.Add(toggle);
                }
            }).AddTo(disposables);

            disposables.Add(Disposable.Create(() => toggleDisposables?.Dispose()));

            vm.SelectedDomainIndex.Subscribe(index =>
            {
                var toggles = domainToolbar.Query<ToolbarToggle>().ToList();
                for (var i = 0; i < toggles.Count; i++) toggles[i].SetValueWithoutNotify(i == index);
            }).AddTo(disposables);

            planList.makeItem = () =>
            {
                var label = new Label();
                label.AddToClassList("list-item");
                return label;
            };

            logList.makeItem = () =>
            {
                var label = new Label();
                label.AddToClassList("list-item");
                label.AddToClassList("list-item--log");
                return label;
            };

            IDisposable actorBindings = null;

            vm.SelectedDebugInfo.Subscribe(info =>
            {
                actorBindings?.Dispose();
                actorBindings = null;

                if (info == null)
                {
                    worldStateContainer?.Clear();
                    if (currentMethodLabel != null) currentMethodLabel.text = "";
                    planList.itemsSource = null;
                    planList.Rebuild();
                    logList.itemsSource = null;
                    logList.Rebuild();
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

                var logClearCallback = new EventCallback<ClickEvent>(_ => logs.Clear());
                logClearButton.RegisterCallback(logClearCallback);
                bindings.Add(Disposable.Create(() => logClearButton.UnregisterCallback(logClearCallback)));

                actorBindings = bindings;
            }).AddTo(disposables);

            disposables.Add(Disposable.Create(() => actorBindings?.Dispose()));
        }

        void OnEditorUpdate()
        {
            if (!EditorApplication.isPlaying) return;

            viewModel.Update();

            if (worldStateContainer != null && viewModel.SelectedDebugInfo.CurrentValue != null)
                RenderWorldState(worldStateContainer, viewModel.SelectedDebugInfo.CurrentValue.WorldState);
        }

        void RenderWorldState(VisualElement container, object state)
        {
            container.Clear();

            if (state == null)
                return;

            var type = state.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .OrderBy(p => p.Name);

            foreach (var property in properties)
            {
                var row = new VisualElement();
                row.AddToClassList("world-state-row");

                var nameLabel = new Label(property.Name);
                nameLabel.AddToClassList("world-state-name");

                var value = property.GetValue(state);
                var valueLabel = new Label(FormatValue(value));
                valueLabel.AddToClassList("world-state-value");

                if (value is bool boolValue)
                    valueLabel.AddToClassList(boolValue ? "world-state-value--true" : "world-state-value--false");

                row.Add(nameLabel);
                row.Add(valueLabel);
                container.Add(row);
            }
        }

        string FormatValue(object value)
        {
            if (value == null)
                return "null";

            if (value is bool boolValue)
                return boolValue ? "True" : "False";

            if (value is float floatValue)
                return floatValue.ToString("F2");

            if (value is double doubleValue)
                return doubleValue.ToString("F2");

            if (value is Vector3 vec3)
                return $"({vec3.x:F2}, {vec3.y:F2}, {vec3.z:F2})";

            if (value is Vector2 vec2)
                return $"({vec2.x:F2}, {vec2.y:F2})";

            return value.ToString();
        }
    }
}
