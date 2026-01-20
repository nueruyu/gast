using Gast.Lib.AI.Debugging;
using Gast.Shared.UnityExtensions;
using ObservableCollections;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
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

        readonly CompositeDisposable disposables = new();

        readonly ReactiveProperty<IReadOnlyDictionary<ContextKey, AIDebugInfo>> allDebugInfo = new();
        readonly ReactiveProperty<int> selectedActorIndex = new();
        readonly ReactiveProperty<int> selectedDomainIndex = new();

        ReadOnlyReactiveProperty<string[]> actorIdChoices;
        ReadOnlyReactiveProperty<object> selectedActorId;

        ReadOnlyReactiveProperty<string[]> domainNameChoices;
        ReadOnlyReactiveProperty<string> selectedDomainName;

        ReadOnlyReactiveProperty<ContextKey?> selectedContextKey;
        ReadOnlyReactiveProperty<AIDebugInfo> selectedDebugInfo;

        VisualElement worldStateContainer;

        protected virtual void OnEnable()
        {
            allDebugInfo.Value = new Dictionary<ContextKey, AIDebugInfo>();
            selectedActorIndex.Value = -1;
            selectedDomainIndex.Value = -1;

            var actorIds = allDebugInfo
                .Select(dict =>
                {
                    return dict.Keys
                        .Select(key => key.ActorId)
                        .Distinct()
                        .OrderBy(x => x.ToString())
                        .ToArray();
                })
                .ToReadOnlyReactiveProperty();

            actorIdChoices = actorIds
                .Select(ids => ids.Select(id => id.ToString()).ToArray())
                .ToReadOnlyReactiveProperty();

            selectedActorId = actorIds
                .CombineLatest(selectedActorIndex, (ids, index) =>
                {
                    return index >= 0 ? ids[index] : null;
                })
                .ToReadOnlyReactiveProperty();

            domainNameChoices = allDebugInfo
                .CombineLatest(selectedActorId, (dict, actorId) =>
                {
                    if (Equals(actorId, null))
                        return Array.Empty<string>();

                    return dict.Keys
                        .Where(key => Equals(key.ActorId, actorId))
                        .Select(key => key.DomainName)
                        .OrderBy(x => x)
                        .ToArray();
                })
                .ToReadOnlyReactiveProperty();

            selectedDomainName = domainNameChoices
                .CombineLatest(selectedDomainIndex, (domains, index) =>
                {
                    return index >= 0 ? domains[index] : null;
                })
                .ToReadOnlyReactiveProperty();

            selectedContextKey = allDebugInfo
                .CombineLatest(selectedActorId, selectedDomainName, (dict, actorId, domain) =>
                {
                    if (actorId == null || domain == null)
                        return (ContextKey?)null;

                    return new ContextKey(actorId, domain);
                })
                .ToReadOnlyReactiveProperty();

            selectedDebugInfo = selectedContextKey
                .Select(key =>
                {
                    if (!key.HasValue)
                        return null;

                    if (allDebugInfo.CurrentValue.TryGetValue(key.Value, out var info))
                    {
                        return info;
                    }

                    return null;
                })
                .ToReadOnlyReactiveProperty();

            EditorApplication.update += OnEditorUpdate;
        }

        protected virtual void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
            disposables.Clear();
        }

        void OnEditorUpdate()
        {
            if (!EditorApplication.isPlaying)
                return;
            if (!AIDebuggerBridge.IsInitialized)
                return;

            var latestAllDebugInfo = AIDebuggerBridge.GetAllDebugInfo();
            if (!allDebugInfo.Value.SequenceEqual(latestAllDebugInfo))
            {
                allDebugInfo.Value = latestAllDebugInfo.ToDictionary(x => x.Key, x => x.Value);
            }

            if (worldStateContainer != null && selectedDebugInfo?.CurrentValue != null)
            {
                RenderWorldState(worldStateContainer, selectedDebugInfo.CurrentValue.WorldState.CurrentValue);
            }
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
            var planList = root.Q<ListView>("plan-list");
            var logList = root.Q<ListView>("log-list");

            SetupActorList(actorList);
            SetupDomainToolbar(domainToolbar);
            SetupDetailListViews(planList, logList);
            BindToSelectedInfo(planList, logList);
        }

        void SetupActorList(ListView actorList)
        {
            actorList.makeItem = () =>
            {
                return new Label();
            };

            actorList.bindItem = (element, i) =>
            {
                ((Label)element).text = actorIdChoices.CurrentValue[i];
            };

            actorIdChoices.Subscribe(ids =>
            {
                actorList.itemsSource = ids;
                actorList.Rebuild();
            }).AddTo(disposables);

            actorList.selectionChanged += (selection) =>
            {
                selectedActorIndex.Value = actorList.selectedIndex;
            };

            selectedActorIndex.Subscribe(index =>
            {
                actorList.selectedIndex = index;
            }).AddTo(disposables);
        }

        void SetupDomainToolbar(VisualElement domainToolbar)
        {
            domainNameChoices.Subscribe(domains =>
            {
                domainToolbar.Clear();
                if (domains.Length == 0)
                    selectedDomainIndex.Value = -1;

                for (var i = 0; i < domains.Length; i++)
                {
                    var index = i;
                    var toggle = new ToolbarToggle
                    {
                        text = domains[i]
                    };
                    toggle.RegisterValueChangedCallback(evt =>
                    {
                        if (evt.newValue)
                            selectedDomainIndex.Value = index;
                    });
                    domainToolbar.Add(toggle);
                }
            }).AddTo(disposables);

            selectedDomainIndex.Subscribe(index =>
            {
                var toggles = domainToolbar.Query<ToolbarToggle>().ToList();
                for (var i = 0; i < toggles.Count; i++)
                {
                    toggles[i].SetValueWithoutNotify(i == index);
                }
            }).AddTo(disposables);
        }

        void SetupDetailListViews(ListView planList, ListView logList)
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

        void BindToSelectedInfo(ListView planList, ListView logList)
        {
            IDisposable actorBindings = null;

            selectedDebugInfo.Subscribe(info =>
            {
                actorBindings?.Dispose();
                actorBindings = null;

                if (info == null)
                {
                    ClearUI(planList, logList);
                    return;
                }

                actorBindings = BindDebugInfo(info, planList, logList);
            }).AddTo(disposables);

            Disposable.Create(() =>
            {
                actorBindings?.Dispose();
            }).AddTo(disposables);
        }

        void ClearUI(ListView planList, ListView logList)
        {
            worldStateContainer?.Clear();
            planList.itemsSource = null;
            planList.Rebuild();
            logList.itemsSource = null;
            logList.Rebuild();
        }

        IDisposable BindDebugInfo(AIDebugInfo info, ListView planList, ListView logList)
        {
            var bindings = new CompositeDisposable();

            IReadOnlyList<string> currentPlan = null;
            string currentTaskPath = null;

            planList.bindItem = (element, index) =>
            {
                if (currentPlan == null || index < 0 || index >= currentPlan.Count)
                    return;

                var item = currentPlan[index];
                var label = (Label)element;
                label.text = item;

                var isActive = currentTaskPath?.EndsWith(item) ?? false;
                label.EnableInClassList(ActiveItemClass, isActive);
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
                if (index < 0 || index >= logListSource.Count)
                    return;

                ((Label)element).text = logListSource[index];
            };

            logList.itemsSource = logListSource;
            logList.Rebuild();

            logs.ObserveAdd().Subscribe(e =>
            {
                logListSource.Insert(e.Index, e.Value);
                logList.RefreshItems();
                logList.ScrollToItem(logListSource.Count - 1);
            }).AddTo(bindings);

            logs.ObserveRemove().Subscribe(e =>
            {
                logListSource.RemoveAt(e.Index);
                logList.RefreshItems();
            }).AddTo(bindings);

            return bindings;
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
                {
                    valueLabel.AddToClassList(boolValue ? "world-state-value--true" : "world-state-value--false");
                }

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
