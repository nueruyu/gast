using Gast.Lib.AI;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Gast.Lib.AI.Debugging
{
    public class AIDebugger : IAIDebugger
    {
        readonly ConcurrentDictionary<ContextKey, AIDebugInfo> debugInfoMap = new();

        public void Register(ContextKey contextKey)
        {
            var added = debugInfoMap.TryAdd(contextKey, new AIDebugInfo(contextKey));
            Debug.Log($"[AIDebugger] Register: {contextKey}, Added: {added}, Total: {debugInfoMap.Count}");
        }

        public void Unregister(ContextKey contextKey)
        {
            debugInfoMap.TryRemove(contextKey, out _);
        }

        public AIDebugInfo GetDebugInfo(ContextKey contextKey)
        {
            debugInfoMap.TryGetValue(contextKey, out var info);
            return info;
        }

        public IReadOnlyDictionary<ContextKey, AIDebugInfo> GetAllDebugInfo()
        {
            return debugInfoMap;
        }

        public void UpdateWorldState(ContextKey contextKey, string worldState)
        {
            if (debugInfoMap.TryGetValue(contextKey, out var info))
            {
                info.WorldStateText.Value = worldState;
            }
        }

        public void UpdatePlan(ContextKey contextKey, IReadOnlyList<string> plan)
        {
            if (debugInfoMap.TryGetValue(contextKey, out var info))
            {
                info.CurrentPlan.Value = plan;
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
                info.Logs.Add(log);
                if (info.Logs.Count > 100)
                {
                    info.Logs.RemoveAt(0);
                }
            }
        }
    }
}