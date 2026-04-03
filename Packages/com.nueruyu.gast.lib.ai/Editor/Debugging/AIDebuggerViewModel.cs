using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Lib.AI.Debugging;
using ObservableCollections;
using R3;

namespace Gast.Lib.AI.Editor.Debugging
{
    public struct ActorInfo
    {
        public object Id;
        public string Name;
    }

    public class AIDebuggerViewModel : IDisposable
    {
        readonly Dictionary<object, int> actorReferenceCounts = new();
        readonly CompositeDisposable disposables = new();
        CompositeDisposable debugInfoSubscriptions;

        public AIDebuggerViewModel()
        {
            SelectedDebugInfo = SelectedActorId
                .CombineLatest(
                    SelectedDomainIndex,
                    (actorId, domainIndex) =>
                    {
                        if (actorId == null ||
                            domainIndex < 0 ||
                            domainIndex >= DomainNameChoices.Count ||
                            !AIDebuggerBridge.IsInitialized.CurrentValue)
                            return null;

                        var domainName = DomainNameChoices[domainIndex];
                        var key = new ContextKey(actorId, domainName);
                        AIDebuggerBridge.AllDebugInfo.TryGetValue(key, out var info);
                        return info;
                    })
                .ToReadOnlyReactiveProperty();

            SelectedActorId
                .Subscribe(UpdateDomainNameChoices)
                .AddTo(disposables);

            AIDebuggerBridge.IsInitialized
                .Subscribe(OnInitializedChanged)
                .AddTo(disposables);
        }

        public ReactiveProperty<bool> LogAutoScroll { get; } = new(true);
        public ReactiveProperty<object> SelectedActorId { get; } = new(null);
        public ReactiveProperty<int> SelectedDomainIndex { get; } = new(-1);

        public ObservableList<ActorInfo> Actors { get; } = new();
        public ObservableList<string> DomainNameChoices { get; } = new();
        public ReadOnlyReactiveProperty<AIDebugInfo> SelectedDebugInfo { get; }

        public void Dispose()
        {
            debugInfoSubscriptions?.Dispose();
            disposables.Dispose();
        }

        void OnInitializedChanged(bool initialized)
        {
            debugInfoSubscriptions?.Dispose();
            OnDebugInfoReset();

            if (!initialized)
                return;

            var debugInfoDict = AIDebuggerBridge.AllDebugInfo;

            debugInfoSubscriptions = new CompositeDisposable();

            debugInfoDict.ObserveAdd()
                .Subscribe(e => OnDebugInfoAdded(e.Value.Value))
                .AddTo(debugInfoSubscriptions);
            debugInfoDict.ObserveRemove()
                .Subscribe(e => OnDebugInfoRemoved(e.Value.Value))
                .AddTo(debugInfoSubscriptions);
            debugInfoDict.ObserveReset()
                .Subscribe(_ => OnDebugInfoReset())
                .AddTo(debugInfoSubscriptions);

            foreach (var (_, info) in debugInfoDict)
                OnDebugInfoAdded(info);
        }

        void OnDebugInfoAdded(AIDebugInfo info)
        {
            var actorId = info.ContextKey.ActorId;
            if (actorReferenceCounts.TryAdd(actorId, 0))
                Actors.Add(new ActorInfo { Id = actorId, Name = info.ActorName });

            actorReferenceCounts[actorId]++;

            if (Equals(SelectedActorId.Value, actorId))
                DomainNameChoices.Add(info.ContextKey.DomainName);
        }

        void OnDebugInfoRemoved(AIDebugInfo info)
        {
            var actorId = info.ContextKey.ActorId;
            if (actorReferenceCounts.ContainsKey(actorId))
            {
                actorReferenceCounts[actorId]--;
                if (actorReferenceCounts[actorId] <= 0)
                {
                    actorReferenceCounts.Remove(actorId);

                    var actorToRemove = Actors.First(a => Equals(a.Id, actorId));
                    Actors.Remove(actorToRemove);
                }
            }

            if (Equals(SelectedActorId.Value, actorId))
                DomainNameChoices.Remove(info.ContextKey.DomainName);
        }

        void OnDebugInfoReset()
        {
            actorReferenceCounts.Clear();
            Actors.Clear();
            DomainNameChoices.Clear();
        }

        void UpdateDomainNameChoices(object actorId)
        {
            if (actorId == null ||
                !AIDebuggerBridge.IsInitialized.CurrentValue)
            {
                DomainNameChoices.Clear();
                return;
            }

            var newDomainNames = AIDebuggerBridge.AllDebugInfo
                .Where(kvp => Equals(kvp.Key.ActorId, actorId))
                .OrderBy(kvp => kvp.Value.RegistrationIndex)
                .Select(kvp => kvp.Key.DomainName)
                .ToArray();

            if (!DomainNameChoices.SequenceEqual(newDomainNames))
            {
                DomainNameChoices.Clear();
                DomainNameChoices.AddRange(newDomainNames);
            }
        }
    }
}