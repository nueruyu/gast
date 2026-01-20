using System.Collections.Generic;

namespace Gast.Lib.AI.Debugging
{
    public interface IAIDebugger
    {
        void Register(ContextKey contextKey);

        void Unregister(ContextKey contextKey);

        AIDebugInfo GetDebugInfo(ContextKey contextKey);

        IReadOnlyDictionary<ContextKey, AIDebugInfo> GetAllDebugInfo();

        void UpdateWorldState(ContextKey contextKey, string worldState);

        void UpdatePlan(ContextKey contextKey, IReadOnlyList<string> plan);

        void UpdateActiveTaskPath(ContextKey contextKey, string taskPath);

        void AddLog(ContextKey contextKey, string log);
    }
}