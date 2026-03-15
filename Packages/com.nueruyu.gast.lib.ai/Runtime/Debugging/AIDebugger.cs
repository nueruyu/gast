using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Gast.Lib.AI.Debugging
{
    public class AIDebugger : IContextRegistry, IAIDebugger
    {
        readonly ConcurrentDictionary<ContextKey, AIDebugInfo> debugInfoMap = new();

        public void Register(ContextKey contextKey, object worldState, string actorName)
        {
            var added = debugInfoMap.TryAdd(contextKey, new AIDebugInfo(contextKey, actorName, worldState));
            Debug.Log($"[AIDebugger] Register: {contextKey}, Added: {added}, Total: {debugInfoMap.Count}");
        }

        public void Unregister(ContextKey contextKey)
        {
            debugInfoMap.TryRemove(contextKey, out _);
        }

        public IReadOnlyDictionary<ContextKey, AIDebugInfo> GetAllDebugInfo()
        {
            return debugInfoMap;
        }

        public void UpdatePlan(ContextKey contextKey, IReadOnlyList<string> plan)
        {
            if (debugInfoMap.TryGetValue(contextKey, out var info))
            {
                info.CurrentPlan.Value = plan;
            }
        }

        public void UpdateCurrentMethod(ContextKey contextKey, string methodName)
        {
            if (debugInfoMap.TryGetValue(contextKey, out var info))
            {
                info.CurrentMethodName.Value = methodName;
            }
        }

        public void UpdateActiveTaskPath(ContextKey contextKey, string taskPath)
        {
            if (debugInfoMap.TryGetValue(contextKey, out var info))
            {
                info.ActiveTaskPath.Value = taskPath;
            }
        }

        public void AddLog(ContextKey contextKey, string log)
        {
            if (debugInfoMap.TryGetValue(contextKey, out var info))
            {
                var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                info.Logs.Add($"[{timestamp}] {log}");
                if (info.Logs.Count > 100)
                {
                    info.Logs.RemoveAt(0);
                }
            }
        }
    }
}
