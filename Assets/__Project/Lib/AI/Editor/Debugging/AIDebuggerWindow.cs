using Gast.Lib.AI.Debugging;
using Gast.Shared.UnityExtensions;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gast.Lib.AI.Editor.Debugging
{
    public class AIDebuggerWindow : EditorWindow
    {
        [MenuItem("Gast/Tools/AI Debugger")]
        public static void ShowWindow()
        {
            GetWindow<AIDebuggerWindow>("AI Debugger");
        }

        const string ActiveItemClass = "list-item--active";

        [SerializeField] VisualTreeAsset visualTreeAsset;

        CompositeDisposable disposables;

        ReactiveProperty<IReadOnlyDictionary<object, AIDebugInfo>> allDebugInfo;
        ReadOnlyReactiveProperty<IReadOnlyList<string>> actorChoices;
        ReactiveProperty<int> selectedActorIndex;
        ReadOnlyReactiveProperty<object> selectedActorId;
        ReadOnlyReactiveProperty<AIDebugInfo> selectedDebugInfo;

        protected ReadOnlyReactiveProperty<string> WorldStateText { get; private set; }
        protected ReadOnlyReactiveProperty<string> ActiveTaskPathText { get; private set; }
        protected ReadOnlyReactiveProperty<IReadOnlyList<string>> PlanItems { get; private set; }
        protected ReadOnlyReactiveProperty<IReadOnlyList<string>> LogItems { get; private set; }

        protected virtual void OnEnable()
        {
            disposables = new CompositeDisposable();

            allDebugInfo = new(new Dictionary<object, AIDebugInfo>());

            actorChoices = allDebugInfo
                .Select(dict => (IReadOnlyList<string>)dict.Keys.Select(id => id.ToString()).ToList())
                .ToReadOnlyReactiveProperty();

            selectedActorIndex = new ReactiveProperty<int>(-1);

            selectedActorId = allDebugInfo.CombineLatest(selectedActorIndex, (dict, index) =>
            {
                if (index < 0 || index >= dict.Count)
                    return null;
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

            EditorApplication.update += OnEditorUpdate;
        }

        int lastActorCount;

        void OnEditorUpdate()
        {
            if (EditorApplication.isPlaying && AIDebuggerBridge.IsInitialized)
            {
                var newInfo = AIDebuggerBridge.Instance.GetAllDebugInfo();
                var countChanged = newInfo.Count != lastActorCount;
                lastActorCount = newInfo.Count;

                if (countChanged || !ReferenceEquals(newInfo, allDebugInfo.Value))
                {
                    allDebugInfo.Value = newInfo;
                    allDebugInfo.ForceNotify();
                }
            }
            else if (allDebugInfo.Value.Count > 0)
            {
                lastActorCount = 0;
                allDebugInfo.Value = new Dictionary<object, AIDebugInfo>();
            }
        }

        protected virtual void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
            disposables?.Dispose();
        }

        public virtual void CreateGUI()
        {
            var root = rootVisualElement;
            root.Clear();

            var uxml = visualTreeAsset;

            if (uxml == null)
            {
                root.Add(new Label($"VisualTreeAsset is not assigned"));
                return;
            }

            uxml.CloneTree(root);

            SetupDropdown(root);
            SetupWorldState(root);
            SetupPlanList(root);
            SetupLogList(root);
        }

        void SetupDropdown(VisualElement root)
        {
            var dropdown = root.Q<DropdownField>("actor-dropdown");
            if (dropdown == null)
                return;

            dropdown.Bind(selectedActorIndex, actorChoices);
        }

        void SetupWorldState(VisualElement root)
        {
            var worldStateLabel = root.Q<Label>("world-state-label");
            if (worldStateLabel == null)
                return;

            WorldStateText.Subscribe(x => worldStateLabel.text = x).AddTo(disposables);
        }

        void SetupPlanList(VisualElement root)
        {
            var planList = root.Q<ListView>("plan-list");
            if (planList == null)
                return;

            planList.makeItem = () =>
            {
                var label = new Label();
                label.AddToClassList("list-item");
                return label;
            };

            planList.bindItem = (element, index) =>
            {
                if (PlanItems.CurrentValue == null || index < 0 || index >= PlanItems.CurrentValue.Count)
                    return;

                var item = PlanItems.CurrentValue[index];
                var label = (Label)element;
                label.text = item;

                var isActive = ActiveTaskPathText.CurrentValue?.EndsWith(item, StringComparison.Ordinal) ?? false;
                label.EnableInClassList(ActiveItemClass, isActive);
            };

            PlanItems.Subscribe(items =>
            {
                planList.itemsSource = items as System.Collections.IList ?? new List<string>(items);
                planList.Rebuild();
            }).AddTo(disposables);

            ActiveTaskPathText.Subscribe(_ => planList.Rebuild()).AddTo(disposables);
        }

        void SetupLogList(VisualElement root)
        {
            var logList = root.Q<ListView>("log-list");
            if (logList == null)
                return;

            logList.makeItem = () =>
            {
                var label = new Label();
                label.AddToClassList("list-item");
                label.AddToClassList("list-item--log");
                return label;
            };

            logList.bindItem = (element, index) =>
            {
                if (LogItems.CurrentValue == null || index < 0 || index >= LogItems.CurrentValue.Count)
                    return;

                ((Label)element).text = LogItems.CurrentValue[index];
            };

            LogItems.Subscribe(items =>
            {
                logList.itemsSource = items as System.Collections.IList ?? new List<string>(items);
                logList.Rebuild();
            }).AddTo(disposables);
        }
    }
}