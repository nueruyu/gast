using System;
using System.Collections.Generic;
using ObservableCollections;

namespace Gast.Lib.AI.Debugging
{
    public class AIDebugger : IContextRegistry, IAIDebugger
    {
        readonly ObservableDictionary<ContextKey, AIDebugInfo> allDebugInfo = new();
        int registrationCounter;

        public IReadOnlyObservableDictionary<ContextKey, AIDebugInfo> AllDebugInfo => allDebugInfo;

        public void UpdatePlan(ContextKey contextKey, IReadOnlyList<string> plan)
        {
            WithInfo(contextKey, info => info.CurrentPlan.Value = plan);
        }

        public void UpdateCurrentMethod(ContextKey contextKey, string methodName)
        {
            WithInfo(contextKey, info => info.CurrentMethodName.Value = methodName);
        }

        public void EnterTask(ContextKey contextKey, string taskName)
        {
            WithInfo(contextKey, info => info.EnterTask(taskName));
        }

        public void ExitTask(ContextKey contextKey)
        {
            WithInfo(contextKey, info => info.ExitTask());
        }

        public void AddLog(ContextKey contextKey, string log)
        {
            WithInfo(contextKey, info =>
            {
                var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                info.Logs.Add($"[{timestamp}] {log}");
                if (info.Logs.Count > 100)
                    info.Logs.RemoveAt(0);
            });
        }

        public void Register(ContextKey contextKey, object worldState, string actorName)
        {
            if (!allDebugInfo.ContainsKey(contextKey))
                allDebugInfo.Add(contextKey, new AIDebugInfo(contextKey, actorName, worldState, registrationCounter++));
        }

        public void Unregister(ContextKey contextKey)
        {
            allDebugInfo.Remove(contextKey);
        }

        void WithInfo(ContextKey contextKey, Action<AIDebugInfo> action)
        {
            if (allDebugInfo.TryGetValue(contextKey, out var info))
                action(info);
        }
    }
}