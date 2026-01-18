using Gast.Lib.AI.Debugging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Gast.Features.Debugging
{
    public class AIDebugger : IAIDebugger
    {
        readonly ConcurrentDictionary<object, AIDebugInfo> debugInfoMap = new();

        public void Register(object actorId)
        {
            var added = debugInfoMap.TryAdd(actorId, new AIDebugInfo(actorId));
            Debug.Log($"[AIDebugger] Register: {actorId}, Added: {added}, Total: {debugInfoMap.Count}");
        }

        public void Unregister(object actorId)
        {
            debugInfoMap.TryRemove(actorId, out _);
        }

        public AIDebugInfo GetDebugInfo(object actorId)
        {
            debugInfoMap.TryGetValue(actorId, out var info);
            return info;
        }

        public IReadOnlyDictionary<object, AIDebugInfo> GetAllDebugInfo()
        {
            return debugInfoMap;
        }

        public void UpdateWorldState(object actorId, string worldState)
        {
            if (debugInfoMap.TryGetValue(actorId, out var info))
            {
                info.WorldStateText.Value = worldState;
            }
        }

        public void UpdatePlan(object actorId, IReadOnlyList<string> plan)
        {
            if (debugInfoMap.TryGetValue(actorId, out var info))
            {
                info.CurrentPlan.Value = plan;
            }
        }

        public void UpdateActiveTaskPath(object actorId, string taskPath)
        {
            if (debugInfoMap.TryGetValue(actorId, out var info))
            {
                info.ActiveTaskPath.Value = taskPath;
            }
        }

        public void AddLog(object actorId, string log)
        {
            if (debugInfoMap.TryGetValue(actorId, out var info))
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
