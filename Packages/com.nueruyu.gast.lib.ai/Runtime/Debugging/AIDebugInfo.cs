using ObservableCollections;
using R3;
using System.Collections.Generic;

namespace Gast.Lib.AI.Debugging
{
    public class AIDebugInfo
    {
        public ContextKey ContextKey { get; }
        public object WorldState { get; }
        public ReactiveProperty<string> ActiveTaskPath { get; } = new("");
        public ReactiveProperty<IReadOnlyList<string>> CurrentPlan { get; } = new(System.Array.Empty<string>());
        public ObservableList<string> Logs { get; } = new();

        public AIDebugInfo(ContextKey contextKey, object worldState)
        {
            ContextKey = contextKey;
            WorldState = worldState;
        }
    }
}
