using System.Collections.Generic;
using System.Linq;

namespace Gast.Lib.AI.Debugging
{
    public static class DebugLogger
    {
        static readonly Dictionary<ContextKey, Stack<string>> taskStacks = new();
        public static bool EnableLogging { get; set; } = true;

        public static void Log(ContextKey contextKey, string message)
        {
            if (!EnableLogging || !AIDebuggerBridge.IsInitialized)
                return;
            AIDebuggerBridge.Debugger.AddLog(contextKey, message);
        }

        public static void LogMethodSelected<TWorldState>(
            ContextKey contextKey,
            string compoundTaskName,
            string methodName,
            TWorldState state)
        {
            Log(contextKey, $"Method selected - [{compoundTaskName}] Selected method: '{methodName}'");
        }

        public static void LogPlan(ContextKey contextKey, IEnumerable<ITask> plan)
        {
            if (!AIDebuggerBridge.IsInitialized)
                return;

            var planNames = plan.Select(p => p.Name).ToArray();
            AIDebuggerBridge.Debugger.UpdatePlan(contextKey, planNames);

            Log(contextKey, $"Planning complete. Plan has {planNames.Length} actions.");
        }

        public static void LogPlanFailed(ContextKey contextKey, string reason)
        {
            Log(contextKey, $"Planning failed: {reason}");
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

        static void UpdateActiveTaskPath(ContextKey contextKey)
        {
            if (taskStacks.TryGetValue(contextKey, out var stack) && stack.Count > 0)
            {
                var path = string.Join(" / ", stack.Reverse());
                AIDebuggerBridge.Debugger.UpdateActiveTaskPath(contextKey, path);
            }
            else
            {
                AIDebuggerBridge.Debugger.UpdateActiveTaskPath(contextKey, "");
            }
        }

        public static void ClearContext(ContextKey contextKey)
        {
            taskStacks.Remove(contextKey);
        }
    }
}