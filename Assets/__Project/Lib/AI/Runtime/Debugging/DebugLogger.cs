using System.Collections.Generic;
using System.Linq;

namespace Gast.Lib.AI.Debugging
{
    public static class DebugLogger
    {
        public static bool EnableLogging { get; set; } = true;

        static readonly Dictionary<ContextKey, Stack<string>> taskStacks = new();

        public static void LogMethodSelected<TWorldState>(ContextKey contextKey, string compoundTaskName, string methodName, TWorldState state)
        {
            if (!EnableLogging || !AIDebuggerBridge.IsInitialized)
                return;
            AIDebuggerBridge.Instance.AddLog(contextKey, $"{compoundTaskName} -> Selected method '{methodName}'");
        }

        public static void LogPlan(ContextKey contextKey, IEnumerable<ITask> plan)
        {
            if (!EnableLogging || !AIDebuggerBridge.IsInitialized)
                return;
            var planNames = plan.Select(p => p.Name).ToArray();
            AIDebuggerBridge.Instance.UpdatePlan(contextKey, planNames);
            AIDebuggerBridge.Instance.AddLog(contextKey, $"Planning complete. Plan has {planNames.Length} actions.");
        }

        public static void LogPlanFailed(ContextKey contextKey, string reason)
        {
            if (!EnableLogging || !AIDebuggerBridge.IsInitialized)
                return;
            AIDebuggerBridge.Instance.AddLog(contextKey, $"Planning failed: {reason}");
        }

        public static void EnterTask(ContextKey contextKey, string taskName)
        {
            if (!EnableLogging || !AIDebuggerBridge.IsInitialized)
                return;

            if (!taskStacks.TryGetValue(contextKey, out var stack))
            {
                stack = new Stack<string>();
                taskStacks[contextKey] = stack;
            }
            stack.Push(taskName);
            UpdateActiveTaskPath(contextKey);
        }

        public static void ExitTask(ContextKey contextKey)
        {
            if (!EnableLogging || !AIDebuggerBridge.IsInitialized)
                return;

            if (taskStacks.TryGetValue(contextKey, out var stack) && stack.Count > 0)
            {
                stack.Pop();
                UpdateActiveTaskPath(contextKey);
            }
        }

        private static void UpdateActiveTaskPath(ContextKey contextKey)
        {
            if (taskStacks.TryGetValue(contextKey, out var stack) && stack.Count > 0)
            {
                var path = string.Join(" / ", stack.Reverse());
                AIDebuggerBridge.Instance.UpdateActiveTaskPath(contextKey, path);
            }
            else
            {
                AIDebuggerBridge.Instance.UpdateActiveTaskPath(contextKey, "");
            }
        }
    }
}