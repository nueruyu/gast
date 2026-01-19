using Gast.Lib.AI.Debugging;
using Gast.Shared.UnityExtensions;
using ObservableCollections;
using R3;
using System;
using System.Collections;
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

            if (visualTreeAsset == null)
            {
                root.Add(new Label("VisualTreeAsset is not assigned"));
                return;
            }

            visualTreeAsset.CloneTree(root);

            var dropdown = root.Q<DropdownField>("actor-dropdown");
            var worldStateLabel = root.Q<Label>("world-state-label");
            var planList = root.Q<ListView>("plan-list");
            var logList = root.Q<ListView>("log-list");

            SetupDropdown(dropdown);
            SetupListViews(planList, logList);
            BindToSelectedActor(worldStateLabel, planList, logList);
        }

        void SetupDropdown(DropdownField dropdown)
        {
            dropdown.Bind(selectedActorIndex, actorChoices);
        }

        void SetupListViews(ListView planList, ListView logList)
        {
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
        }

        void BindToSelectedActor(Label worldStateLabel, ListView planList, ListView logList)
        {
            IDisposable actorBindings = null;

            selectedDebugInfo.Subscribe(info =>
            {
                actorBindings?.Dispose();

                if (info == null)
                {
                    ClearUI(worldStateLabel, planList, logList);
                    return;
                }

                actorBindings = BindActorInfo(info, worldStateLabel, planList, logList);
            }).AddTo(disposables);

            Disposable.Create(() => actorBindings?.Dispose()).AddTo(disposables);
        }

        void ClearUI(Label worldStateLabel, ListView planList, ListView logList)
        {
            worldStateLabel.text = "";

            planList.itemsSource = null;
            planList.Rebuild();

            logList.itemsSource = null;
            logList.Rebuild();
        }

        IDisposable BindActorInfo(AIDebugInfo info, Label worldStateLabel, ListView planList, ListView logList)
        {
            var bindings = new CompositeDisposable();

            // World State
            info.WorldStateText
                .Subscribe(x => worldStateLabel.text = x)
                .AddTo(bindings);

            // Plan List
            IReadOnlyList<string> currentPlan = null;
            string currentTaskPath = null;

            planList.bindItem = (element, index) =>
            {
                if (currentPlan == null || index < 0 || index >= currentPlan.Count)
                    return;

                var item = currentPlan[index];
                var label = (Label)element;
                label.text = item;

                var isActive = currentTaskPath?.EndsWith(item, StringComparison.Ordinal) ?? false;
                label.EnableInClassList(ActiveItemClass, isActive);
            };

            info.CurrentPlan
                .Subscribe(items =>
                {
                    currentPlan = items;
                    planList.itemsSource = items as IList ?? new List<string>(items);
                    planList.Rebuild();
                })
                .AddTo(bindings);

            info.ActiveTaskPath
                .Subscribe(path =>
                {
                    currentTaskPath = path;
                    planList.RefreshItems();
                })
                .AddTo(bindings);

            // Log List
            var logs = info.Logs;
            var logListSource = new List<string>(logs);

            logList.bindItem = (element, index) =>
            {
                if (index < 0 || index >= logListSource.Count)
                    return;
                ((Label)element).text = logListSource[index];
            };

            logList.itemsSource = logListSource;
            logList.Rebuild();

            logs.ObserveAdd()
                .Subscribe(e =>
                {
                    logListSource.Add(e.Value);
                    logList.RefreshItems();
                    logList.ScrollToItem(e.Index);
                })
                .AddTo(bindings);

            return bindings;
        }
    }
}