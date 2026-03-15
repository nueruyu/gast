using Gast.Lib.AI.Debugging;
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

        private struct ActorInfo
        {
            public object Id;
            public string Name;
        }

        [SerializeField] VisualTreeAsset visualTreeAsset;

        readonly CompositeDisposable disposables = new();

        readonly ReactiveProperty<IReadOnlyDictionary<ContextKey, AIDebugInfo>> allDebugInfo = new();
        readonly ReactiveProperty<int> selectedActorIndex = new();
        readonly ReactiveProperty<int> selectedDomainIndex = new();
        readonly ReactiveProperty<bool> logAutoScroll = new(true);

        ReadOnlyReactiveProperty<ActorInfo[]> actors;
        ReadOnlyReactiveProperty<object> selectedActorId;

        ReadOnlyReactiveProperty<string[]> domainNameChoices;
        ReadOnlyReactiveProperty<string> selectedDomainName;

        ReadOnlyReactiveProperty<AIDebugInfo> selectedDebugInfo;

        VisualElement worldStateContainer;
        Label currentMethodLabel;

        protected virtual void OnEnable()
        {
            allDebugInfo.Value = new Dictionary<ContextKey, AIDebugInfo>();
            selectedActorIndex.Value = -1;
            selectedDomainIndex.Value = -1;

            actors = allDebugInfo
                .Select(dict => dict.Values
                    .Select(info => new ActorInfo { Id = info.ContextKey.ActorId, Name = info.ActorName })
                    .DistinctBy(a => a.Id)
                    .ToArray())
                .ToReadOnlyReactiveProperty();

            selectedActorId = actors
                .CombineLatest(selectedActorIndex, (actorInfos, index) =>
                {
                    return index >= 0 && index < actorInfos.Length ? actorInfos[index].Id : null;
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
                    return index >= 0 && index < domains.Length ? domains[index] : null;
                })
                .ToReadOnlyReactiveProperty();

            selectedDebugInfo = allDebugInfo
                .CombineLatest(selectedActorId, selectedDomainName, (dict, actorId, domain) =>
                {
                    if (actorId == null || domain == null)
                        return null;
                    dict.TryGetValue(new ContextKey(actorId, domain), out var info);
                    return info;
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
            if (!DictionaryEquals(allDebugInfo.Value, latestAllDebugInfo))
            {
                allDebugInfo.Value = latestAllDebugInfo.ToDictionary(x => x.Key, x => x.Value);
            }

            if (worldStateContainer != null && selectedDebugInfo?.CurrentValue != null)
            {
                RenderWorldState(worldStateContainer, selectedDebugInfo.CurrentValue.WorldState);
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
            currentMethodLabel = root.Q<Label>("current-method-label");
            var planList = root.Q<ListView>("plan-list");
            var logList = root.Q<ListView>("log-list");
            var logClearButton = root.Q<Button>("log-clear-button");
            var logAutoScrollToggle = root.Q<Toggle>("log-autoscroll-toggle");

            logAutoScrollToggle.value = logAutoScroll.Value;
            logAutoScrollToggle.RegisterValueChangedCallback(evt => logAutoScroll.Value = evt.newValue);

            SetupActorList(actorList);
            SetupDomainToolbar(domainToolbar);
            SetupDetailListViews(planList, logList);
            BindToSelectedInfo(planList, logList, logClearButton);
        }

        void SetupActorList(ListView actorList)
        {
            actorList.makeItem = () =>
            {
                return new Label();
            };

            actorList.bindItem = (element, i) =>
            {
                var actor = actors.CurrentValue[i];
                ((Label)element).text = $"{actor.Name} ({actor.Id})";
            };

            actors.Subscribe(actorInfos =>
            {
                actorList.itemsSource = actorInfos;
                actorList.Rebuild();
            }).AddTo(disposables);

            Action<IEnumerable<object>> onSelectionChanged = (selection) =>
            {
                selectedActorIndex.Value = actorList.selectedIndex;
            };
            actorList.selectionChanged += onSelectionChanged;
            disposables.Add(Disposable.Create(() => actorList.selectionChanged -= onSelectionChanged));

            selectedActorIndex.Subscribe(index =>
            {
                actorList.selectedIndex = index;
            }).AddTo(disposables);
        }

        void SetupDomainToolbar(VisualElement domainToolbar)
        {
            CompositeDisposable toggleDisposables = null;

            domainNameChoices.Subscribe(domains =>
            {
                toggleDisposables?.Dispose();
                toggleDisposables = new CompositeDisposable();

                domainToolbar.Clear();

                if (domains.Length > 0)
                {
                    if (selectedDomainIndex.Value < 0 || selectedDomainIndex.Value >= domains.Length)
                    {
                        selectedDomainIndex.Value = 0;
                    }
                }
                else
                {
                    selectedDomainIndex.Value = -1;
                }

                for (var i = 0; i < domains.Length; i++)
                {
                    var index = i;
                    var toggle = new ToolbarToggle
                    {
                        text = domains[i]
                    };
                    toggle.SetValueWithoutNotify(index == selectedDomainIndex.Value);

                    var callback = new EventCallback<ChangeEvent<bool>>(evt =>
                    {
                        if (evt.newValue)
                            selectedDomainIndex.Value = index;
                    });
                    toggle.RegisterValueChangedCallback(callback);
                    toggleDisposables.Add(Disposable.Create(() => toggle.UnregisterValueChangedCallback(callback)));
                    domainToolbar.Add(toggle);
                }
            }).AddTo(disposables);

            Disposable.Create(() => toggleDisposables?.Dispose()).AddTo(disposables);

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

        void BindToSelectedInfo(ListView planList, ListView logList, Button logClearButton)
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

                actorBindings = BindDebugInfo(info, planList, logList, logClearButton);
            }).AddTo(disposables);

            Disposable.Create(() =>
            {
                actorBindings?.Dispose();
            }).AddTo(disposables);
        }

        void ClearUI(ListView planList, ListView logList)
        {
            worldStateContainer?.Clear();
            if (currentMethodLabel != null)
                currentMethodLabel.text = "";
            planList.itemsSource = null;
            planList.Rebuild();
            logList.itemsSource = null;
            logList.Rebuild();
        }

        IDisposable BindDebugInfo(AIDebugInfo info, ListView planList, ListView logList, Button logClearButton)
        {
            var bindings = new CompositeDisposable();

            info.CurrentMethodName.SubscribeToText(currentMethodLabel).AddTo(bindings);

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
                if (logAutoScroll.Value)
                {
                    logList.ScrollToItem(logListSource.Count - 1);
                }
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

            var logClearCallback = new EventCallback<ClickEvent>(evt => logs.Clear());
            logClearButton.RegisterCallback(logClearCallback);
            bindings.Add(Disposable.Create(() => logClearButton.UnregisterCallback(logClearCallback)));

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

        static bool DictionaryEquals(
            IReadOnlyDictionary<ContextKey, AIDebugInfo> a,
            IReadOnlyDictionary<ContextKey, AIDebugInfo> b)
        {
            if (a.Count != b.Count)
                return false;

            foreach (var kvp in a)
            {
                if (!b.TryGetValue(kvp.Key, out var bValue))
                    return false;
                if (!ReferenceEquals(kvp.Value, bValue))
                    return false;
            }

            return true;
        }
    }
}
