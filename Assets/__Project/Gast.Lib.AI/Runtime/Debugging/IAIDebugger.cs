using System.Collections.Generic;

namespace Gast.Lib.AI.Debugging
{
    public interface IAIDebugger
    {
        IReadOnlyDictionary<ContextKey, AIDebugInfo> GetAllDebugInfo();

        void UpdatePlan(ContextKey contextKey, IReadOnlyList<string> plan);

        void UpdateActiveTaskPath(ContextKey contextKey, string taskPath);

        void AddLog(ContextKey contextKey, string log);
    }
}
