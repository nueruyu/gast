using UnityEngine;

namespace Gast.Lib.AI
{
    /// <summary>
    /// Simple logging system for HTN planning and execution.
    /// Can be toggled on/off for debugging vs production.
    /// </summary>
    public static class DebugLogger
    {
        /// <summary>
        /// Enable or disable HTN logging.
        /// </summary>
        public static bool EnableLogging { get; set; } = false;

        public static void LogPlanning<TWorldState>(TWorldState state)
        {
            if (!EnableLogging)
                return;
            Debug.Log($"[HTN] Starting planning with state: {state}");
        }

        public static void LogMethodSelected<TWorldState>(
            string compoundTaskName,
            string methodName,
            TWorldState state)
        {
            if (!EnableLogging)
                return;
            Debug.Log($"[HTN] {compoundTaskName} -> Selected method '{methodName}' (state: {state})");
        }

        public static void LogPrimitiveAdded(string primitiveName)
        {
            if (!EnableLogging)
                return;
            Debug.Log($"[HTN] Added primitive to plan: {primitiveName}");
        }

        public static void LogPlanComplete(int actionCount)
        {
            if (!EnableLogging)
                return;
            Debug.Log($"[HTN] Planning complete. Plan has {actionCount} actions.");
        }

        public static void LogPlanFailed(string reason)
        {
            if (!EnableLogging)
                return;
            Debug.LogWarning($"[HTN] Planning failed: {reason}");
        }

        public static void LogExecutingAction(string actionName)
        {
            if (!EnableLogging)
                return;
            Debug.Log($"[HTN] Executing: {actionName}");
        }
    }
}