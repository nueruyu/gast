using ObservableCollections;
using R3;
using System.Collections.Generic;

namespace Gast.Lib.AI.Debugging
{
    public class AIDebugInfo
    {
        public object ActorId { get; }
        public ReactiveProperty<string> WorldStateText { get; } = new("");
        public ReactiveProperty<string> ActiveTaskPath { get; } = new("");
        public ReactiveProperty<IReadOnlyList<string>> CurrentPlan { get; } = new(System.Array.Empty<string>());
        public ObservableList<string> Logs { get; } = new();

        public AIDebugInfo(object actorId)
        {
            ActorId = actorId;
        }
    }
}
