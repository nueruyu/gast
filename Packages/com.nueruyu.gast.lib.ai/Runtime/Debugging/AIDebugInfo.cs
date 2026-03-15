using ObservableCollections;
using R3;
using System.Collections.Generic;
using System.Linq;

namespace Gast.Lib.AI.Debugging
{
    public class AIDebugInfo
    {
        readonly Stack<string> taskStack = new();

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

        internal void EnterTask(string taskName)
        {
            taskStack.Push(taskName);
            ActiveTaskPath.Value = string.Join(" / ", taskStack.Reverse());
        }

        internal void ExitTask()
        {
            if (taskStack.Count > 0)
            {
                taskStack.Pop();
                ActiveTaskPath.Value = taskStack.Count > 0
                    ? string.Join(" / ", taskStack.Reverse())
                    : "";
            }
        }
    }
}
