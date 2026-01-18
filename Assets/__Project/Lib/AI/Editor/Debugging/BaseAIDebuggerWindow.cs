using Gast.Lib.AI.Debugging;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gast.Lib.AI.Editor.Debugging
{
    public abstract class BaseAIDebuggerWindow : EditorWindow
    {
        private CancellationTokenSource cancellationTokenSource;
        private CompositeDisposable disposables;

        private ReactiveProperty<IReadOnlyDictionary<object, AIDebugInfo>> allDebugInfo;
        private ReadOnlyReactiveProperty<IReadOnlyList<string>> actorChoices;
        private ReactiveProperty<int> selectedActorIndex;
        private ReadOnlyReactiveProperty<object> selectedActorId;
        private ReadOnlyReactiveProperty<AIDebugInfo> selectedDebugInfo;

        protected ReadOnlyReactiveProperty<string> WorldStateText { get; private set; }
        protected ReadOnlyReactiveProperty<string> ActiveTaskPathText { get; private set; }
        protected ReadOnlyReactiveProperty<IReadOnlyList<string>> PlanItems { get; private set; }
        protected ReadOnlyReactiveProperty<IReadOnlyList<string>> LogItems { get; private set; }

        protected virtual void OnEnable()
        {
            cancellationTokenSource = new CancellationTokenSource();
            disposables = new CompositeDisposable();

            allDebugInfo = new ReactiveProperty<IReadOnlyDictionary<object, AIDebugInfo>>(new Dictionary<object, AIDebugInfo>());

            actorChoices = allDebugInfo
                .Select(dict => (IReadOnlyList<string>)dict.Keys.Select(id => id.ToString()).ToList())
                .ToReadOnlyReactiveProperty();

            selectedActorIndex = new ReactiveProperty<int>(-1);

            selectedActorId = allDebugInfo.CombineLatest(selectedActorIndex, (dict, index) =>
            {
                if (index < 0 || index >= dict.Count) return null;
                return dict.Keys.ElementAt(index);
            }).ToReadOnlyReactiveProperty();

            selectedDebugInfo = selectedActorId
                .Select(id => id != null && allDebugInfo.CurrentValue.TryGetValue(id, out var info) ? info : null)
                .ToReadOnlyReactiveProperty();

            WorldStateText = selectedDebugInfo
                .Select(info => info != null ? info.WorldStateText.AsObservable() : Observable.Return(""))
                .Switch()
                .ToReadOnlyReactiveProperty("");

            ActiveTaskPathText = selectedDebugInfo
                .Select(info => info != null ? info.ActiveTaskPath.AsObservable() : Observable.Return(""))
                .Switch()
                .ToReadOnlyReactiveProperty("");

            PlanItems = selectedDebugInfo
                .Select(info => info != null ? info.CurrentPlan.AsObservable() : Observable.Return((IReadOnlyList<string>)Array.Empty<string>()))
                .Switch()
                .ToReadOnlyReactiveProperty(Array.Empty<string>());

            var logs = selectedDebugInfo
                .Select(info => info != null ? info.Logs.ObserveAdd() : Observable.Empty<CollectionAddEvent<string>>())
                .Switch();
            LogItems = logs
                .Select(_ => selectedDebugInfo.CurrentValue?.Logs.ToList() ?? (IReadOnlyList<string>)Array.Empty<string>())
                .ToReadOnlyReactiveProperty(Array.Empty<string>());

            Observable.EveryUpdate(cancellationTokenSource.Token)
                .Subscribe(_ =>
                {
                    if (EditorApplication.isPlaying && AIDebuggerBridge.IsInitialized)
                    {
                        allDebugInfo.Value = AIDebuggerBridge.Instance.GetAllDebugInfo();
                    }
                    else if (allDebugInfo.Value.Count > 0)
                    {
                        allDebugInfo.Value = new Dictionary<object, AIDebugInfo>();
                    }
                }).AddTo(disposables);
        }

        protected virtual void OnDisable()
        {
            disposables?.Dispose();
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
        }

        public virtual void CreateGUI()
        {
            var root = rootVisualElement;
            root.Clear();

            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.paddingLeft = 5;
            header.style.paddingRight = 5;
            header.style.paddingTop = 5;
            header.style.paddingBottom = 5;
            header.style.alignItems = Align.Center;

            var targetLabel = new Label("Target AI:");
            targetLabel.style.marginRight = 5;
            header.Add(targetLabel);

            var actorDropdown = new DropdownField();
            actorDropdown.style.flexGrow = 1;
            BindDropdown(actorDropdown, selectedActorIndex, actorChoices);
            header.Add(actorDropdown);
            root.Add(header);

            var mainContainer = new TwoPaneSplitView(0, 300, TwoPaneSplitViewOrientation.Horizontal);
            mainContainer.style.flexGrow = 1;
            root.Add(mainContainer);

            var leftPane = new ScrollView(ScrollViewMode.Vertical);
            mainContainer.Add(leftPane);

            var rightPane = new VisualElement();
            rightPane.style.flexGrow = 1;
            mainContainer.Add(rightPane);

            // Left Pane: World State & Plan
            CreateLeftPane(leftPane);

            // Right Pane: Logs
            CreateRightPane(rightPane);
        }

        protected virtual void CreateLeftPane(VisualElement container)
        {
            var worldStateHeader = new Label("World State");
            worldStateHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            worldStateHeader.style.marginTop = 5;
            container.Add(worldStateHeader);

            var worldStateLabel = new Label();
            worldStateLabel.style.whiteSpace = WhiteSpace.Normal;
            worldStateLabel.style.paddingLeft = 5;
            worldStateLabel.style.paddingRight = 5;
            worldStateLabel.style.paddingTop = 5;
            worldStateLabel.style.paddingBottom = 5;
            worldStateLabel.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            worldStateLabel.style.borderTopLeftRadius = 3;
            worldStateLabel.style.borderTopRightRadius = 3;
            worldStateLabel.style.borderBottomLeftRadius = 3;
            worldStateLabel.style.borderBottomRightRadius = 3;
            WorldStateText.Subscribe(x => worldStateLabel.text = x).AddTo(disposables);
            container.Add(worldStateLabel);

            var planHeader = new Label("Current Plan");
            planHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            planHeader.style.marginTop = 10;
            container.Add(planHeader);

            var planView = new ListView();
            planView.style.flexGrow = 1;
            planView.style.minHeight = 200;
            planView.makeItem = () =>
            {
                var label = new Label();
                label.style.paddingLeft = 2;
                label.style.paddingRight = 2;
                label.style.paddingTop = 2;
                label.style.paddingBottom = 2;
                return label;
            };
            planView.bindItem = (element, index) =>
            {
                if (PlanItems.CurrentValue != null && index >= 0 && index < PlanItems.CurrentValue.Count)
                {
                    var item = PlanItems.CurrentValue[index];
                    var label = (Label)element;
                    label.text = item;
                    bool isActive = ActiveTaskPathText.CurrentValue?.EndsWith(item, StringComparison.Ordinal) ?? false;
                    label.style.backgroundColor = isActive ? new Color(0.3f, 0.4f, 0.6f) : Color.clear;
                }
            };
            PlanItems.Subscribe(items =>
            {
                planView.itemsSource = items as System.Collections.IList ?? new List<string>(items);
                planView.Rebuild();
            }).AddTo(disposables);

            ActiveTaskPathText.Subscribe(_ => planView.Rebuild()).AddTo(disposables);
            container.Add(planView);
        }

        protected virtual void CreateRightPane(VisualElement container)
        {
            var logsHeader = new Label("Logs");
            logsHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            logsHeader.style.marginTop = 5;
            container.Add(logsHeader);

            var logView = new ListView();
            logView.style.flexGrow = 1;
            logView.makeItem = () =>
            {
                var label = new Label();
                label.style.paddingLeft = 2;
                label.style.paddingRight = 2;
                label.style.paddingTop = 2;
                label.style.paddingBottom = 2;
                label.style.whiteSpace = WhiteSpace.Normal;
                return label;
            };
            logView.bindItem = (element, index) =>
            {
                if (LogItems.CurrentValue != null && index >= 0 && index < LogItems.CurrentValue.Count)
                {
                    ((Label)element).text = LogItems.CurrentValue[index];
                }
            };
            LogItems.Subscribe(items =>
            {
                logView.itemsSource = items as System.Collections.IList ?? new List<string>(items);
                logView.Rebuild();
            }).AddTo(disposables);
            container.Add(logView);
        }

        private void BindDropdown(
            DropdownField dropdown,
            ReactiveProperty<int> selectedIndex,
            ReadOnlyReactiveProperty<IReadOnlyList<string>> choices)
        {
            choices.Subscribe(c =>
            {
                dropdown.choices = c is List<string> list ? list : new List<string>(c);
                if (selectedIndex.Value >= c.Count && c.Count > 0)
                {
                    selectedIndex.Value = c.Count - 1;
                }
                else if (c.Count == 0)
                {
                    selectedIndex.Value = -1;
                }
            }).AddTo(disposables);

            selectedIndex.Subscribe(index =>
            {
                if (dropdown.index != index)
                {
                    dropdown.index = index;
                }
            }).AddTo(disposables);

            dropdown.RegisterValueChangedCallback(evt =>
            {
                selectedIndex.Value = dropdown.index;
            });
        }
    }
}
