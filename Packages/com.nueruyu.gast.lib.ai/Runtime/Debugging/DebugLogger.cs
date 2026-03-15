using System.Collections.Generic;
using System.Linq;

namespace Gast.Lib.AI.Debugging
{
    public static class DebugLogger
    {
        public static bool EnableLogging { get; set; } = true;

        static bool CanLog => EnableLogging && AIDebuggerBridge.IsInitialized;

        public static void Log(ContextKey contextKey, string message)
        {
            if (CanLog) AIDebuggerBridge.Debugger.AddLog(contextKey, message);
        }

        public static void LogMethodSelected<TWorldState>(
            ContextKey contextKey,
            string compoundTaskName,
            string methodName,
            TWorldState state)
        {
            if (!CanLog) return;
            AIDebuggerBridge.Debugger.UpdateCurrentMethod(contextKey, methodName);
            AIDebuggerBridge.Debugger.AddLog(contextKey, $"{compoundTaskName} -> Selected method '{methodName}'");
        }

        public static void LogPlan(ContextKey contextKey, IEnumerable<ITask> plan)
        {
            if (!CanLog) return;
            var planNames = plan.Select(p => p.Name).ToArray();
            AIDebuggerBridge.Debugger.UpdatePlan(contextKey, planNames);
            AIDebuggerBridge.Debugger.AddLog(contextKey, $"Planning complete. Plan has {planNames.Length} actions.");
        }

        public static void LogPlanFailed(ContextKey contextKey, string reason)
        {
            Log(contextKey, $"Planning failed: {reason}");
        }

        public static void EnterTask(ContextKey contextKey, string taskName)
        {
            if (CanLog) AIDebuggerBridge.Debugger.EnterTask(contextKey, taskName);
        }

        public static void ExitTask(ContextKey contextKey)
        {
            if (CanLog) AIDebuggerBridge.Debugger.ExitTask(contextKey);
        }
    }
}
