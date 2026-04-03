using System.Collections.Generic;
using ObservableCollections;

namespace Gast.Lib.AI.Debugging
{
    public interface IAIDebugger
    {
        IReadOnlyObservableDictionary<ContextKey, AIDebugInfo> AllDebugInfo { get; }

        void UpdatePlan(ContextKey contextKey, IReadOnlyList<string> plan);

        void UpdateCurrentMethod(ContextKey contextKey, string methodName);

        void EnterTask(ContextKey contextKey, string taskName);

        void ExitTask(ContextKey contextKey);

        void AddLog(ContextKey contextKey, string log);
    }
}