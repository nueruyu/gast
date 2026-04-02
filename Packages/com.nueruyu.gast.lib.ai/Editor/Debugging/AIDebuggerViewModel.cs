using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Lib.AI.Debugging;
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
        readonly ReactiveProperty<IReadOnlyDictionary<ContextKey, AIDebugInfo>> allDebugInfo = new();
        readonly CompositeDisposable disposables = new();

        ReadOnlyReactiveProperty<object> selectedActorId;
        ReadOnlyReactiveProperty<string> selectedDomainName;

        public ReactiveProperty<bool> LogAutoScroll { get; } = new(true);
        public ReactiveProperty<object> SelectedActorId { get; } = new((object)null);
        public ReactiveProperty<int> SelectedDomainIndex { get; } = new(-1);

        public ReadOnlyReactiveProperty<ActorInfo[]> Actors { get; }
        public ReadOnlyReactiveProperty<string[]> DomainNameChoices { get; }
        public ReadOnlyReactiveProperty<AIDebugInfo> SelectedDebugInfo { get; }

        public AIDebuggerViewModel()
        {
            allDebugInfo.Value = new Dictionary<ContextKey, AIDebugInfo>();

            Actors = allDebugInfo
                .Select(dict => dict.Values
                    .GroupBy(info => info.ContextKey.ActorId)
                    .OrderBy(g => g.Min(info => info.RegistrationIndex))
                    .Select(g => new ActorInfo { Id = g.Key, Name = g.First().ActorName })
                    .ToArray())
                .ToReadOnlyReactiveProperty();

            selectedActorId = SelectedActorId.ToReadOnlyReactiveProperty();

            DomainNameChoices = allDebugInfo
                .CombineLatest(selectedActorId, (dict, actorId) =>
                {
                    if (actorId == null) return Array.Empty<string>();
                    return dict.Keys
                        .Where(key => Equals(key.ActorId, actorId))
                        .Select(key => key.DomainName)
                        .OrderBy(x => x)
                        .ToArray();
                })
                .ToReadOnlyReactiveProperty();

            selectedDomainName = DomainNameChoices
                .CombineLatest(SelectedDomainIndex,
                    (domains, index) => index >= 0 && index < domains.Length ? domains[index] : null)
                .ToReadOnlyReactiveProperty();

            SelectedDebugInfo = allDebugInfo
                .CombineLatest(selectedActorId, selectedDomainName, (dict, actorId, domain) =>
                {
                    if (actorId == null || domain == null) return null;
                    dict.TryGetValue(new ContextKey(actorId, domain), out var info);
                    return info;
                })
                .ToReadOnlyReactiveProperty();
        }

        public void Update()
        {
            if (!AIDebuggerBridge.IsInitialized)
            {
                if (allDebugInfo.Value.Count > 0)
                {
                    allDebugInfo.Value = new Dictionary<ContextKey, AIDebugInfo>();
                }
                return;
            }

            var latestAllDebugInfo = AIDebuggerBridge.GetAllDebugInfo();
            if (!DictionaryEquals(allDebugInfo.Value, latestAllDebugInfo))
                allDebugInfo.Value = latestAllDebugInfo.ToDictionary(x => x.Key, x => x.Value);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

        static bool DictionaryEquals(
            IReadOnlyDictionary<ContextKey, AIDebugInfo> a,
            IReadOnlyDictionary<ContextKey, AIDebugInfo> b)
        {
            if (a == null || b == null) return false;
            if (a.Count != b.Count) return false;

            foreach (var kvp in a)
            {
                if (!b.TryGetValue(kvp.Key, out var bValue)) return false;
                if (!ReferenceEquals(kvp.Value, bValue)) return false;
            }

            return true;
        }
    }
}
