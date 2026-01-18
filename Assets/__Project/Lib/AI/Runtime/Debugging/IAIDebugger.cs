using System.Collections.Generic;

namespace Gast.Lib.AI.Debugging
{
    public interface IAIDebugger
    {
        void Register(object actorId);
        void Unregister(object actorId);
        AIDebugInfo GetDebugInfo(object actorId);
        IReadOnlyDictionary<object, AIDebugInfo> GetAllDebugInfo();
        void UpdateWorldState(object actorId, string worldState);
        void UpdatePlan(object actorId, IReadOnlyList<string> plan);
        void UpdateActiveTaskPath(object actorId, string taskPath);
        void AddLog(object actorId, string log);
    }
}
