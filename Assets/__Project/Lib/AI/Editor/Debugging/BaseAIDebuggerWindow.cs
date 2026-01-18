using Gast.Lib.AI.Debugging;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Gast.Shared.UnityExtensions;

namespace Gast.Lib.AI.Editor.Debugging
{
    public abstract class BaseAIDebuggerWindow : EditorWindow
    {
        private CancellationTokenSource cancellationTokenSource;

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

            allDebugInfo = new ReactiveProperty<IReadOnlyDictionary<object, AIDebugInfo>>(new Dictionary<object, AIDebugInfo>());

            actorChoices = allDebugInfo
                .Select(dict => (IReadOnlyList<string>)dict.Keys.Select(id => id.ToString()).ToList())
                .ToReadOnlyReactiveProperty();

            selectedActorIndex = new ReactiveProperty<int>(-1);

            selectedActorId = allDebugInfo.CombineLatest(selectedActorIndex, (dict, index) =>
            {
                if (index < 0 || index >= dict.Keys.Count) return null;
                return dict.Keys.ElementAt(index);
            }).ToReadOnlyReactiveProperty();

            selectedDebugInfo = selectedActorId
                .Select(id => id != null && allDebugInfo.CurrentValue.TryGetValue(id, out var info) ? info : null)
                .ToReadOnlyReactiveProperty();

            WorldStateText = selectedDebugInfo.Select(info => info?.WorldStateText ?? Observable.Return("")).Switch().ToReadOnlyReactiveProperty("");
            ActiveTaskPathText = selectedDebugInfo.Select(info => info?.ActiveTaskPath ?? Observable.Return("")).Switch().ToReadOnlyReactiveProperty("");
            PlanItems = selectedDebugInfo.Select(info => info?.CurrentPlan ?? Observable.Return(Array.Empty<string>())).Switch().ToReadOnlyReactiveProperty(Array.Empty<string>());

            var logs = selectedDebugInfo.Select(info => info?.Logs.ObserveAdd() ?? Observable.Empty<CollectionAddEvent<string>>()).Switch();
            LogItems = logs.Select(_ => selectedDebugInfo.CurrentValue?.Logs.ToList() ?? (IReadOnlyList<string>)Array.Empty<string>()).ToReadOnlyReactiveProperty(Array.Empty<string>());

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
                });
        }

        protected virtual void OnDisable()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
        }

        public virtual void CreateGUI()
        {
            var root = rootVisualElement;
            root.Clear();
            var token = cancellationTokenSource.Token;

            var header = new VisualElement { style = { flexDirection = FlexDirection.Row, padding = 5, alignItems = Align.Center } };
            header.Add(new Label("Target AI:") { style = { marginRight = 5 } });
            var actorDropdown = new DropdownField { style = { flexGrow = 1 } };
            actorDropdown.Bind(selectedActorIndex, actorChoices, token).AddTo(cancellationTokenSource);
            header.Add(actorDropdown);
            root.Add(header);

            var mainContainer = new TwoPaneSplitView(0, 300, TwoPaneSplitViewOrientation.Horizontal);
            mainContainer.style.flexGrow = 1;
            root.Add(mainContainer);

            var leftPane = new ScrollView(ScrollViewMode.Vertical);
            mainContainer.Add(leftPane);

            var rightPane = new VisualElement { style = { flexGrow = 1 } };
            mainContainer.Add(rightPane);

            // Left Pane: World State & Plan
            CreateLeftPane(leftPane);

            // Right Pane: Logs
            CreateRightPane(rightPane);
        }

        protected virtual void CreateLeftPane(VisualElement container)
        {
            var token = cancellationTokenSource.Token;

            container.Add(new Label("World State") { style = { unityFontStyleAndWeight = FontStyle.Bold, marginTop = 5 } });
            var worldStateLabel = new Label { style = { whiteSpace = WhiteSpace.Normal, padding = 5, backgroundColor = new Color(0.2f, 0.2f, 0.2f), borderTopLeftRadius = 3, borderTopRightRadius = 3, borderBottomLeftRadius = 3, borderBottomRightRadius = 3 }};
            worldStateLabel.BindText(WorldStateText, token).AddTo(cancellationTokenSource);
            container.Add(worldStateLabel);

            container.Add(new Label("Current Plan") { style = { unityFontStyleAndWeight = FontStyle.Bold, marginTop = 10 } });
            var planView = new ListView { style = { flexGrow = 1, minHeight = 200 } };
            planView.makeItem = () => new Label { style = { padding = 2 }};
            planView.BindTo(PlanItems, (element, item) =>
            {
                var label = (Label)element;
                label.text = item;
                bool isActive = ActiveTaskPathText.CurrentValue?.EndsWith(item, StringComparison.Ordinal) ?? false;
                label.style.backgroundColor = isActive ? new Color(0.3f, 0.4f, 0.6f) : Color.clear;
            }, token).AddTo(cancellationTokenSource);

            ActiveTaskPathText.Subscribe(_ => planView.Rebuild(), token).AddTo(cancellationTokenSource);
            container.Add(planView);
        }

        protected virtual void CreateRightPane(VisualElement container)
        {
            var token = cancellationTokenSource.Token;
            container.Add(new Label("Logs") { style = { unityFontStyleAndWeight = FontStyle.Bold, marginTop = 5 } });
            var logView = new ListView { style = { flexGrow = 1 } };
            logView.makeItem = () => new Label { style = { padding = 2, whiteSpace = WhiteSpace.Normal } };
            logView.BindTo(LogItems, (element, item) => ((Label)element).text = item, token).AddTo(cancellationTokenSource);
            container.Add(logView);
        }
    }
}
