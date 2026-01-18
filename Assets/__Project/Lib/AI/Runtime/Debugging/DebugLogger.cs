using Gast.Lib.AI.Debugging;
using System.Collections.Generic;
using System.Linq;

namespace Gast.Lib.AI.Debugging
{
    public static class DebugLogger
    {
        public static bool EnableLogging { get; set; } = true;

        static readonly Dictionary<object, Stack<string>> taskStacks = new();

        public static void LogMethodSelected<TWorldState>(object actorId, string compoundTaskName, string methodName, TWorldState state)
        {
            if (!EnableLogging || !AIDebuggerBridge.IsInitialized) return;
            AIDebuggerBridge.Instance.AddLog(actorId, $"{compoundTaskName} -> Selected method '{methodName}'");
        }

        public static void LogPlan(object actorId, IReadOnlyList<ITask> plan)
        {
            if (!EnableLogging || !AIDebuggerBridge.IsInitialized) return;
            var planNames = plan.Select(p => p.Name).ToList();
            AIDebuggerBridge.Instance.UpdatePlan(actorId, planNames);
            AIDebuggerBridge.Instance.AddLog(actorId, $"Planning complete. Plan has {plan.Count} actions.");
        }

        public static void LogPlanFailed(object actorId, string reason)
        {
            if (!EnableLogging || !AIDebuggerBridge.IsInitialized) return;
            AIDebuggerBridge.Instance.AddLog(actorId, $"Planning failed: {reason}");
        }

        public static void EnterTask(object actorId, string taskName)
        {
            if (!EnableLogging || !AIDebuggerBridge.IsInitialized) return;

            if (!taskStacks.TryGetValue(actorId, out var stack))
            {
                stack = new Stack<string>();
                taskStacks[actorId] = stack;
            }
            stack.Push(taskName);
            UpdateActiveTaskPath(actorId);
        }

        public static void ExitTask(object actorId)
        {
            if (!EnableLogging || !AIDebuggerBridge.IsInitialized) return;

            if (taskStacks.TryGetValue(actorId, out var stack) && stack.Count > 0)
            {
                stack.Pop();
                UpdateActiveTaskPath(actorId);
            }
        }

        private static void UpdateActiveTaskPath(object actorId)
        {
            if (taskStacks.TryGetValue(actorId, out var stack) && stack.Count > 0)
            {
                var path = string.Join(" / ", stack.Reverse());
                AIDebuggerBridge.Instance.UpdateActiveTaskPath(actorId, path);
            }
            else
            {
                AIDebuggerBridge.Instance.UpdateActiveTaskPath(actorId, "");
            }
        }
    }
}
