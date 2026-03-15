using ObservableCollections;
using R3;
using System.Collections.Generic;

namespace Gast.Lib.AI.Debugging
{
    public class AIDebugInfo
    {
        public ContextKey ContextKey { get; }
        public string ActorName { get; }
        public object WorldState { get; }
        public ReactiveProperty<string> ActiveTaskPath { get; } = new("");
        public ReactiveProperty<IReadOnlyList<string>> CurrentPlan { get; } = new(System.Array.Empty<string>());
        public ReactiveProperty<string> CurrentMethodName { get; } = new("");
        public ObservableList<string> Logs { get; } = new();

        public AIDebugInfo(ContextKey contextKey, string actorName, object worldState)
        {
            ContextKey = contextKey;
            ActorName = actorName;
            WorldState = worldState;
        }
    }
}
